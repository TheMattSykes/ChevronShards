using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ChevronShards
{
	public class LoadScreen
	{
		private Texture2D LoadScreenTex;
		private Texture2D ActiveSaveTex;
		private Texture2D NewSave;
		private Texture2D DeleteSave;
		private Texture2D Arrow;

		private Texture2D ChevronShardArea;
		private Texture2D ChevronShard1;
		private Texture2D ChevronShard2;
		private Texture2D ChevronShard3;

		private Vector2 ArrowPos;

		private int LoadOptionNumber;

		private bool DeleteMode;

		private bool[] ActiveSaves = new bool[3];
		private bool[,] CompletedDungeons = new bool[3, 3];


		public LoadScreen()
		{
			LoadOptionNumber = 1; // default option is 1
			ArrowPos = new Vector2(40, 180);
		}


		/// DefaultSettings
		/// If the player starts a new game the variables will be set to the values specified in the function.
		public void DefaultSettings(Player mainPlayer, InformationDisplay mainID, AreaInterface areaInt) 
		{ 
			int STARTX = 2; // inital starting section of overworld is 2,4
			int STARTY = 4;

			int STARTDUNNUM = 1; // set default dungeon number

			mainID.ShowFirstDay = true; // show the first day information screen on loading of game file.
			// Set variables for player object
			mainPlayer.CurrentOWSec = new Vector2(STARTX, STARTY);
			mainPlayer.EntityPos = new Vector2(144, 432); // 144,432
			mainPlayer.CurrentDungeonNumber = STARTDUNNUM;
			mainPlayer.CurrentBLNumber = STARTDUNNUM;

			areaInt.GenerateStructure(STARTX, STARTY);
			areaInt.GenerateRectangleCollisions();
		}

		/// RegularSettings
		/// If the player reloads a saved game the following variables will ve set to the speicfied values.
		public void RegularSettings(Player mainPlayer, InformationDisplay mainID) 
		{ 
			mainID.ShowFirstDay = false;
			mainID.ShowHUD = true;
			mainPlayer.AllowEntityMovement = true;
			mainPlayer.AllowWeaponFire = true;
			mainID.ShowPlayer = true;
			mainID.ShowEnemies = true;
			mainID.ShowItems = true;
			mainID.EraseList = false;
			mainID.GraphicShowTime = 0;
		}


		/// LoadContent
		/// Load content data into Texture variables.
		public void LoadContent(ContentManager Content)
		{ 
			LoadScreenTex = Content.Load<Texture2D>("LoadScreen");
			ActiveSaveTex = Content.Load<Texture2D>("ActiveSave");
			NewSave = Content.Load<Texture2D>("NewSave");
			DeleteSave = Content.Load<Texture2D>("DeleteSave");
			Arrow = Content.Load<Texture2D>("Arrow");

			ChevronShardArea = Content.Load<Texture2D>("ShardsArea");
			ChevronShard1 = Content.Load<Texture2D>("ChevronShard1");
			ChevronShard2 = Content.Load<Texture2D>("ChevronShard2");
			ChevronShard3 = Content.Load<Texture2D>("ChevronShard3");
		}


		/// Update
		public void update(GamePadState state, InformationDisplay mainID, LoadGame mainLG, Player mainPlayer, HUD mainHUD, AreaInterface areaInt) 
		{

			mainLG.INIT(ref ActiveSaves, ref CompletedDungeons); // initialise

			if (mainID.RegisterSelectPress == true)
			{
				if (DeleteMode == true)
				{
					// DELETE MODE

					// If conditions: so that program only registers one press of each specified button at a time.
					if (state.Buttons.LeftStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S) == true)
					{
						mainID.RegisterSelectPress = false;
					}

					if (mainID.RegisterBPress == true) // confirm the player wants to delete the save file
					{
						if (state.Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.B) == true) // player chooses not to delete
						{
							DeleteMode = false; // exit delete mode
							mainID.RegisterBPress = false;
						}
					}

					// Player chooses to delete file
					if (mainID.RegisterStartPress == true) // if start button can be pressed
					{
						if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
						{
							mainLG.DeleteGameFile(LoadOptionNumber); // Delete file
							DeleteMode = false; // exit delete mode
							mainID.RegisterStartPress = false;
						}
					}
				}

				if (DeleteMode == false) // If not in delete mode
				{ 
					// SWITCHING OPTIONS
					if (state.Buttons.LeftStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S) == true)
					{

						LoadOptionNumber += 1;

						if (LoadOptionNumber >= 4) { LoadOptionNumber = 1; } // only allow upto 4 options

						// Move the position of the arrow graphic in accordance to the option number
						if (LoadOptionNumber == 1)
						{
							ArrowPos = new Vector2(40, 180);
						}

						if (LoadOptionNumber == 2)
						{
							ArrowPos = new Vector2(40, 340);
						}

						if (LoadOptionNumber == 3)
						{
							ArrowPos = new Vector2(40, 500);
						}

						mainID.RegisterSelectPress = false;
					}

					// CHECK TO SEE IF ENTERING DELETE MODE
					if (mainID.RegisterBPress == true)
					{
						if (state.Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.B) == true)
						{
							if (ActiveSaves[LoadOptionNumber - 1] == true) // -1 as option number minimum is 1 compared with Active saves which starts with 0.
							{
								DeleteMode = true; // enter delete mode
							}
							mainID.RegisterBPress = false;
						}
					}

					// LOAD GAME FILE AND SETUP, LoadOptionNumber 1 corresponds with file 0.
					if (mainID.RegisterStartPress == true)
					{
						if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
						{
							if (ActiveSaves[LoadOptionNumber-1] == true) // -1 as option number minimum is 1 compared with Active saves which starts with 0.
							{
								mainLG.LoadGameFile(mainID, mainPlayer, mainHUD, LoadOptionNumber);

								areaInt.GenerateStructure((int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y);
								areaInt.GenerateRectangleCollisions();
								mainPlayer.PlayerGenCoordinate(areaInt);

								RegularSettings(mainPlayer, mainID);
							}
							else
							{
								DefaultSettings(mainPlayer, mainID, areaInt);
							}

							mainID.ShowPlayer = true;
							mainID.SaveFileNumber = LoadOptionNumber;

							mainID.ShowLoadScreen = false;

							mainID.RegisterStartPress = false;
						}
					}
				}
			}
		}

		/// DRAW
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font) 
		{
			spriteBatch.Draw(LoadScreenTex, new Vector2(0,0)); // Draw the main loadscreen background
			spriteBatch.Draw(Arrow, ArrowPos); // Draw arrow


			if (ActiveSaves[0] == true) // check to see if save file is active
			{
				if (DeleteMode == true && LoadOptionNumber == 1) // show delete save information
				{
					spriteBatch.Draw(DeleteSave, new Vector2(105, 144));
				}
				else 
				{
					// Draw save file UI and information onto LoadScreen.
					spriteBatch.Draw(ActiveSaveTex, new Vector2(105, 144));
					spriteBatch.DrawString(font, "SAVE FILE 1", new Vector2(150, 184), Color.White);
					spriteBatch.Draw(ChevronShardArea, new Vector2(320, 161));

					// Show which Chevron Shards the player has currently collected.
					if (CompletedDungeons[0, 0] == true)
					{
						spriteBatch.Draw(ChevronShard1, new Vector2(340, 180));
					}
					if (CompletedDungeons[0, 1] == true)
					{
						spriteBatch.Draw(ChevronShard2, new Vector2(410, 180));
					}
					if (CompletedDungeons[0, 2] == true)
					{
						spriteBatch.Draw(ChevronShard3, new Vector2(480, 180));
					}
				}
			}
			else
			{
				spriteBatch.Draw(NewSave, new Vector2(105, 144)); // If save file is not active then show NewSave texture.
			}

			// Comments follow for the rest of saves, same as for the first save file.
			if (ActiveSaves[1] == true)
			{
				if (DeleteMode == true && LoadOptionNumber == 2)
				{
					spriteBatch.Draw(DeleteSave, new Vector2(105, 304));
				}
				else
				{
					spriteBatch.Draw(ActiveSaveTex, new Vector2(105, 304));
					spriteBatch.DrawString(font, "SAVE FILE 2", new Vector2(150, 344), Color.White);
					spriteBatch.Draw(ChevronShardArea, new Vector2(320, 321));

					if (CompletedDungeons[1, 0] == true)
					{
						spriteBatch.Draw(ChevronShard1, new Vector2(340, 340));
					}
					if (CompletedDungeons[1, 1] == true)
					{
						spriteBatch.Draw(ChevronShard2, new Vector2(410, 340));
					}
					if (CompletedDungeons[1, 2] == true)
					{
						spriteBatch.Draw(ChevronShard3, new Vector2(480, 340));

					}
				}
			}
			else
			{
				spriteBatch.Draw(NewSave, new Vector2(105, 304));
			}


			if (ActiveSaves[2] == true)
			{
				if (DeleteMode == true && LoadOptionNumber == 3)
				{
					spriteBatch.Draw(DeleteSave, new Vector2(105, 464));
				}
				else
				{
					spriteBatch.Draw(ActiveSaveTex, new Vector2(105, 464));
					spriteBatch.DrawString(font, "SAVE FILE 3", new Vector2(150, 504), Color.White);
					spriteBatch.Draw(ChevronShardArea, new Vector2(320, 481));

					if (CompletedDungeons[2, 0] == true)
					{
						spriteBatch.Draw(ChevronShard1, new Vector2(340, 500));
					}
					if (CompletedDungeons[2, 1] == true)
					{
						spriteBatch.Draw(ChevronShard2, new Vector2(410, 500));
					}
					if (CompletedDungeons[2, 2] == true)
					{
						spriteBatch.Draw(ChevronShard3, new Vector2(480, 500));
					}
				}
			}
			else
			{
				spriteBatch.Draw(NewSave, new Vector2(105, 464));
			}
		}
	}
}