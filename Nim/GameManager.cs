using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;
using System.Net;
using System.Xml;
using Microsoft.Xna.Framework.Audio;

namespace Nim
{
	public class GameManager : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private List<Cookie> _cookies = new();

		private Song MainTheme;

		bool isPlaying = false;
		public enum Turn { Player, Opponent, GameOver }
		Turn turn;
		int plateSelected = -1;

		private ButtonState lastButtonState;

		Rectangle EndTurnButtonRect;
		Texture2D EndTurnButton;

		Rectangle PlateRect;
		Rectangle PlateRect1;
		Rectangle PlateRect2;
		Texture2D PlateTexture;

		string winnerMsg;

		SpriteFont font;

		AIOpponent opponent = new();
		Board<Cookie> board = new();

		#region MonoGame
		public GameManager()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			if (GraphicsDevice == null) _graphics.ApplyChanges();

			_graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
			_graphics.ApplyChanges();
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font");

			// TODO: use this.Content to load your game content here

			EndTurnButton = Content.Load<Texture2D>("Sprites/EndTurnButton");
			Vector2 pos = new Vector2(
					GraphicsDevice.Viewport.Bounds.Width* 0.5f -EndTurnButton.Width * 0.5f * 0.5f,
					GraphicsDevice.Viewport.Bounds.Height - EndTurnButton.Height * 0.5f
					);
			EndTurnButtonRect = new Rectangle((int)pos.X, (int)pos.Y, (int)(EndTurnButton.Width * 0.5),(int)( EndTurnButton.Height * 0.5f));

			PlateTexture = Content.Load<Texture2D>("Sprites/Plate");
			pos = new Vector2(
					(GraphicsDevice.Viewport.Bounds.Width * 0.33f) - (PlateTexture.Width * 1.5f),
					(GraphicsDevice.Viewport.Bounds.Height * 0.5f) - (PlateTexture.Height* 0.5f * 1.5f)
					);
			PlateRect = new((int)pos.X, (int)pos.Y, 768, 768);
			pos = new Vector2(
					(GraphicsDevice.Viewport.Bounds.Width * 0.66f) - (PlateTexture.Width * 1.5f),
					(GraphicsDevice.Viewport.Bounds.Height * 0.5f) - (PlateTexture.Height * 0.5f * 1.5f)
					);
			PlateRect1 = new((int)pos.X, (int)pos.Y, 768, 768);
			pos = new Vector2(
					(GraphicsDevice.Viewport.Bounds.Width * 0.99f) - (PlateTexture.Width * 1.5f),
					(GraphicsDevice.Viewport.Bounds.Height * 0.5f) - (PlateTexture.Height * 0.5f * 1.5f)
					);
			PlateRect2 = new((int)pos.X, (int)pos.Y, 768, 768);

			MainTheme = Content.Load<Song>("MainTheme");
			MediaPlayer.Play(MainTheme);
			//MediaPlayer.IsRepeating = true;

