using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nim
{
	public class GameManager : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Texture2D OatmealCookie;
		private Texture2D OatmealCookie2;
		private Texture2D ChocolateChipCookie;
		private Texture2D ChocolateChipCookie2;
		private Texture2D Snickerdoodle;
		private Texture2D Snickerdoodle2;

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

			Play();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

			MainTheme = Content.Load<Song>("MainTheme");
			MediaPlayer.Play(MainTheme);
			MediaPlayer.IsRepeating = true;

            OatmealCookie = Content.Load<Texture2D>("Sprites/OatmealCookie");
            OatmealCookie2 = Content.Load<Texture2D>("Sprites/OatmealCookieBigger");
			ChocolateChipCookie = Content.Load<Texture2D>("Sprites/ChocochipCookieSprite");
			ChocolateChipCookie2 = Content.Load<Texture2D>("Sprites/ChocochipCookieSpriteSlightlyBigger");
			Snickerdoodle = Content.Load<Texture2D>("Sprites/Snickerdoodle");
			Snickerdoodle2 = Content.Load<Texture2D>("Sprites/SnickerDoodleBigger");
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

			_spriteBatch.Draw(OatmealCookie, new Vector2(1250, 800), Color.White);
			_spriteBatch.Draw(OatmealCookie2, new Vector2(1025, 600), Color.White);
			_spriteBatch.Draw(ChocolateChipCookie, new Vector2(1575, 800), Color.White);
			_spriteBatch.Draw(ChocolateChipCookie2, new Vector2(1150, 300), Color.White);
			_spriteBatch.Draw(Snickerdoodle, new Vector2(900, 800), Color.White);
			_spriteBatch.Draw(Snickerdoodle2, new Vector2(1400, 600), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
		#endregion
		#region GameManager
		public void Menu()
		{

		}

		public void Play()
		{
			Pile<Cookie> pile = new(4, new ChocolateChip());
			board.piles.Add(pile);
			pile = new(6, new Snickerdoodle());
			board.piles.Add(pile);

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
