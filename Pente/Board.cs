using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using static Pente.Board;

namespace Pente
{
	public class Board 
	{
		public struct Piece
		{
			public Piece(Piece piece)
			{
				this = piece;
			}

			public Piece(Texture2D tex, bool playerPiece = true)
			{
				_texture = tex;
				playerOwned = playerPiece;
			}

			public Texture2D _texture { get; private set; }
			public bool playerOwned = true;
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

		public Space[,] board = new Space[19, 19];
		public Texture2D Texture;

		public void Clear(Texture2D texture)
		{
			int k = 19;
			board = new Space[k, k];
			int vx = 504; int vy = 85;
			Vector2 vec2 = new(vx, vy);
			for (uint x = 0; x < k; x++)
			{
				vy = 85;
				for (uint y = 0; y < k; y++)
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

		public bool TrySetPiece(Piece piece, MouseState mouse)
		{
			for (uint x = 0; x < board.GetLength(0); x++)
			{
				for (uint y = 0; y < board.GetLength(1); y++)
				{
					if (board[x, y].TrySetPiece(piece, mouse))
						return true;
				}
			}
			return false;
		}

		public void RandomSetPiece(Piece piece)
		{
			Random rand = new();

			bool placed = false;

			while (!placed)
			{
				uint x = (uint)rand.Next(0, board.GetLength(0));
				uint y = (uint)rand.Next(0, board.GetLength(1));

				placed = board[x,y].TrySetOpponentPiece(piece);
			}
			
			

		}
	}
}