			Setup();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(MainTheme);
			else if (MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();

			switch (turn)
			{
				case Turn.Player:
					MouseState mouse = Mouse.GetState();

					if (mouse.LeftButton == ButtonState.Pressed && lastButtonState != ButtonState.Pressed)
					{
						if (EndTurnButtonRect.Contains(mouse.Position))
						{
							NextTurn();
							break;
						}


						if (plateSelected < 0)
						{
							if (PlateRect.Contains(mouse.Position))
							{
								plateSelected = 0;
							}
							if (PlateRect1.Contains(mouse.Position))
							{
								plateSelected = 1;
							}
							if (PlateRect2.Contains(mouse.Position))
							{
								plateSelected = 2;
							}

							TakeFromPile(1, plateSelected, false);
						}
						else
						{
							TakeFromPile(1, plateSelected, false);
						}
					}
					break;

				case Turn.Opponent:
					Debug.WriteLine("Opponent Turn");
					NextTurn();
					break;

				default:
					MouseState mous2e = Mouse.GetState();
					if (mous2e.LeftButton == ButtonState.Pressed && lastButtonState != ButtonState.Pressed)
					{
						System.Environment.Exit(0);
					}
					break;
			}

			MouseState mouse1 = Mouse.GetState();
			lastButtonState = mouse1.LeftButton;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			_spriteBatch.Begin(SpriteSortMode.BackToFront);

			switch (turn)
			{
				case Turn.GameOver:
					_spriteBatch.DrawString(
						font, 
						winnerMsg + " Click to exit.", 
						new (GraphicsDevice.Viewport.Bounds.Width * 0.5f, GraphicsDevice.Viewport.Bounds.Height * 0.5f),
						Color.Red);
					break;
				default:
					foreach (var pile in board.piles)
						foreach (var cookie in pile.Get())
						{
							_spriteBatch.Draw(
								cookie._texture, // Texture
								cookie._drawLocation, // Draw Coordinate
								null, // Source Rect.
								Color.White, // Tint
								0, // Rotation
								Vector2.Zero, // Origin
								Vector2.One * 2, // Scale
								default,// Sprite Effects
								0 //LayerDepth
								);
						}

					_spriteBatch.Draw(EndTurnButton, EndTurnButtonRect, Color.White);

					_spriteBatch.Draw(PlateTexture, PlateRect, null, Color.White, 0, Vector2.Zero, default, 1);
					_spriteBatch.Draw(PlateTexture, PlateRect1, null, Color.White, 0, Vector2.Zero, default, 1);
					_spriteBatch.Draw(PlateTexture, PlateRect2, null, Color.White, 0, Vector2.Zero, default, 1);
					
					break;
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}
		#endregion
		#region GameManager
		public void Menu()
		{

		}

		public void Setup()
		{
			_cookies.Clear();
			_cookies.Add(new ChocolateChip(Content.Load<Texture2D>("Sprites/ChocolateChipCookie")));
			_cookies.Add(new Oatmeal(Content.Load<Texture2D>("Sprites/OatmealCookie")));
			_cookies.Add(new Snickerdoodle(Content.Load<Texture2D>("Sprites/SnickerdoodleCookie")));
			_cookies.Add(new Lofthouse(Content.Load<Texture2D>("Sprites/LofthouseCookie")));

			Random r = new(Guid.NewGuid().GetHashCode());

			int k = 2;
			for (int i = 0; i < 3; i++) // n = 3; Create n piles.
			{
				r = new(Guid.NewGuid().GetHashCode()); // re-seed the random.
				var cookie = _cookies[r.Next(0, _cookies.Count)];
				Pile<Cookie> pile = new(k * 2, cookie);
				board.piles.Add(pile);
				k++;
			}
			//{X:76.80005 Y:416}
			board.piles[0].pileCenter = PlateRect.Center.ToVector2();
			//{X:921.6001 Y:416}
			board.piles[1].pileCenter = PlateRect1.Center.ToVector2();
			//{X:1766.3999 Y:416}
			board.piles[2].pileCenter = PlateRect2.Center.ToVector2();

			foreach (var pile in board.piles)
			{
				for (var i = 0; i < pile.Get().Count; i++)
				{
					var cookie = pile.Get()[i];
					cookie._drawLocation = RandomInCircle(pile.pileCenter, 128); // Random draw location for now.
					pile.Get()[i] = new(cookie); // Copy cookie into pile as new object to randomize the position.
				}
			}

			turn = Turn.Player;
		}

		public void GameOver(Turn winner)
		{
			Debug.WriteLine(winner.ToString());
			switch (winner)
			{
				case Turn.Player: winnerMsg = "You Win!"; break;
				case Turn.Opponent: winnerMsg = "AI Wins!"; break;
				default: break;
			}
			turn = Turn.GameOver;
		}
		#endregion
		#region PlayerController
		public void TakeFromPile(int count, int pileIndex, bool endTurn = true)
		{
			if (board.piles.Count <= pileIndex || pileIndex < 0)
			{
				Content.Load<SoundEffect>("Nimsounds/Plate-03").Play();
				return;
			}
			if (board.piles[pileIndex].IsEmpty())
			{
				Content.Load<SoundEffect>("Nimsounds/Plate-03").Play();
				return;
			}

			for (int i = 0; i < count; i++)
			{
				board.piles[pileIndex].RemoveFirst();
				if (board.piles[pileIndex].IsEmpty())
				{
					break;
				}
			}

			Content.Load<SoundEffect>("Nimsounds/Monch-03").Play();
			Debug.WriteLine(board.piles[0].Count() + " | " + board.piles[1].Count() + " | " + board.piles[2].Count());
			if (endTurn) NextTurn();
		}

		public void NextTurn()
		{
			switch (turn)
			{
				case Turn.Opponent:
					if (BoardEval())
					{
						GameOver(Turn.Opponent);
						return;
					}
					turn = Turn.Player;
					break;
				case Turn.Player:
					if (BoardEval())
					{
						GameOver(Turn.Player);
						return;
					}
					plateSelected = -1;
					turn = Turn.Opponent;
					break;
				default: break;
			}
		}

		public bool BoardEval()
		{
			if (board == null || board.piles.Count < 1) return false;
			for (int i = 0; i < board.piles.Count; i++)
			{
				if (board.piles[i].Get().Count > 1) return false;
			}

			foreach (var pile in board.piles)
			{
				if (pile.Get().Count > 1)
					return false;
				if (pile.IsEmpty()) continue;
				foreach (var other in board.piles)
				{
					if (pile == other || other.IsEmpty()) continue;
					if (pile.Get().Count == 1 && !other.IsEmpty())
						return false;
				}
			}

			return true;
		}

		public Vector2 RandomInCircle(Vector2 center, float radius)
		{
			var r = new Random(Guid.NewGuid().GetHashCode());
			float angle = 2.0f * MathF.PI * r.Next();
			var x = center.X + radius * MathF.Cos(angle);
			var y = center.Y + radius * MathF.Sin(angle);
			return new Vector2(x, y);
		}
		#endregion
	}
}