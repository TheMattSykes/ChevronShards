using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


/*
 * CHEVRON SHARDS
 * VERSION 0.16.1
 * 
 * A GENERIC USB NES CONTROLLER CAN BE USED WITH THIS GAME:
 * RIGHTSTICK IS THE START BUTTON, LEFTSTICK IS THE SELECT BUTTON,
 * LEFTTHUMBSTICK for DPAD, B IS A BUTTON AND A IS B BUTTON.
 * 
 * © Matthew Sykes 2017
 */


namespace ChevronShards
{
	public class Game1 : Game
	{

		// Declaration of variables
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private GamePadState state;

		private SpriteFont font;
		private SpriteFont TitleFont;

		private bool FalseBool;


		// Declare classes
		private LoadGame mainLG;

		private InformationDisplay mainID;
		private LoadScreen mainLS; // LS for Load Screen
		private PauseScreen mainPS;

		private HUD mainHUD;

		private Player mainPlayer;

		private AreaInterface areaInt;
		private OverworldManager mainOWM; // OWM for OverWorldManager
		private DungeonManager mainDM;
		private BossLevelManager mainBLM;

		private EnemyManager mainEM;
		private ItemManager mainIM;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}


		/// Initialize method:
		/// The following method will instantiate objects used throughout the program and set up default values for variables.

		protected override void Initialize()
		{
			try
			{
				mainID = new InformationDisplay();
				mainID.GameError = false;
			}
			catch 
			{ 
				Console.WriteLine("[WARNING] A game error has occurred! " + "GE0000MID");
			}

			try
			{
				Window.Title = "Chevron Shards";

				// WINDOW SIZE
				graphics.PreferredBackBufferWidth = 768;
				graphics.PreferredBackBufferHeight = 720;
				graphics.ApplyChanges();
			}
			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				mainID.GameErrorCode = "GE0006WIN";
				Console.WriteLine("[WARNING] A game error has occurred! " + mainID.GameErrorCode);
				mainID.GameError= true;
			}

			try{
				Console.WriteLine("Game is Initialising...");

				// TITLE SCREEN
				mainID.ShowTitleScreen = true;
				mainID.ShowLoadScreen = false;
				mainID.ShowHUD = false;

				//mainLG = new LoadGame();
				mainLG = new LoadGame();
				mainPS = new PauseScreen();
				mainLS = new LoadScreen();
				mainHUD = new HUD();
				mainPlayer = new Player();
				mainOWM = new OverworldManager(); // OWM stands for OverWorldManager
				mainDM = new DungeonManager(); // DM stands for DungeonManager
				mainBLM = new BossLevelManager(); // BLM stands for BossLevelManager
				mainEM = new EnemyManager(); // EM stands for EnemyManager
				mainIM = new ItemManager(); // IM stands for ItemManager

				mainID.SplashScreenTime = 0;

				mainHUD.Initialise();

				mainPlayer.InOverworld = true; // set player location to overworld
				mainPlayer.InDungeon = false;
				mainPlayer.InBossLevel = false;

				InterfaceUpdate();

				areaInt.ChangeSec = false;

				mainID.GamePaused = false;

				mainPlayer.AllowEntityMovement = false;
				mainPlayer.AllowWeaponFire = false;
				mainPlayer.PlayerWeaponFiring = false;
				mainPlayer.NewPlayerWeaponFire = true;
				mainPlayer.DrawWeapon = false;

				mainPlayer.AllowEntityDirChange = true;
				mainPlayer.Orientation = 'R';
				mainPlayer.CurrentWeapon = "Sword";
				mainPlayer.ChangeWeapon();

				mainID.ShowPlayer = false;
				mainID.NewEnemyList = true;

				mainID.ShowItems = false;
				mainID.NewItemList = true;
				mainID.EraseItemList = false;

				mainID.EnergyBarrierStatus = false;

				mainID.GameOver = false;
				mainID.GameOverAniCompleted = false;
				mainID.GameOverTime = 0;
				mainID.GameOverStoryTime = 0;


				mainID.ElapsedTime = 0f;

				mainID.CurrentFrame = new Point(0, 0);

				mainID.EntityKilled = false;
				mainID.KillAnimationTimer = 0;

			}
			catch
			{
				mainID.GameErrorCode = "GE0007INI";
				Console.WriteLine("[WARNING] A game error has occurred! " + mainID.GameErrorCode);
				mainID.GameError= true;
			}

