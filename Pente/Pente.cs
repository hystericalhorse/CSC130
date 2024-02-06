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
		public enum Turn { Player, PlayerTwo, AI };
		private Turn turn;

		public enum GameState { Menu, Pause, Play, Over };
		private GameState gameState;
		public enum Mode { PVP, PVC }
		private Mode gameMode;

		private bool mouseUp = true;
		private double timer;

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

			NewGame(Mode.PVC);
			//NewGame();

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
							if (timer <= 0)
							{
								mouseUp = false;
								PassTurn();
							}

							if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouseUp)	
							{
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleBlueSparkle")), Mouse.GetState()))
								{
									mouseUp = false;
									PassTurn();
								}
								
							}
							break;
						case Turn.AI:
							if (timer <= 0)
							{
								board.RandomSetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), false));
								PassTurn();
							}
							break;
						case Turn.PlayerTwo:
							if (timer <= 0)
							{
								mouseUp = false;
								PassTurn();
							}

							if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouseUp)
							{
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle")), Mouse.GetState()))
								{
									mouseUp = false;
									PassTurn();
								}

							}
							break;
					}
					
					break;
				case GameState.Over:
					break;
			}

			mouseUp = Mouse.GetState().LeftButton == ButtonState.Released;
			base.Update(gameTime);

			if (timer > 0)
				timer -= gameTime.ElapsedGameTime.TotalSeconds;
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
		public void NewGame(Mode mode = Mode.PVP)
		{
			board = new();
			board.Clear(Content.Load<Texture2D>("Sprites/Default"));
			turn = Turn.Player;

			gameMode = mode;

			gameState = GameState.Play;
			NewTurn(Turn.Player);
		}

		public void PassTurn()
		{
			switch (gameMode)
			{
				default:
				case Mode.PVC:
					NewTurn((turn == Turn.Player) ? Turn.AI : Turn.Player);
					break;
				case Mode.PVP:
					NewTurn((turn == Turn.Player) ? Turn.PlayerTwo : Turn.Player);
					break;
			}

		}

		public void NewTurn(Turn turn)
		{
			switch (turn)
			{
				case Turn.Player:
				case Turn.PlayerTwo:
					timer = 20;
					this.turn = turn;
					break;
				case Turn.AI:
					Random rnd = new();
					timer = rnd.NextInt64(1, 4);
					this.turn = turn;
					break;
			}

		}

		#endregion
	}
}
