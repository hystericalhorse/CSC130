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

		private int currentBoardSize = 19;

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

			currentBoardSize = 19;
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

			blueMarble = Content.Load<Texture2D>("Sprites/MarbleBlueSparkle");
			redMarble = Content.Load<Texture2D>("Sprites/MarbleRedSparkle");
				
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
					SettingsButton.Update(Mouse.GetState(), mouseUp);
					foreach (var button in boardSizeButtons)
						button.Update(Mouse.GetState(), mouseUp);
					break;
				case GameState.Play:
					ReturnToMenuButton.Update(Mouse.GetState(), mouseUp);
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
					RestartMenuButton.Update(Mouse.GetState(), mouseUp);
					break;
			}

			mouseUp = Mouse.GetState().LeftButton == ButtonState.Released;
			base.Update(gameTime);

			if (timer > 0)
				timer -= gameTime.ElapsedGameTime.TotalSeconds;
		}

		// move these
		private Texture2D blueMarble;
		private Texture2D redMarble;
		//

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Affair);

			_spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
			// TODO: Add your drawing code here
			switch (gameState)
			{
				default:
				case GameState.Menu:
					foreach (var button in MenuButtons)
						button.Draw(ref _spriteBatch, ref arialFont);

					BoardSizeTextbox.Draw(ref _spriteBatch, ref arialFont);
					break;
				case GameState.Play:
					//_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					board.Draw(ref _spriteBatch, Mouse.GetState().Position);
					ReturnToMenuButton.Draw(ref _spriteBatch, ref arialFont);

					DrawGameUI();
					break;
				case GameState.Pause:
					//_spriteBatch.Draw(board.Texture, board.Texture.Bounds, Color.White);
					//PauseTurnButton.Draw(ref _spriteBatch);

					SettingsButton.Draw(ref _spriteBatch);
					foreach (var button in boardSizeButtons)
						button.Draw(ref _spriteBatch, ref arialFont);

					break;
				case GameState.Over:
					DrawGameOverUI();
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

		Button RestartMenuButton;

		List<Button> MenuButtons = new();

		Textbox BoardSizeTextbox;
		public void Menu()
		{
			NewGamePVPButton = new Button(new Rectangle(halfScreenWidth - (int)(512 * 0.5f), 500, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "New PVP Game");
			NewGamePVPButton.onClick += () => { NewGame(Mode.PVP); };

			NewGamePVCButton = new Button(new Rectangle(halfScreenWidth - (int)(512 * 0.5f), 700, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "New PVC Game");
			NewGamePVCButton.onClick += () => { NewGame(Mode.PVC); };

			SettingsButton = new Button(new Rectangle(0, 0, 64, 64), Content.Load<Texture2D>("Sprites/Gear"));
			SettingsButton.onClick += () => { Settings(); };

			BoardSizeTextbox = new($"Board Size: {currentBoardSize}", new(64 + 32, 10));

			MenuButtons.Clear();
			MenuButtons.Add(NewGamePVCButton);
			MenuButtons.Add(NewGamePVPButton);
			MenuButtons.Add(SettingsButton);


			gameState = GameState.Menu;
		}

		public void GoToMenu()
		{
			Console.WriteLine($"{currentBoardSize}");
			Menu();
		}

		Button ReturnToMenuButton;
		public void NewGame(Mode mode = Mode.PVP)
		{
			board = new(currentBoardSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
			board.Texture = Content.Load<Texture2D>("Sprites/GameBoard");
			board.Clear(Content.Load<Texture2D>("Sprites/Default"));

			board.BoardSlices["top_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_t");
			board.BoardSlices["right_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_r");
			board.BoardSlices["left_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_l");
			board.BoardSlices["bottom_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_b");
			board.BoardSlices["middle_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_m");
			board.BoardSlices["top_left_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_tl");
			board.BoardSlices["top_right_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_tr");
			board.BoardSlices["bottom_left_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_bl");
			board.BoardSlices["bottom_right_slice"] = Content.Load<Texture2D>("Sprites/Board/grid_br");
			turn = Turn.Player;

			ReturnToMenuButton = new Button(new Rectangle(halfScreenWidth + 420, halfScreenHeight + 200, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "Return to Menu");
			ReturnToMenuButton.onClick += () => { GoToMenu(); };

			gameMode = mode;
			playerCaptures = 0;
			playerTwoCaptures = 0;
			computerCaptures = 0;

			gameState = GameState.Play;
			NewTurn(Turn.Player);
		}

		List<Button> boardSizeButtons = new();
		struct Textbox
		{
			public Textbox(string text, Vector2 location)
			{
				this.Text = text;
				this.Location = location;
			}

			public string Text;
			public Vector2 Location;

			public void Draw(ref SpriteBatch spriteBatch, ref SpriteFont spriteFont)
			{
				spriteBatch.DrawString(spriteFont, Text, Location, Color.White);
			}
		}

		public void Settings()
		{
			SettingsButton.onClick = null;
			SettingsButton.onClick += () => { Menu(); };

			boardSizeButtons.Clear();
			
			int h = 80; int w = 80;
			int x = halfScreenWidth - (w * 7); int y = halfScreenHeight - h;

			for (int i = 0; i < 16; i++)
			{
				Rectangle rect = new(x + (w * i), y, w, h);
				int k = 9 + (i * 2);
				Button button = new Button(rect, Content.Load<Texture2D>("Sprites/Square"), k.ToString());

				button.onClick = () => {

					SetBoardSize(k);
					GoToMenu();
				};

				boardSizeButtons.Add(button);
			}

			gameState = GameState.Pause;
		}

		public void SetBoardSize(int i)
		{
			currentBoardSize = i;
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

			RestartMenuButton = new Button(new Rectangle(halfScreenWidth - (int)(512 * 0.5f), 500, 512, 128), Content.Load<Texture2D>("Sprites/Square"), "Return to Menu");
			RestartMenuButton.onClick += () => { GoToMenu(); };

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

		#region UI

		public void DrawGameUI()
		{
			// turn information
			int turnOffsetX = 1480;
			int turnOffsetY = 25;
			int timeroffsetY = 40;

			//marble offsets
			int marbleXOffset = 30;
			int marbleYOffset = 100;
			int marbleTxtOffsetX = 70; // 60
			int marbleTxtOffsetY = 10;
			int marble2OffsetY = 100;
			int captureStringOffset = 60; // draw will turn this negative

			_spriteBatch.Draw(blueMarble, new Rectangle(marbleXOffset, marbleYOffset, 64, 64), Color.White);
			_spriteBatch.Draw(redMarble, new Rectangle(marbleXOffset, marbleYOffset + marble2OffsetY, 64, 64), Color.White);

			string turnString = string.Empty;
			string oppCaptures = string.Empty;
			switch (gameMode)
			{
				case Mode.PVP:
					turnString = (turn == Turn.Player) ? "Player One's turn" : "Player Two's turn";
					oppCaptures = "X " + playerTwoCaptures;
					break;
				case Mode.PVC:
					turnString = (turn == Turn.Player) ? "Player One's turn" : "Computer's turn";
					oppCaptures = "X " + computerCaptures;
					break;
			}

			// draw string for blue marble
			_spriteBatch.DrawString(
				arialFont,
				oppCaptures,
				new(marbleXOffset + marbleTxtOffsetX, marbleYOffset + marbleTxtOffsetY),
				Color.White
				);

			// draw string for red marble
			_spriteBatch.DrawString(
				arialFont,
				"X " + playerCaptures,
				new(marbleXOffset + marbleTxtOffsetX, marbleYOffset + marbleTxtOffsetY + marble2OffsetY),
				Color.White
				);

			_spriteBatch.DrawString(
				arialFont,
				"Captures",
				new(marbleXOffset, marbleYOffset - captureStringOffset),
				Color.White
				);

			_spriteBatch.DrawString(
				arialFont,
				turnString,
				new(turnOffsetX, turnOffsetY),
				Color.White
				);

			_spriteBatch.DrawString(
				arialFont,
				(turn == Turn.AI) ? "" : "Time Remaining: " + string.Format("{0:0}", timer),
				new(turnOffsetX, turnOffsetY + timeroffsetY),
				Color.White
				);
		}

		public void DrawGameOverUI()
		{
			string winnerText = string.Empty;

			switch (turn)
			{
				case Turn.Player: winnerText = "Player One Wins"; break;
				case Turn.AI: winnerText = "Computer Wins"; break;
				case Turn.PlayerTwo: winnerText = "Player Two Wins"; break;
			}

			_spriteBatch.DrawString(
				arialFont,
				winnerText,
				new (halfScreenWidth - 165, halfScreenHeight - 100),
				Color.White
				);

			RestartMenuButton.Draw(ref _spriteBatch);
			RestartMenuButton.Draw(ref _spriteBatch, ref arialFont);
		}

		#endregion
	}
}
