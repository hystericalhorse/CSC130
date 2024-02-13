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


			Menu();
			//NewGame(Mode.PVP);
			//NewGame();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			
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
					newGame.Update(Mouse.GetState(), mouseUp);
					newGamePVAI.Update(Mouse.GetState(), mouseUp);
					break;
				case GameState.Pause:
					pauseTurn.Update(Mouse.GetState(), mouseUp);
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
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleBlueSparkle"), Board.Owner.Player), Mouse.GetState(), out bool winningMove))
								{
									mouseUp = false;
									if (winningMove)
										GameOver();
									else
										PassTurn();
								}
								
							}
							break;
						case Turn.AI:
							if (timer <= 0)
							{
								board.RandomSetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), Board.Owner.AI), out bool winningMove);
								if (winningMove)
									GameOver();
								else
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
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), Board.Owner.PlayerTwo), Mouse.GetState(), out bool winningMove))
								{
									mouseUp = false;
									if(winningMove)
										GameOver();
									else
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
			switch (gameState)
			{
				default:
				case GameState.Menu:
					newGame.Draw(ref _spriteBatch);
					newGamePVAI.Draw(ref _spriteBatch);
					break;
				case GameState.Play:
					_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					board.Draw(ref _spriteBatch, Mouse.GetState().Position);
					break;
				case GameState.Pause:
					_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					pauseTurn.Draw(ref _spriteBatch);
					break;
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		#region GameManager

		Button newGame;
		Button newGamePVAI;

		Button pauseTurn;

		public void Menu()
		{
			float posx = _graphics.PreferredBackBufferWidth * 0.5f;
			newGame = new(new((int)posx - (int)(512 * 0.5f), 700, 512, 128) ,Content.Load<Texture2D>("Sprites/Button"));
			newGame.onClick += () => { NewGame(); };

			newGamePVAI = new(new((int)posx - (int)(512 * 0.5f), 500, 512, 128), Content.Load<Texture2D>("Sprites/Button"));
			newGame.onClick += () => { NewGame(Mode.PVC); };

			gameState = GameState.Menu;
		}

		public void NewGame(Mode mode = Mode.PVP)
		{
			board = new(19);
			board.Texture = Content.Load<Texture2D>("Sprites/GameBoard");
			board.Clear(Content.Load<Texture2D>("Sprites/Default"));
			turn = Turn.Player;

			gameMode = mode;

			gameState = GameState.Play;
			NewTurn(Turn.Player);
		}

		public void PauseTurn()
		{
			gameState = GameState.Play;
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

		public void GameOver()
		{
			gameState = GameState.Over;

			Debug.WriteLine($"Game Over. Winner:{turn}");
			switch (turn)
			{
				case Turn.Player:
				case Turn.PlayerTwo:
				case Turn.AI:
					break;
			}
		}

		#endregion
	}
}
