using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;
using System.Net;

namespace Nim
{
	public class GameManager : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private List<Cookie> _cookies = new();

		private Song MainTheme;

		bool isPlaying = false;
		enum Turn { Player, Opponent }
		Turn turn;

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

			Setup();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

			MainTheme = Content.Load<Song>("MainTheme");
			MediaPlayer.Play(MainTheme);
			MediaPlayer.IsRepeating = true;
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
					if (Keyboard.GetState().IsKeyDown(Keys.Q))
					{
						TakeFromPile(1, 0);
						Debug.WriteLine(board.piles[0].Count());
					}
					if (Keyboard.GetState().IsKeyDown(Keys.W))
					{
						TakeFromPile(1, 1);
						Debug.WriteLine(board.piles[1].Count());
					}
					break;

				case Turn.Opponent:
					turn = Turn.Player;
					break;

				default:
					break;
			}


			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			_spriteBatch.Begin();

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
						1 //LayerDepth
						);
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

			int k = 1;
			for (int i = 0; i < 3; i++) // n = 3; Create n piles.
			{
				r = new(Guid.NewGuid().GetHashCode()); // re-seed the random.
				var cookie = _cookies[r.Next(0, _cookies.Count - 1)];
				Pile<Cookie> pile = new(k*2, _cookies[r.Next(0, _cookies.Count - 1)]);
				board.piles.Add(pile);
				k += 2;
			}

			foreach (var pile in board.piles)
			{
				for (var i = 0; i < pile.Get().Count; i++)
				{
					var cookie = pile.Get()[i];
					pile.Get()[i] = new(cookie); // Copy cookie into pile as new object to randomize the position.
					r = new(Guid.NewGuid().GetHashCode());
					cookie._drawLocation = new Vector2(r.Next(800, 1600), r.Next(300, 800)); // Random draw location for now.
				}
			}

			turn = Turn.Player;
		}

		public void GameOver()
		{

		}
		#endregion
		#region PlayerController
		public void TakeFromPile(int count, int pileIndex)
		{
			if (board.piles.Count <= pileIndex) return;
			if (board.piles[pileIndex].IsEmpty()) return;

			for (int i = 0; i < count; i++)
			{
				board.piles[pileIndex].RemoveFirst();
				if (board.piles[pileIndex].IsEmpty())
				{
					break;
				}
			}

			turn = Turn.Opponent;
		}
		#endregion
	}
}
