using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	public class HUD // Stands for Heads Up Display
	{
		private Texture2D HUDBack;
		private Texture2D HeartIcon;
		private Texture2D HUD_A_Button;
		private Texture2D HUD_B_Button;
		private Texture2D HUD_ItemHolder;
		private Texture2D Coin_Icon;
		private string _CoinAmountString;
		private bool _showHUD;

		// Textures and booleans for whether time is Dawn or Dusk.
		private Texture2D DawnGraphic;
		private bool _Dawn;
		public bool Dawn { get { return _Dawn; } set { _Dawn = value; } }

		private Texture2D DuskGraphic;
		private bool _Dusk;
		public bool Dusk { get { return _Dusk; } set { _Dusk = value; } }

		private Texture2D DuskChanger;

		// String representations of times.
		private string _CurrentTime;

		private string _CurrentHourStr;
		private string _CurrentMinStr;


		private int _CurrentHour;
		public int CurrentHour{ get { return _CurrentHour; } set { _CurrentHour = value; }}

		private int _CurrentMin;
		public int CurrentMin { get { return _CurrentMin; } set { _CurrentMin = value; } }


		private int _CurrentTimeAdder;

		private int _TotalTime;
		public int TotalTime { get { return _TotalTime; } set { _TotalTime = value; } }

		private int _TotalTimeAdder;
		private int _FinalCountdown;
		private bool _FinalCountdownBool;

		/// Initialise
		/// Set default values when game is setup or restarted
		public void Initialise() { 

			_FinalCountdownBool = false;
			_TotalTime = 48;
			_TotalTimeAdder = 0;
			_CurrentHour = 8;
			_CurrentMin = 0;
			_CurrentTimeAdder = 0;
			_FinalCountdown = 60;
		}

		/// LoadContent
		/// Load content data into Texture variables.
		public void LoadContent(ContentManager content, Player mainPlayer)
		{
			// HUD ICONS AND GRAPHICS
			HUDBack = content.Load<Texture2D>("Bar");
			HeartIcon = content.Load<Texture2D>("Heart");
			HUD_A_Button = content.Load<Texture2D>("HUD_A_Button_Blank");

			if (mainPlayer.CurrentWeapon == "Seed")
			{
				HUD_B_Button = content.Load<Texture2D>("HUD_B_Button_Seed");
			}
			if (mainPlayer.CurrentWeapon == "FireBall")
			{
				HUD_B_Button = content.Load<Texture2D>("HUD_B_Button_FireBall");
			}
			if (mainPlayer.CurrentWeapon == "WaterBall")
			{
				HUD_B_Button = content.Load<Texture2D>("HUD_B_Button_WaterBall");
			}
			if (mainPlayer.CurrentWeapon == "Sword")
			{
				HUD_B_Button = content.Load<Texture2D>("HUD_B_Button_Sword");
			}

			HUD_ItemHolder = content.Load<Texture2D>("HUD_ItemsHolder");
			Coin_Icon = content.Load<Texture2D>("Coin_Icon");

			if (_Dawn == true)
			{
				DawnGraphic = content.Load<Texture2D>("Dawn");
			}

			if (_Dusk == true)
			{
				DuskGraphic = content.Load<Texture2D>("Dusk");
				DuskChanger = content.Load<Texture2D>("DuskChanger");
			}
		}


		/// UPDATE
		public void update(GameTime gameTime, Player mainPlayer, InformationDisplay mainID) 
		{ 
			/// COIN AMOUNT DISPLAY

			// Coin count always displayed as a 3 digit number.
			if (mainPlayer.CoinCount >= 0 && mainPlayer.CoinCount < 10)
			{
				_CoinAmountString = "x00" + mainPlayer.CoinCount.ToString();
			}
			if (mainPlayer.CoinCount >= 10 && mainPlayer.CoinCount < 100)
			{
				_CoinAmountString = "x0" + mainPlayer.CoinCount.ToString();
			}
			if (mainPlayer.CoinCount > 99)
			{
				_CoinAmountString = "x" + mainPlayer.CoinCount.ToString();
			}



			/// TIMING SYSTEM
			/// This ensures there are constraints in place for minutes, hours and days.
			_CurrentTimeAdder += gameTime.ElapsedGameTime.Milliseconds;

			int TimeInMs = 1800; // 1800 is default value. Denotes speed of clock.

			if (_FinalCountdownBool == false)
			{
				if (_TotalTime == 1)
				{
					// When the game time reaches the final hour
					_CurrentTimeAdder = 0;
					_FinalCountdownBool = true;
				}

				if (_CurrentTimeAdder >= TimeInMs)
				{
					// Add a minute eachtime gametime reaches 1800ms.
					_CurrentMin += 1;
					_CurrentTimeAdder = 0;
				}

				if (_CurrentHour == 24)
				{
					_CurrentHour = 0;
				}

				if (_CurrentMin >= 60)
				{
					_CurrentMin = 0;
					_CurrentHour += 1;
					_TotalTime -= 1;
				}




				if ((_TotalTime <= 36 && _TotalTime > 24) || (_TotalTime <= 12 && _TotalTime >= 1))
				{
					// Between 20:00-07:59 the time is dusk.
					_Dusk = true;
					_Dawn = false;
				}
				else {
					_Dawn = true;
					_Dusk = false;
				}

				if (_TotalTime == 24 && mainID.FinalDayShown == false)
				{
					// Final day when only 24 hours remaining.
					mainID.ShowFinalDay = true;
				}


				// Form a clock as a string
				// Depending on how many digits is in the current minute and hour.
				// Formats as a 24h clock.
				if (_CurrentHour < 10)
				{
					_CurrentHourStr = "0" + _CurrentHour.ToString();
				}

				if (_CurrentMin < 10)
				{
					_CurrentMinStr = "0" + _CurrentMin.ToString();
				}

				if (_CurrentHour >= 10)
				{
					_CurrentHourStr = _CurrentHour.ToString();
				}

				if (_CurrentMin >= 10)
				{
					_CurrentMinStr = _CurrentMin.ToString();
				}

				_CurrentTime = _CurrentHourStr + ":" + _CurrentMinStr;
			}
			else {
				// The game has reached the final hour

				if (_CurrentTimeAdder >= TimeInMs)
				{
					_FinalCountdown -= 1;
					_CurrentTimeAdder = 0;
				}

				if (_FinalCountdown <= 0)
				{
					// Game over when the timer has ran to 0 for the final hour.
					_TotalTime = 0;
					mainID.GameOver = true;
					_FinalCountdown = 0;
				}
			}
		}

		/// DRAW
		public void Draw(SpriteBatch spriteBatch, Player mainPlayer, InformationDisplay mainID, SpriteFont font) 
		{ 
			spriteBatch.Draw(HUDBack, new Vector2(0, 0));

			spriteBatch.DrawString(font, "HEALTH", new Vector2(20, 10), Color.White);
			spriteBatch.Draw(HeartIcon, new Vector2(22, 45));

			if (mainPlayer.Health > 20)
			{
				spriteBatch.DrawString(font, (mainPlayer.Health.ToString()), new Vector2(65, 55), Color.White); // current player health levels displayed
			}
			else {
				spriteBatch.DrawString(font, (mainPlayer.Health.ToString()), new Vector2(65, 55), Color.Red); // current player health levels displayed
			}

			// A and B button icons on the HUD
			spriteBatch.Draw(HUD_B_Button, new Vector2(160, 6));
			spriteBatch.Draw(HUD_A_Button, new Vector2(240, 6));
			spriteBatch.Draw(HUD_ItemHolder, new Vector2(320, 6));

			spriteBatch.Draw(Coin_Icon, new Vector2(344, 18));
			spriteBatch.DrawString(font, _CoinAmountString, new Vector2(338, 60), Color.White); // current player coin amount displayed


			if (_FinalCountdownBool == false)
			{
				spriteBatch.DrawString(font, _CurrentTime, new Vector2(632, 55), Color.White);
			}
			else {
				// Final hour string drawn as red font.
				spriteBatch.DrawString(font, "FINAL HOUR: " + _FinalCountdown, new Vector2(580, 55), Color.Red);
			}

			if (_Dawn == true && DawnGraphic != null)
			{
				spriteBatch.Draw(DawnGraphic, new Vector2(618, 16));
			}
			if (_Dusk == true && DuskGraphic != null && DuskChanger != null)
			{
				spriteBatch.Draw(DuskGraphic, new Vector2(618, 16)); // Transparent layer darkens the game to give night effect.s
				spriteBatch.Draw(DuskChanger, new Vector2(0, 96));
			}
		}
	}
}