using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Pente
{
	public class Pente : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private readonly Color Affair = new Color(116,80,133);

		private Board board;
		public enum Turn { Player, Opponent };
		private Turn turn;

		public enum GameState { Menu, Pause, Play, Over };
		private GameState gameState;

		private bool mouseUp = true;

		public Pente()
		{
			_graphics = new GraphicsDeviceManager(this);

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			_graphics.PreferredBackBufferWidth = 1920;
			_graphics.PreferredBackBufferHeight = 1080;
			_graphics.IsFullScreen = false;
			_graphics.ApplyChanges();

			NewGame();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			board.Texture = Content.Load<Texture2D>("Sprites/GameBoard");
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			switch (gameState)
			{
				default:
					break;
				case GameState.Menu:
					break;
				case GameState.Pause:
					break;
				case GameState.Play:
					switch (turn)
					{
						case Turn.Player:
							if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouseUp)	
							{
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleBlueSparkle")), Mouse.GetState()))
								{
									mouseUp = false;
									turn = Turn.Opponent;
								}
								
							}
							break;
						case Turn.Opponent:
							board.RandomSetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), false));
							turn = Turn.Player;
							break;
					}	
					
					break;
				case GameState.Over:
					break;
			}

			mouseUp = Mouse.GetState().LeftButton == ButtonState.Released;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Affair);

			_spriteBatch.Begin(SpriteSortMode.BackToFront);
			// TODO: Add your drawing code here
			_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
			board.Draw(ref _spriteBatch, Mouse.GetState().Position);

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		#region GameManager
		public void NewGame()
		{
			board = new();
			board.Clear(Content.Load<Texture2D>("Sprites/Default"));
			turn = Turn.Player;

			gameState = GameState.Play;
		}

		#endregion
	}
}