			base.Initialize(); // THIS MUST BE LAST 

		}



		/// LoadContent Method:
		/// For loading textures into the game and calling the LoadContent methods of the other classes.

		protected override void LoadContent() // loads textures and sprites
		{

			try
			{
				// SPRITEBATCH
				spriteBatch = new SpriteBatch(GraphicsDevice);
			}
			catch
			{
				Console.WriteLine("[WARNING] A game error has occurred: GE0000SPB.");
				mainID.GameError= true;
				mainID.GameErrorCode = "GE0000SPB"; 
			}


			try
			{ // ensure that content loads successfully, otherwise there is a game error.
				if (mainID.ShowLoadScreen  == false)
				{
					mainLS.LoadContent(this.Content);
				}

				mainID.LoadContent(this.Content, mainPlayer);

				mainPS.LoadContent(this.Content);


				mainHUD.LoadContent(this.Content, mainPlayer);


				InterfaceUpdate();

				if (mainID.ShowPlayer == true)
				{
					areaInt.LoadContent(this.Content);
				}

				mainPlayer.LoadContent(this.Content);

			}
			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				Console.WriteLine("[WARNING] A game error has occured: GE0002TEX");
				mainID.GameError = true;
				mainID.GameErrorCode = "GE0002TEX";
			}


			try { 
				// ENEMY GRAPHICS AND ENEMY WEAPON GRAPHICS
				if (areaInt.EnemyList != null)
				{
					if (mainID.ShowEnemies == true)
					{
						mainEM.LoadContent(this.Content, areaInt);
					}
				}
			}

			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				Console.WriteLine("[WARNING] A game error has occurred: GE0003TEX");
				mainID.GameError= true;
				mainID.GameErrorCode = "GE0003TEX";
			}



			try
			{
				// ITEM GRAPHICS
				if (areaInt.ItemList != null && mainID.ShowItems == true)
				{
					mainIM.LoadContent(this.Content, areaInt);
				}


			}

			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				Console.WriteLine("[WARNING] A game error has occurred!");
				mainID.GameError= true;
				mainID.GameErrorCode = "GE0004TEX";
			}



			try
			{
				// MAIN FONTS
				font = Content.Load<SpriteFont>("mainFont");
				TitleFont = Content.Load<SpriteFont>("TitleFont");
			}
			catch
			{
				// IF AN ERROR OCCURS WHILE LOADING A TEXTURE
				Console.WriteLine("[WARNING] A game error has occurred!");
				mainID.GameError= true;
				mainID.GameErrorCode = "GE0005FON";

			}
		}



		/// InterfaceUpdate Method:
		/// This method is used for setting an Interface of AreaInterface type to act as the object of the current area.

		public void InterfaceUpdate() 
		{ 
			if (mainPlayer.InOverworld == true)
			{
				areaInt = mainOWM as AreaInterface;
			}
			if (mainPlayer.InDungeon == true)
			{
				areaInt = mainDM as AreaInterface;
			}
			if (mainPlayer.InBossLevel == true)
			{
				areaInt = mainBLM as AreaInterface;
			}
		}







		/// Update Method: 
		/// The main game logic is located in this method or the update methods in the classes called in this method.
		/// The majority of user input is registered i.e. button/key pressing is registered here.

		protected override void Update(GameTime gameTime)
		{
			state = GamePad.GetState(PlayerIndex.One); // creates a state for gamepads


			// EXIT GAME VIA ESCAPE KEY
			if (state.Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit(); // the game closes
			}



			InterfaceUpdate();


			mainID.update(gameTime, mainPlayer, mainHUD, areaInt, mainLG, state);

			if (mainID.Init == true) 
			{
				Initialize(); // reinitialise game
			}


			// PAUSE SCREEN
			if (mainID.GameError== false && mainID.RegisterStartPress == true && mainID.ShowTitleScreen == false && mainID.GameOver == false && mainID.GameComplete == false)
			{
				bool ExitGame = false;

				mainPS.update(state, mainID, mainLG, mainPlayer, mainHUD, ref ExitGame);

				if (ExitGame == true)
				{
					Exit(); // close game
				}
			}
			//


			if (mainID.GameError == false && mainID.GamePaused == false && mainID.GameComplete == false)
			{


				if (mainID.GameOver == false)
				{

					if (mainID.ShowLoadScreen == true) 
					{
						mainLS.update(state, mainID, mainLG, mainPlayer, mainHUD, areaInt);
					}


					// IN GAME CLOCK SYSTEM - Prints an in game clock on the HUD.
					if (mainID.ShowFirstDay == false && mainID.ShowTitleScreen == false && mainID.ShowFinalDay == false && mainID.ShowLoadScreen == false && mainID.GameOver == false)
					{
						mainHUD.update(gameTime, mainPlayer, mainID);
					}



					if ((areaInt.ChangeSec == true) && mainID.AllowSectionChangeReset == true)
					{ 
						// Generate structure for area i.e. read text file and load to array, then draw
						if (mainPlayer.InOverworld == true)
						{
							mainOWM.GenerateStructure((int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y);
							mainOWM.GenerateRectangleCollisions();
						}

						if (mainPlayer.InDungeon == true)
						{
							mainDM.GenerateStructure(mainPlayer.CurrentDungeonNumber, (int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y);
							mainPlayer.CurrentBLNumber = mainPlayer.CurrentDungeonNumber;
							mainDM.GenerateRectangleCollisions();
						}

						if (mainPlayer.InBossLevel == true)
						{
							mainBLM.SetBossLevelNumber(mainPlayer.CurrentBLNumber);
							mainBLM.GenerateStructure();
							mainDM.GenerateRectangleCollisions();
						}

						if (areaInt.EnemyList != null)
						{
							areaInt.EnemyList.Clear(); // clear the area's enemy list
						}

						if (areaInt.ItemList != null)
						{
							areaInt.ItemList.Clear(); // clear the area's item list
							mainID.NewItemList = true;
						}

						mainID.AllowSectionChangeReset = false;
					}


					if (areaInt.ChangeSec == false) { mainID.AllowSectionChangeReset = true; } // reset change section



					areaInt.update();




					// ENEMY, WEAPON AND ITEM UPDATE AND LIST GENERATION
					if (areaInt.ChangeSec == false && mainID.GameOver == false)
					{
						Rectangle WeaponRectangle = new Rectangle(0, 0, 0, 0); // default rectangle

						mainPlayer.PlayerWeapon.SetRectangle();
						WeaponRectangle = mainPlayer.PlayerWeapon.GetWeaponRect(); // sets the weapon rectangle to the dimentions and position of the sword.



						FalseBool = false; // a bool that will be set to false everytime update is called.


						if (mainPlayer.HasChangedArea == true) 
						{
							mainID.NewEnemyList = true;
							mainID.EraseList = true;
							mainPlayer.HasChangedArea = false;
						}


						areaInt.GenerateEnemies(mainPlayer.Rect(), mainID);

						areaInt.GenerateItems(mainPlayer, areaInt, mainID);

						if (areaInt.EnemyList != null) // check to see if enemy list is empty
						{
							if (areaInt.EnemyList.Count > 0)
							{
								mainEM.Update(gameTime, areaInt, mainPlayer, mainID); // Update the enemy manager
							}
						}
					}
				}





				// PLAYER UPDATE AND MOVEMENT

				if (mainPlayer.AllowEntityMovement == true && areaInt.ChangeSec == false)
				{
					bool trueBool = true;

					mainPlayer.Update(gameTime, state, areaInt, mainID);

					// If the player changes area type, then reaload the interface, content and enemies
					if (mainPlayer.InOverworld == true && mainPlayer.HasChangedArea == true)
					{
						mainOWM.GenerateStructure((int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y);
						mainOWM.GenerateEnemies(mainPlayer.Rect(), mainID);
						areaInt.GenerateRectangleCollisions();
					}

					if (mainPlayer.InDungeon == true && mainPlayer.HasChangedArea == true)
					{
						InterfaceUpdate();
						areaInt.GenerateStructure(mainPlayer.CurrentDungeonNumber, 2, 3);
						areaInt.GenerateRectangleCollisions();
						mainPlayer.CurrentBLNumber = mainPlayer.CurrentDungeonNumber;
						areaInt.GenerateEnemies(mainPlayer.Rect(), mainID);
					}

					if (mainPlayer.InBossLevel == true && mainPlayer.HasChangedArea == true)
					{
						InterfaceUpdate();
						areaInt.SetBossLevelNumber(mainPlayer.CurrentBLNumber);
						areaInt.GenerateStructure();
						areaInt.GenerateRectangleCollisions();
						areaInt.GenerateEnemies(mainPlayer.Rect(), mainID);
					}
					// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
				}


				// ITEM UPDATE
				if (areaInt.ChangeSec == false)
				{
					mainIM.update(areaInt, mainPlayer);
				}
			}

			base.Update(gameTime);
		}






		// ------------------------------------- DRAW -------------------------------------

		protected override void Draw(GameTime gameTime)
		{
			InterfaceUpdate(); // change AreaInterface to current area type

			LoadContent(); // reload content into Texture variabless

            spriteBatch.Begin();


			var fps = Math.Round((1 / gameTime.ElapsedGameTime.TotalSeconds),4);

            if (fps < 59.9)
            {
                Console.WriteLine("[Notice] FPS Drop: " + fps.ToString()); // print frames per second to console
            }


			if (mainID.GameError== false && mainID.GamePaused == true && mainID.GameComplete == false)
			{
				mainPS.Draw(spriteBatch, mainPlayer); // update the pause screen
			}

			if (mainID.GameError== false && mainID.GamePaused == false && mainID.GameComplete == false)
			{

				if (mainID.ShowTitleScreen == false && mainID.GameOverAniCompleted == false)
				{ // IF THE TITLE SCREEN IS CURRENTLY NOT SHOWING.

					if (mainID.ShowLoadScreen == true)
					{
						mainLS.Draw(gameTime, spriteBatch, font);
					}
					else {

						/// Drawing the overworld and dungeons

						// DECIDE WHICH PART OF OVERWORLD TO DRAW.

						bool InOverworld = mainPlayer.InOverworld;
						bool InDungeon = mainPlayer.InDungeon;
						bool InBossLevel = mainPlayer.InBossLevel;


						if (mainID.ShowPlayer == true || mainID.ShowEnemies == true || mainID.ShowItems == true)
						{
							areaInt.Draw(spriteBatch, mainPlayer, mainID);
						}

						if (mainID.ShowItems == true && areaInt.ChangeSec == false)
						{
							if (areaInt.ItemList != null)
							{
								mainIM.Draw(spriteBatch, areaInt);
							}
						}

						if (mainID.ShowEnemies == true && areaInt.ChangeSec == false)
						{
							if (areaInt.EnemyList != null)
							{
								mainEM.Draw(gameTime, spriteBatch, areaInt);
							}
						}

						if (mainID.ShowPlayer == true && areaInt.ChangeSec == false) // due to the order of drawing, mainID.ShowPlayer is used twice
						{ // draw the player to the screen
							mainPlayer.Draw(gameTime, spriteBatch);
						}


						// DRAW THE GAMES HEADS UP DISPLAY
						if (mainID.ShowHUD == true)
						{
							mainHUD.Draw(spriteBatch, mainPlayer, mainID, font);
						}
					}
				}
			}


			mainID.Draw(spriteBatch, gameTime, font, mainPlayer);


			base.Draw(gameTime);
			spriteBatch.End();
		}
	}
}
