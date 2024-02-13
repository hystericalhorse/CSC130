using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

		int playerCaptures;
		int playerTwoCaptures;
		int computerCaptures;

		SpriteFont arialFont;

		private bool mouseUp = true;
		private double timer;
		private int halfScreenWidth;
		private int halfScreenHeight;


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

			halfScreenWidth = (int) _graphics.PreferredBackBufferWidth / 2;
			halfScreenHeight = (int) _graphics.PreferredBackBufferHeight / 2;

			Menu();
			//NewGame(Mode.PVP);
			//NewGame();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			arialFont = Content.Load<SpriteFont>("Fonts/Arial");

			//_spriteBatch.DrawString(
			//			arial,
			//			"Hello World",
			//			new(GraphicsDevice.Viewport.Bounds.Width * 0.5f, GraphicsDevice.Viewport.Bounds.Height * 0.5f),
			//			Color.Red);
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
					foreach (var button in MenuButtons)
						button.Update(Mouse.GetState(), mouseUp);
					break;
				case GameState.Pause:
					PauseTurnButton.Update(Mouse.GetState(), mouseUp);
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
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleBlueSparkle"), Board.Owner.Player), Mouse.GetState(), out bool winningMove, ref playerCaptures))
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
								board.RandomSetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), Board.Owner.AI), out bool winningMove, ref computerCaptures);
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
								if (board.TrySetPiece(new(Content.Load<Texture2D>("Sprites/MarbleRedSparkle"), Board.Owner.PlayerTwo), Mouse.GetState(), out bool winningMove, ref playerTwoCaptures))
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

			_spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
			// TODO: Add your drawing code here
			switch (gameState)
			{
				default:
				case GameState.Menu:
					foreach (var button in MenuButtons)
						button.Draw(ref _spriteBatch, ref arialFont);
					break;
				case GameState.Play:
					_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					board.Draw(ref _spriteBatch, Mouse.GetState().Position);
					break;
				case GameState.Pause:
					_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					PauseTurnButton.Draw(ref _spriteBatch);
					break;
			}

			//_spriteBatch.DrawString(
			//			arialFont,
			//			"Hello World",
			//			new(GraphicsDevice.Viewport.Bounds.Width * 0.5f, GraphicsDevice.Viewport.Bounds.Height * 0.5f),
			//			Color.Red);

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		#region GameManager

		Button NewGamePVPButton;
		Button NewGamePVCButton;

		Button SettingsButton;

		Button PauseTurnButton;

		List<Button> MenuButtons = new();

		public void Menu()
		{
			NewGamePVPButton = new Button(new Rectangle(halfScreenWidth - (int)(512 * 0.5f), 700, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "New PVP Game");
			NewGamePVPButton.onClick += () => { NewGame(Mode.PVP); };

			NewGamePVCButton = new Button(new Rectangle(halfScreenWidth - (int)(512 * 0.5f), 500, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "New PVC Game");
			NewGamePVCButton.onClick += () => { NewGame(Mode.PVC); };

			SettingsButton = new Button(new Rectangle(0, 0, 64, 64), Content.Load<Texture2D>("Sprites/Gear"));
			SettingsButton.onClick += () => {  };

			MenuButtons.Clear();
			MenuButtons.Add(NewGamePVCButton);
			MenuButtons.Add(NewGamePVPButton);
			MenuButtons.Add(SettingsButton);

			gameState = GameState.Menu;
		}

		public void NewGame(Mode mode = Mode.PVP)
		{
			board = new(19);
			board.Texture = Content.Load<Texture2D>("Sprites/GameBoard");
			board.Clear(Content.Load<Texture2D>("Sprites/Default"));
			turn = Turn.Player;

			gameMode = mode;
			playerCaptures = 0;
			playerTwoCaptures = 0;
			computerCaptures = 0;

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
