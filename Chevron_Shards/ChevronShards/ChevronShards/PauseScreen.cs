using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	class PauseScreen
	{
		// Declare Texture variable names
		private Texture2D ChevronShard1;
		private Texture2D ChevronShard2;
		private Texture2D ChevronShard3;

		private Texture2D PauseScreenTexture;
		private Texture2D Arrow; // Arrow indicates which option the user has selected
		private Vector2 ArrowPos;
		private int OptionNumber;


		// Load textures into Texture variables
		public void LoadContent(ContentManager Content) 
		{ 
			PauseScreenTexture = Content.Load<Texture2D>("PauseScreen");
			Arrow = Content.Load<Texture2D>("Arrow");

			ChevronShard1 = Content.Load<Texture2D>("ChevronShard1");
			ChevronShard2 = Content.Load<Texture2D>("ChevronShard2");
			ChevronShard3 = Content.Load<Texture2D>("ChevronShard3");
		}


		public void update(GamePadState state, InformationDisplay mainID, LoadGame mainLG, Player mainPlayer, HUD mainHUD, ref bool ExitGame) 
		{
			if (mainID.GamePaused == false && mainID.ShowPlayer == true) // If the game is not paused allow the game to be paused as long as the player graphic is currently shown
			{
				if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) == true) // once start or enter pressed, close title screen.
				{
					mainID.GamePaused = true; // pause game
					mainID.RegisterStartPress = false;
					mainID.RegisterSelectPress = false;
					OptionNumber = 1; // default option number to 1
					ArrowPos = new Vector2(250, 315); // default arrow position
				}
			}

			if (mainID.GamePaused == true)
			{
				if (mainID.RegisterSelectPress == true)
				{
					if (state.Buttons.LeftStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S) == true)
					{

						OptionNumber += 1; // Increment option number by 1 each time start or enter pressed

						if (OptionNumber == 4) { OptionNumber = 1; } // options only go upto 3 so reset to 1

						if (OptionNumber == 1)
						{
							ArrowPos = new Vector2(250, 315); // change arrow position when option changed
						}

						if (OptionNumber == 2)
						{
							ArrowPos = new Vector2(250, 392);
						}

						if (OptionNumber == 3)
						{
							ArrowPos = new Vector2(250, 469);
						}

						mainID.RegisterSelectPress = false; // Only allow one option to be changed per press of button
					}
				}

				if (mainID.RegisterStartPress == true)
				{
					if (state.Buttons.RightStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) == true) // once start or enter pressed, close title screen.
					{
						if (OptionNumber == 1)
						{
							mainID.GamePaused = false; // resume the game
						}

						if (OptionNumber == 2)
						{
							mainLG.SaveGameFile(mainPlayer,mainHUD,mainID); // Save the game file selected when started
						}

						if (OptionNumber == 3)
						{
							ExitGame = true; // Close the game
						}

						mainID.RegisterStartPress = false;
					}
				}
			}
		}


		public void Draw(SpriteBatch spriteBatch, Player mainPlayer) 
		{ 
			spriteBatch.Draw(PauseScreenTexture, new Vector2(0, 96));
			spriteBatch.Draw(Arrow, ArrowPos);

			// Draw the chevron shards currently collected on the pause screen, denoted by dungeons completed
			if ((mainPlayer.CompletedDungeons)[0] == true)
			{
				spriteBatch.Draw(ChevronShard1, new Vector2(238, 152));
			}

			if ((mainPlayer.CompletedDungeons)[1] == true)
			{
				spriteBatch.Draw(ChevronShard2, new Vector2(318, 152));
			}

			if ((mainPlayer.CompletedDungeons)[2] == true)
			{
				spriteBatch.Draw(ChevronShard3, new Vector2(398, 152));
			}
		}
	}
}