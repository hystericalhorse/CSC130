using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pente
{
	public class Board 
	{
		public Board()
		{
			board = new Space[boardSize, boardSize];
		}

		public Board(int size)
		{
			boardSize = size;
			board = new Space[boardSize, boardSize];
		}

		public enum Owner { Player, PlayerTwo, AI}
		public struct Piece
		{
			public Piece(Piece piece)
			{
				this = piece;
			}

			public Piece(Texture2D tex, Owner owner = Owner.Player)
			{
				_texture = tex;
				this.owner = owner;
			}

			public Texture2D _texture { get; private set; }
			public Owner owner = Owner.Player;
		}

		public struct Space
		{
			public Space(Vector2 vec2)
			{
				origin = vec2;
			}

			public Texture2D emptyTex = null;

			public void Draw(ref SpriteBatch spriteBatch, Point mousePos)
			{
				Rectangle rect = new(origin.ToPoint(), new(48));
				if (piece != null)
				{
					spriteBatch.Draw(
					piece?._texture,
					destinationRectangle: rect,
					Color.White
					);
				}
				else
				if (rect.Contains(mousePos))
				{
					spriteBatch.Draw(
					emptyTex,
					destinationRectangle: rect,
					Color.White
					);
				}
			}

			public bool TrySetPiece(Piece p, MouseState mouse)
			{
				Rectangle rect = new(origin.ToPoint(), new(48));
				if (rect.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
				{
					if (this.piece == null)
					{
						this.piece = new(p);
						return true;
					}					
				}

				return false;
			}

			public bool TrySetOpponentPiece(Piece p)
			{
				if (this.piece == null)
				{
					this.piece = new(p);
					return true;
				}

				return false;
			}

			public Piece? piece = null;
			public Vector2 origin;
		}

		public Space[,] board;
		public int boardSize = 19;
		public Texture2D Texture;

		public void Clear(Texture2D texture)
		{
			//int k = 19;
			board = new Space[boardSize, boardSize];
			int vx = 504; int vy = 85;
			Vector2 vec2 = new(vx, vy);
			for (uint x = 0; x < boardSize; x++)
			{
				vy = 85;
				for (uint y = 0; y < boardSize; y++)
				{
					vec2 = new(vx, vy);
					board[x, y] = new(vec2);
					board[x, y].emptyTex = texture;
					vy += 48;
				}

				vx += 48;
			}
		}

		public void Draw(ref SpriteBatch spriteBatch, Point mousePos)
		{
			for (uint x = 0; x < board.GetLength(0); x++)
			{
				for (uint y = 0; y < board.GetLength(1); y++)
				{
					board[x, y].Draw(ref spriteBatch, mousePos);
				}
			}
		}

		public bool TrySetPiece(Piece piece, MouseState mouse, out bool winningMove)
		{
			winningMove = false;
			for (uint x = 0; x < board.GetLength(0); x++)
			{
				for (uint y = 0; y < board.GetLength(1); y++)
				{
					if (board[x, y].TrySetPiece(piece, mouse))
					{
						winningMove = CheckWin(new(x,y));
						return true;
					}
				}
			}
			return false;
		}

		public void RandomSetPiece(Piece piece, out bool winningMove)
		{
			Random rand = new();

			bool placed = false;
			winningMove = false;

			uint x = 0, y = 0;

			while (!placed)
			{
				x = (uint)rand.Next(0, board.GetLength(0));
				y = (uint)rand.Next(0, board.GetLength(1));

				placed = board[x,y].TrySetOpponentPiece(piece);
			}

			winningMove = CheckWin(new(x,y));
		}

		public static readonly Vector2 North = new(0,1);
		public static readonly Vector2 NorthEast = new(1,1);
		public static readonly Vector2 East = new(1,0);
		public static readonly Vector2 SouthEast = new(1,-1);
		public static readonly Vector2 South = new(0,-1);
		public static readonly Vector2 SouthWest = new(-1,-1);
		public static readonly Vector2 West = new(-1,0);
		public static readonly Vector2 NorthWest = new(-1,1);

		public readonly List<Vector2> Directions = new()
		{
			North, NorthEast, East, SouthEast
		};

		public bool CheckWin(Vector2 coord)
		{
			Owner owner = board[(int)coord.X, (int)coord.Y].piece.Value.owner;

			foreach (var direction in Directions)
			{
				int count = 1;

				CheckLine(coord, direction, owner, ref count);
				CheckLine(coord, -direction, owner, ref count);

				if (count >= 5)
					return true;
			}

			return false;
		}

		public void CheckLine(Vector2 coord, Vector2 dir, Owner owner, ref int count)
		{
			var next = coord + dir;
			var max = boardSize - 1;

			if ((next.X < 0) || (next.X > max) || (next.Y < 0) || (next.Y > max))
				return;

			if (count >= 5) return;

			if (board[(int)next.X, (int)next.Y].piece != null)
			{
				if (board[(int)next.X, (int)next.Y].piece.Value.owner == owner)
				{
					Debug.WriteLine(count);
					count++;
					CheckLine(next, dir, owner, ref count);
				}
				else
				{
					bool isCapture = CheckCapture(next, dir, owner);
					if (isCapture)
					{
						Debug.WriteLine($"Capture:{board[(int)next.X, (int)next.Y].piece.Value.owner}");
					}
				}
			}
		}

		public bool CheckCapture(Vector2 coord, Vector2 dir, Owner owner)
		{
			var next = coord + dir;
			var max = boardSize - 1;

			if ((next.X < 0) || (next.X > max) || (next.Y < 0) || (next.Y > max))
				return false;

			if (board[(int)next.X, (int)next.Y].piece != null)
			{
				if (board[(int)next.X, (int)next.Y].piece.Value.owner == owner)
					return true;
			}

			return false;
		}
	}
}
