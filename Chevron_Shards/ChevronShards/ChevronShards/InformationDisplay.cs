using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	/// InformationDisplay Class 
	///                          
	/// This classes purpose is to manage the overall game variables such as gameOver, the class is also used to update and draw 
	/// information such as such as error screens or completion screens.

	public class InformationDisplay // class for game information and displaying information
	{
		private Texture2D gameErrorTexture;

		private Texture2D gameCompleteTexture;

		private Texture2D DungeonCompleted;
		private Texture2D GameOverStory;

		private Texture2D FirstDayGraphic;
		private Texture2D FinalDayGraphic;

		private Texture2D SplashScreen;
		private Texture2D ControlsScreen;

		private Texture2D TitleScreen; // With text
		private Texture2D TitleScreenNT; // No text

		private Texture2D EnergyBarrierTexture;

		private Texture2D EntityKilledTexture;


		private bool _gameError;

		public bool GameError { get { return _gameError; } set { _gameError = value; } }

		private string _gameErrorCode; // type of error expressed as a code
		public string GameErrorCode { get { return _gameErrorCode; } set { _gameErrorCode = value; } }

		private bool _init; // boolean denotes whether game should restart
		public bool Init { get { return _init; } set { _init = value; } }

		private int _SplashScreenTime; // how long has the splash screen been displayed for
		public int SplashScreenTime { get { return _SplashScreenTime; } set { _SplashScreenTime = value; } }

		private bool _gameComplete; // has the game been completed
		public bool GameComplete { get { return _gameComplete; } set { _gameComplete = value; } }



		private int _SaveFileNumber;
		public int SaveFileNumber { get { return _SaveFileNumber; } set { _SaveFileNumber = value; } }


		private bool _gamePaused; // has the game been paused
		public bool GamePaused { get { return _gamePaused; } set { _gamePaused = value; } }

		// whether buttons should be registered, i.e. one press at a time.
		private bool _registerStartPress;
		public bool RegisterStartPress { get { return _registerStartPress; } set { _registerStartPress = value; } }

		private bool _registerSelectPress;
		public bool RegisterSelectPress { get { return _registerSelectPress; } set { _registerSelectPress = value; } }

		private bool _registerBPress;
		public bool RegisterBPress { get { return _registerBPress; } set { _registerBPress = value; } }


		private bool _showLoadScreen;
		public bool ShowLoadScreen { get { return _showLoadScreen; } set { _showLoadScreen = value; } }





		private bool _GameOver;
		public bool GameOver { get { return _GameOver; } set { _GameOver = value; } }

		private bool _GameOverAniCompleted; // GameOverAnimation Completed Boolean.
		public bool GameOverAniCompleted { get { return _GameOverAniCompleted; } set { _GameOverAniCompleted = value; } }

		private int _GameOverTime; // how long has the game been in a state of game over
		public int GameOverTime { get { return _GameOverTime; } set { _GameOverTime = value; } }

		private int _GameOverStoryTime; // how long has the game over story been displayed
		public int GameOverStoryTime { get { return _GameOverStoryTime; } set { _GameOverStoryTime = value; } }



		// booleans to denote whether a graphic should be displayed or whether an object should be used/updated.
		private bool _showFirstDay;
		public bool ShowFirstDay { get { return _showFirstDay; } set { _showFirstDay = value; } }

		private bool _showFinalDay;
		public bool ShowFinalDay { get { return _showFinalDay; } set { _showFinalDay = value; } }

		private bool _FinalDayShown;
		public bool FinalDayShown { get { return _FinalDayShown; } set { _FinalDayShown = value; } }

		private int _GraphicShowTime;
		public int GraphicShowTime { get { return _GraphicShowTime; } set { _GraphicShowTime = value; } }


		private bool _ShowTitleScreen;
		public bool ShowTitleScreen { get { return _ShowTitleScreen; } set { _ShowTitleScreen = value; } }

		private bool _showHUD;
		public bool ShowHUD { get { return _showHUD; } set { _showHUD = value; } }



		private bool _AllowSectionChangeReset; // Allow section changes
		public bool AllowSectionChangeReset { get { return _AllowSectionChangeReset; } set { _AllowSectionChangeReset = value; } }


		private bool _EnergyBarrierStatus; // Whether the energy barrier is enabled
		public bool EnergyBarrierStatus { get { return _EnergyBarrierStatus; } set { _EnergyBarrierStatus = value; } }


		// Should objects' graphcis been shown and update methods called; effects other variables and methods.
		private bool _showPlayer;
		public bool ShowPlayer { get { return _showPlayer; } set { _showPlayer = value; } }

		private bool _showEnemies;
		public bool ShowEnemies { get { return _showEnemies; } set { _showEnemies = value; } }

		private bool _newEnemyList;
		public bool NewEnemyList { get { return _newEnemyList; } set { _newEnemyList = value; } }

		private bool _eraseList;
		public bool EraseList { get { return _eraseList; } set { _eraseList = value; } }

		private bool _EntityKilled;
		public bool EntityKilled { get { return _EntityKilled; } set { _EntityKilled = value; } }


		private Vector2 _KilledLocation; // where the entity was killed
		public Vector2 KilledLocation { get { return _KilledLocation; } set { _KilledLocation = value; } }

		private int _KillAnimationTimer; // how long has kill animation been displayed
		public int KillAnimationTimer { get { return _KillAnimationTimer; } set { _KillAnimationTimer = value; } }



		private bool _showItems;
		public bool ShowItems { get { return _showItems; } set { _showItems = value; } }

		private bool _newItemList;
		public bool NewItemList { get { return _newItemList; } set { _newItemList = value; } }

		private bool _eraseItemList;
		public bool EraseItemList { get { return _eraseItemList; } set { _eraseItemList = value; } }


		private Point _currentFrame; // frame within game sprite sheets
		public Point CurrentFrame { get { return _currentFrame; } set { _currentFrame = value; } }

		private double _elapsedTime; // elapsed game time as double
		public double ElapsedTime { get { return _elapsedTime; } set { _elapsedTime = value; } }


		/// LoadContent
		/// Load content data into Texture variables.
		public void LoadContent(ContentManager Content, Player mainPlayer) 
		{
			try
			{
				gameErrorTexture = Content.Load<Texture2D>("GameError");
			}
			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				Console.WriteLine("[WARNING] A game error has occurred! The GameError Texture Could Not Be Loaded: GE0001TEX.");
				_gameError = true;
				_gameErrorCode = "GE0001TEX";
			}

			SplashScreen = Content.Load<Texture2D>("SplashScreen");
			ControlsScreen = Content.Load<Texture2D>("Controls");

			// TITLE SCREENS
			TitleScreen = Content.Load<Texture2D>("Title_Screen_With_Text"); // TITLE SCREEN
			TitleScreenNT = Content.Load<Texture2D>("Title_Screen_No_Text"); // TITLE SCREEN NO TEXT

			EnergyBarrierTexture = Content.Load<Texture2D>("EnergyBarrier");

			// ENTITY KILLED GRAPHIC EFFECT LOADED
			EntityKilledTexture = Content.Load<Texture2D>("EntityKilled");

			DungeonCompleted = Content.Load<Texture2D>("DungeonCompleted");

			if (_GameOver == true)
			{
				GameOverStory = Content.Load<Texture2D>("GameoverStory");
			}

			if (GameComplete == true) 
			{ 
				// If the game has been completed the graphic and rank displayed will depend on the players coin count.
				if (mainPlayer.CoinCount > 185)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankA");
				}
				if (mainPlayer.CoinCount > 165 && mainPlayer.CoinCount < 185)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankB");
				}
				if (mainPlayer.CoinCount > 120 && mainPlayer.CoinCount < 165)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankC");
				}
				if (mainPlayer.CoinCount > 80 && mainPlayer.CoinCount < 120)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankD");
				}
				if (mainPlayer.CoinCount > 50 && mainPlayer.CoinCount < 80)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankE");
				}
				if (mainPlayer.CoinCount < 50)
				{
					gameCompleteTexture = Content.Load<Texture2D>("GameCompletedRankN");
				}
			}

			// DAY INDICATION SCREEN
			if (_showFirstDay == true || _showFinalDay == true)
			{
				FirstDayGraphic = Content.Load<Texture2D>("First_Day");
				FinalDayGraphic = Content.Load<Texture2D>("Final_Day");
			}
		}

		/// Update
		public void update(GameTime gameTime, Player mainPlayer, HUD mainHUD, AreaInterface areaInt, LoadGame mainLG, GamePadState state) 
		{ 
			// REGISTER BUTTON PRESS RESETS, ALLOWS ONLY ONE REGISTER OF BUTTON PRESS FOR START AND SELECT
			if ((state.Buttons.RightStick != ButtonState.Pressed && Keyboard.GetState().IsKeyUp(Keys.Enter) == true) && _registerStartPress == false) // once start or enter pressed, close title screen.
			{
				_registerStartPress = true;
			}

			if ((state.Buttons.A != ButtonState.Pressed && Keyboard.GetState().IsKeyUp(Keys.B) == true) && _ShowTitleScreen == false && _registerBPress == false) // once start or enter pressed, close title screen.
			{
				_registerBPress = true;
			}

			if ((state.Buttons.LeftStick != ButtonState.Pressed && Keyboard.GetState().IsKeyUp(Keys.S) == true) && _ShowTitleScreen == false && _registerSelectPress == false) // once start or enter pressed, close title screen.
			{
				_registerSelectPress = true;
			}

			// GAMEOVER - The a player killed animation is shown for 3000ms then the gameover screen is displayed for 5000ms.
			if (_GameOver == true)
			{

				if (mainHUD.TotalTime == 0)
				{
					mainLG.DeleteGameFile(_SaveFileNumber); // IF the player runs out of time, DELETE the game save file.
				}

				_GameOverTime += gameTime.ElapsedGameTime.Milliseconds;

				_EntityKilled = true;
				_KilledLocation = mainPlayer.EntityPos;
				_showPlayer = false;

				if (_GameOverTime >= 3000 && GameOverStoryTime < 9000)
				{
					_GameOverStoryTime += gameTime.ElapsedGameTime.Milliseconds;

					_EntityKilled = false;
					_GameOverAniCompleted = true;
				}

				if (GameOverStoryTime >= 9000)
				{
					_init = true; // restart the game
				}
			}


			if (_gameComplete == true)
			{
				// IF the game has been completed, the player is shown information, if they press enter or push start they will be able to start again.
				if ((state.Buttons.RightStick == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.Enter) == true))
				{
					mainLG.DeleteGameFile(_SaveFileNumber);
					_init = true;
				}
			}


			// TITLE SCREEN
			if (ShowTitleScreen == true)
			{
				_SplashScreenTime += gameTime.ElapsedGameTime.Milliseconds;

				// Controls Screen
				if (_SplashScreenTime > 5000 && _SplashScreenTime < 14000)
				{
					if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) // once start or enter pressed, close title screen.
					{
						// Skip to title screen
						_SplashScreenTime = 14000;
						_registerStartPress = false;
					}
				}

				// Title Screen
				if (_SplashScreenTime > 14000 && RegisterStartPress == true)
				{
					if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) // once start or enter pressed, close title screen.
					{
						// Go to the LoadScreen.
						_ShowTitleScreen = false;
						_showLoadScreen = true;
						_showFirstDay = false;
						_registerStartPress = false;
					}
				}
			}


			// Splash screens showing the current day to the player, they are shown for a limited time.

			// THE FIRST DAY GRAPHIC IS SHOWN
			if (_showFirstDay == true)
			{
				GraphicShowTime += gameTime.ElapsedGameTime.Milliseconds;

				if (GraphicShowTime >= 2000)
				{
					mainHUD.Dawn = (true);
					mainHUD.Dusk = (false);
					_showFirstDay = false;
					_showHUD = true;
					mainPlayer.AllowEntityMovement = true;
					mainPlayer.AllowWeaponFire = true;
					_showPlayer = true;
					_showEnemies = true;
					_showItems = true;
					_eraseList = false;
					GraphicShowTime = 0;
				}
			}


			// THE FINAL DAY GRAPHIC IS SHOWN
			if (_showFinalDay == true)
			{
				GraphicShowTime += gameTime.ElapsedGameTime.Milliseconds;

				mainPlayer.AllowEntityMovement = false;
				mainPlayer.AllowWeaponFire = false;
				mainPlayer.PlayerWeaponFiring = false;
				mainPlayer.NewPlayerWeaponFire = true;
				mainPlayer.DrawWeapon = false;
				_showPlayer = false;

				if (GraphicShowTime >= 2000)
				{
					mainHUD.Dawn = (true);
					mainHUD.Dusk = (false);
					_showFinalDay = false;
					_showHUD = true;
					mainPlayer.AllowEntityMovement = true;
					mainPlayer.AllowWeaponFire = true;
					_showPlayer = true;
					_showEnemies = true;
					_showItems = true;
					_eraseList = false;
					FinalDayShown = true;
					GraphicShowTime = 0;
				}
			}



			// DUNGEON COMPLETED SCREEN shown for a limited time.
			if (mainPlayer.JustCompletedDungeon == true)
			{
				GraphicShowTime += gameTime.ElapsedGameTime.Milliseconds;

				mainPlayer.AllowEntityMovement = false;
				mainPlayer.AllowWeaponFire = false;
				mainPlayer.PlayerWeaponFiring = false;
				mainPlayer.NewPlayerWeaponFire = true;
				mainPlayer.DrawWeapon = false;
				_showPlayer = false;

				if (GraphicShowTime >= 5000)
				{
					mainPlayer.JustCompletedDungeon = false;

					if (mainPlayer.CompletedDungeons[0] == true && mainPlayer.CompletedDungeons[1] == true && mainPlayer.CompletedDungeons[2] == true)
					{
						// If all three dungeons have been completed then the game has been completed.
						_gameComplete = true;
					}
					else 
					{

						_showHUD = true;
						mainPlayer.AllowEntityMovement = true;
						mainPlayer.AllowWeaponFire = true;
						_showPlayer = true;
						_showEnemies = true;
						_showItems = true;
						_eraseList = false;
						FinalDayShown = true;
						GraphicShowTime = 0;
					}
				}
			}


			// ENERGY BARRIERS PREVENTING EXIT FROM DUNGEON SECTION

			if (areaInt.EnemyList != null && _gamePaused == false)
			{
				if (areaInt.EnemyList.Count > 0 && mainPlayer.InDungeon == true)
				{
					// Enable the energy barrier if there are enemies present in a dungeon section.
					_EnergyBarrierStatus = true;
				}
				else { _EnergyBarrierStatus = false; }
			}
			else { _EnergyBarrierStatus = false; }
		}


		/// Draw
		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, SpriteFont font, Player mainPlayer) 
		{
			if (_gameError == true)
			{
				// Display error screen and error code
				spriteBatch.Draw(gameErrorTexture, new Vector2(0, 0));
				spriteBatch.DrawString(font, ("ERROR CODE: " + _gameErrorCode.ToString()), new Vector2(5, 5), Color.White); // current player health levels displayed
			}

			else if (_GameOverAniCompleted == true) 
			{ 
				// Show the game over story when animation compelted
				spriteBatch.Draw(GameOverStory, new Vector2(0, 0));
			}

			else {

				if (EnergyBarrierStatus == true)
				{
					spriteBatch.Draw(EnergyBarrierTexture, new Vector2(0, 96));
				}


				if (_ShowTitleScreen == true)
				{
					// Show first splash screen for 4000ms
					if (_SplashScreenTime >= 0 && _SplashScreenTime <= 4000)
					{
						spriteBatch.Draw(SplashScreen, new Vector2(0, 0));
					}

					// Show controlls screen for 10000ms
					if (_SplashScreenTime > 4000 && _SplashScreenTime <= 14000)
					{
						spriteBatch.Draw(ControlsScreen, new Vector2(0, 0));
					}

					// Title Screen, alternates between two states - text on or text off.
					if (_SplashScreenTime > 14000) 
					{
						_elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

						if (_elapsedTime >= 0)
						{
							spriteBatch.Draw(TitleScreen, new Vector2(0, 0));
						}
						if (_elapsedTime >= 1000)
						{
							spriteBatch.Draw(TitleScreenNT, new Vector2(0, 0));
						}
						if (_elapsedTime >= 2000)
						{
							_elapsedTime = 0;
						}

						spriteBatch.DrawString(font, (""), new Vector2(600, 680), Color.White); // display version number (600,680 original)
					}
				}

				// Show information screens if the title screen is not being shown and other conditions are met.
				else if (_showFirstDay == true && FirstDayGraphic != null && GameOverAniCompleted == false)
				{
					spriteBatch.Draw(FirstDayGraphic, new Vector2(0, 0));
				}

				else if (_showFinalDay == true && FinalDayGraphic != null && GameOverAniCompleted == false)
				{
					spriteBatch.Draw(FinalDayGraphic, new Vector2(0, 0));
				}

				else if (mainPlayer.JustCompletedDungeon == true)
				{
					spriteBatch.Draw(DungeonCompleted, new Vector2(0, 0));
				}


				// If the game has been completed show game complete.
				if (_gameComplete == true && mainPlayer.JustCompletedDungeon == false)
				{
					spriteBatch.Draw(gameCompleteTexture, new Vector2(0, 0));
				}
			}

			if (_EntityKilled == true)
			{
				// Animation for when an entity is killed, in effect an alternating flash of a texture.

				_KillAnimationTimer += gameTime.ElapsedGameTime.Milliseconds;

				if (_KillAnimationTimer >= 0 && _KillAnimationTimer >= 50)
				{
					spriteBatch.Draw(EntityKilledTexture, _KilledLocation);
				}

				if (_KillAnimationTimer >= 100 && _KillAnimationTimer >= 150)
				{
					spriteBatch.Draw(EntityKilledTexture, _KilledLocation);
				}

				if (_KillAnimationTimer >= 150)
				{
					_KillAnimationTimer = 0;
					_EntityKilled = false;
				}
			}
		}
	}
}