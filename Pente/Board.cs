using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Pente
{
	public class Board 
	{
		public Board()
		{
			board = new Space[boardSize, boardSize];
		}

		public Board(int size, int width = 600, int height = 800)
		{
			boardSize = size;

			this.width = width;
			this.height = height;

			s = (int) (height * 0.83333333333f); // size in pixels of board
			//k = (int) ((height - s) * 0.5f); // 1/6 the size of the window vertical
			k = (int) (height * 0.16666666666f); // 1/6 the size of the window vertical
			r = (int) ((width - s) * 0.5f); // distance between edge of width and board start
			
			p = (s / (boardSize + 2)); // the size the image must be in order to fit the grid in the window
			hp = (int)(p * (0.5f)); // half p

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

			public void Draw(ref SpriteBatch spriteBatch, Point mousePos, int size)
			{
				Rectangle rect = new(origin.ToPoint(), new(size));
				if (piece != null)
				{
					spriteBatch.Draw(
					piece?._texture,
					destinationRectangle: rect,
					Color.White
					);;
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

			public bool TrySetPiece(Piece piece, MouseState mouse, int size)
			{
				Rectangle rect = new(origin.ToPoint(), new(size));
				if (rect.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
				{
					if (this.piece == null)
					{
						this.piece = new(piece);
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
		int width, height;
		public int boardSize = 19;
		public Texture2D Texture;

		int s, k, r, p, hp;

		public Dictionary<string, Texture2D> BoardSlices = new()
		{
			{ "top_slice", null },
			{ "right_slice", null },
			{ "bottom_slice", null },
			{ "left_slice", null },
			{ "middle_slice", null },
			{ "top_right_slice", null },
			{ "top_left_slice", null },
			{ "bottom_right_slice", null },
			{ "bottom_left_slice", null }
		};

		public void Clear(Texture2D texture)
		{
			board = new Space[boardSize, boardSize];
			int vx = r+hp; int vy = k+p;
			Vector2 vec2 = new(vx, vy);
			for (uint x = 0; x < boardSize; x++)
			{
				vy = k;
				for (uint y = 0; y < boardSize; y++)
				{
					vec2 = new(vx, vy);
					board[x, y] = new(vec2);
					board[x, y].emptyTex = texture;
					vy += p;
				}

				vx += p;
			}
		}

		public void Draw(ref SpriteBatch spriteBatch, Point mousePos)
		{
			//s = (int) (height * 0.666f); // size in pixels of board
			//k = (int) ((height - s) * 0.5f); // 1/6 the size of the window vertical
			//r = (int) ((width - s) * 0.5f); // distance between edge of width and board start
			//
			//p = (int)(s / (boardSize + 2)); // the size the image must be in order to fit the grid in the window
			//hp = (int)(p * (0.5f)); // half p

			Point topLeft = new Point(r - hp, k - p); // topLeft Position of the board, given the rect is anchored at the top left
			Point topRight = new Point(r + (p * boardSize) + hp, k - p);
			Point bottomLeft = new Point(r - hp, k + (p * boardSize));
			Point bottomRight = new Point(r + (p * boardSize) + hp, k + (p * boardSize));

			spriteBatch.Draw(
					BoardSlices["top_left_slice"],
					destinationRectangle: new Rectangle(topLeft, new(p,p)),
					Color.White
					);
			spriteBatch.Draw(
					BoardSlices["top_right_slice"],
					destinationRectangle: new Rectangle(topRight, new(p, p)),
					Color.White
					);
			spriteBatch.Draw(
					BoardSlices["bottom_left_slice"],
					destinationRectangle: new Rectangle(bottomLeft, new(p, p)),
					Color.White
					);
			spriteBatch.Draw(
					BoardSlices["bottom_right_slice"],
					destinationRectangle: new Rectangle(bottomRight, new(p, p)),
					Color.White
					);

			int ex = r + hp;
			int ey = k;

			for (uint x = 0; x < boardSize; x++)
			{
				for (uint y = 0; y < boardSize; y++)
				{
					Rectangle rect = new((int)(ex + (x*p)), (int)(ey + (y*p)), p, p);

					//_spriteBatch.Draw(
					//			cookie._texture, // Texture
					//			cookie._drawLocation, // Draw Coordinate
					//			null, // Source Rect.
					//			Color.White, // Tint
					//			0, // Rotation
					//			Vector2.Zero, // Origin
					//			Vector2.One * 2, // Scale
					//			default,// Sprite Effects
					//			0 //LayerDepth
					//			);

					spriteBatch.Draw(
					BoardSlices["middle_slice"],
					destinationRectangle: rect,
					Color.White
					);

					//spriteBatch.Draw(
					//BoardSlices["middle_slice"],
					//new Vector2((int)(ex + (x * p)), (int)(ey + (y * p))),
					//rect,
					//Color.White,
					//0,
					//Vector2.Zero,
					//Vector2.One,
					//default,
					//1
					//);
				}
			}

			ey = k - p;

			for (uint x = 0; x < boardSize; x++)
			{
				Rectangle rect = new((int)(ex + (x * p)), ey, p, p);

				spriteBatch.Draw(
				BoardSlices["top_slice"],
				destinationRectangle: rect,
				Color.White
				);
			}

			ey = k + (p * boardSize);
			for (uint x = 0; x < boardSize; x++)
			{
				Rectangle rect = new((int)(ex + (x * p)), ey, p, p);

				spriteBatch.Draw(
				BoardSlices["bottom_slice"],
				destinationRectangle: rect,
				Color.White
				);
			}

			ex = r - hp;
			for (uint y = 0; y < boardSize; y++)
			{
				Rectangle rect = new(ex, (int)(k + (y * p)), p, p);

				spriteBatch.Draw(
				BoardSlices["left_slice"],
				destinationRectangle: rect,
				Color.White
				);
			}

			ex = r + (p * boardSize) + hp;
			for (uint y = 0; y < boardSize; y++)
			{
				Rectangle rect = new(ex, (int)(k + (y * p)), p, p);

				spriteBatch.Draw(
				BoardSlices["right_slice"],
				destinationRectangle: rect,
				Color.White
				);
			}

			for (uint x = 0; x < board.GetLength(0); x++)
			{
				for (uint y = 0; y < board.GetLength(1); y++)
				{
					board[x, y].Draw(ref spriteBatch, mousePos, p);
				}
			}
		}

		public bool TrySetPiece(Piece piece, MouseState mouse, out bool winningMove, ref int captures)
		{
			winningMove = false;
			for (uint x = 0; x < board.GetLength(0); x++)
			{
				for (uint y = 0; y < board.GetLength(1); y++)
				{
					if (board[x, y].TrySetPiece(piece, mouse, p))
					{
						winningMove = CheckWin(new(x,y), ref captures);
						return true;
					}
				}
			}
			return false;
		}

		public void RandomSetPiece(Piece piece, out bool winningMove, ref int captures)
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

			winningMove = CheckWin(new(x,y), ref captures);
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

		public bool CheckWin(Vector2 coord, ref int captures)
		{
			Owner owner = board[(int)coord.X, (int)coord.Y].piece.Value.owner;

			foreach (var direction in Directions)
			{
				int count = 1;

				CheckLine(coord, direction, owner, ref count, ref captures);
				CheckLine(coord, -direction, owner, ref count, ref captures);

				if (count >= 5 || captures >= 5)
					return true;
			}

			return false;
		}

		public void CheckLine(Vector2 coord, Vector2 dir, Owner owner, ref int count, ref int captures)
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
					CheckLine(next, dir, owner, ref count, ref captures);
				}
				else
				{
					bool isCapture = CheckCapture(next, dir, owner);
					if (isCapture)
					{
						captures++;
						board[(int)next.X, (int)next.Y].piece = null;
						board[(int)(next.X + dir.X), (int)(next.Y + dir.Y)].piece = null;
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
				if (board[(int)next.X, (int)next.Y].piece.Value.owner != owner)
				{
					next = next + dir;
					if ((next.X < 0) || (next.X > max) || (next.Y < 0) || (next.Y > max))
						return false;

					if (board[(int)next.X, (int)next.Y].piece != null)
						if (board[(int)next.X, (int)next.Y].piece.Value.owner == owner)
							return true;
				}
			}

			return false;
		}
	}
}
