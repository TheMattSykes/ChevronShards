using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{

	public class Player : Entity
	{
		/// Variable Declaration and Get Set Functions

		private Texture2D _PlayerGraphic;
		private Texture2D _PlayerHitGraphic;

		private WeaponInterface _playerWeapon; // Player's weapon
		public WeaponInterface PlayerWeapon { get { return _playerWeapon; } }

		private Texture2D _PlayerWeaponGraphic;

		private int WeaponOptNum; // which weapon option is currently selected

		private int timeSinceLastFrame; // for player sprite sheet animation

		private bool _drawWeapon;
		public bool DrawWeapon { get { return _drawWeapon; } set { _drawWeapon = value; } }

		private bool _allowWeaponFire { get; set; } // Is weapon fire allowed
		public bool AllowWeaponFire { get { return _allowWeaponFire; } set { _allowWeaponFire = value; } }

		private double _timeSincePlayerWeaponFire; // time expressed as a double in ms since player last fired weapon.

		private bool _playerWeaponFiring;
		public bool PlayerWeaponFiring { get { return _playerWeaponFiring; } set { _playerWeaponFiring = value; } }

		private bool _newPlayerWeaponFire;
		public bool NewPlayerWeaponFire { get { return _newPlayerWeaponFire; } set { _newPlayerWeaponFire = value; } }


		// Boolean values denote which area the player is in
		private bool _inOverworld;
		public bool InOverworld { get { return _inOverworld; } set { _inOverworld = value; } }

		private bool _inDungeon;
		public bool InDungeon { get { return _inDungeon; } set { _inDungeon = value; } }

		private bool _inBossLevel;
		public bool InBossLevel { get { return _inBossLevel; } set { _inBossLevel = value; } }

		private bool _hasChangedArea;
		public bool HasChangedArea { get { return _hasChangedArea; } set { _hasChangedArea = value; } }



		private bool[,] _visitedOWSections = new bool[5, 5]; // which sections in Row,Column form has the player already visited in the overworld.
		public bool[,] VisitedOWSections  { get { return _visitedOWSections; } set { _visitedOWSections = value; } }

		private Vector2 _currentOWSec; // vector value of where the player is in the overworld.
		public Vector2 CurrentOWSec { get{ return _currentOWSec; } set {_currentOWSec = value; } }

		private Vector2 _previousOWSec; // where the player was previously in the overworld.
		public Vector2 PreviousOWSec { get { return _previousOWSec; } 
			set { _previousOWSec = value; } }




		private bool[,] _visitedDUN1Sections = new bool[4, 4]; // which sections in Row,Column form has the player already visited in dungeon1.
		public bool[,] VisitedDUN1Sections { get { return _visitedDUN1Sections; } set { _visitedDUN1Sections = value; } }

		private bool[,] _visitedDUN2Sections = new bool[4, 4]; // which sections in Row,Column form has the player already visited in dungeon2.
		public bool[,] VisitedDUN2Sections { get { return _visitedDUN2Sections; } set { _visitedDUN2Sections = value; } }

		private bool[,] _visitedDUN3Sections = new bool[4, 4]; // which sections in Row,Column form has the player already visited in dungeon3.
		public bool[,] VisitedDUN3Sections { get { return _visitedDUN3Sections; } set { _visitedDUN3Sections = value; } }

		private int _currentDungeonNumber; // which dungeon is the player currently in
		public int CurrentDungeonNumber { get { return _currentDungeonNumber; } set { _currentDungeonNumber = value; } }

		private int _currentBLNumber; // BossLevel number associated with dungeon, usually set when player enters dungeon.
		public int CurrentBLNumber { get { return _currentBLNumber; } set { _currentBLNumber = value; } }

		private Vector2 _currentDUNSec; // vector value of where the player is in the dungeon.
		public Vector2 CurrentDUNSec 
		{ 
			get { return _currentDUNSec; } 
			set { _currentDUNSec = value; } 
		}

		private Vector2 _previousDUNSec; // where the player was previously in the dungeon.
		public Vector2 PreviousDUNSec
		{
			get { return _previousDUNSec; }
			set { _previousDUNSec = value; }
		}

		private bool[] _completedDungeons = new bool[3]; // Which dungeons have been completed
		public bool[] CompletedDungeons { get { return _completedDungeons; } set { _completedDungeons = value; } }

		private bool _justCompletedDungeon; // When the player has just exited a dungeon this will be set to true.
		public bool JustCompletedDungeon { get { return _justCompletedDungeon; } set { _justCompletedDungeon = value; } }



		private int _coinCount; // Denotes how many coins the player has currently collected
		public int CoinCount { 
			get { return _coinCount; } 
			set 
			{ 
				if (_coinCount < 999)
				{
					_coinCount = value;
				}
				else {
					_coinCount = 999;
				}
			} 
		}


		private bool _playerHit = false; // Has the player been hit
		private int _recoveryTime = 1000; // Denotes how long until the player can be hit again after being hit.
		private int _hitTime = 0; // Will be incremented by gameTime until reaches _recoveryTime.


		private string _currentWeapon; // Name of current player weapon
		public string CurrentWeapon { get { return _currentWeapon; } set { _currentWeapon = value; } }


		// FOR ANIMATION
		private int _msPerFrame = 150; // how much time per frame
		private Point _sheetSize = new Point(2, 0); // total size of sprite sheet
		private Point _currentPlayerFrame = new Point(0, 0); // current frame on sheet
		private bool _hasAnimationReset = false; // has the animation been reset


		public Player() 
		{
			// Set Default values
			_Health = 100;
			_HealthMax = 100;
			_Height = 37;
			_Width = 26;
		}




		/// LOAD TEXTURES
		public void LoadContent(ContentManager content) { 

			// PLAYER TEXTURES
			_PlayerGraphic = content.Load<Texture2D>(GetPlayerGraphic());
			_PlayerHitGraphic = content.Load<Texture2D>("Player_Hit");


			// PLAYERS WEAPON GRAPHIC
			if (_drawWeapon == true)
			{
				_PlayerWeaponGraphic = content.Load<Texture2D>(_playerWeapon.GetWeaponTextureName(_playerWeapon.GetWeaponOrientation()));
			}
		}
		/// END LOAD TEXTURES






		// PLAYER COLLISION DATA
        public Rectangle Rect() {
            Rectangle R = new Rectangle((int)_entityPos.X,(int)_entityPos.Y,_Width,_Height); // draws rectangle around the player
            return R;
        }


		/// Change weapon function
		/// Uses current weapon string to denote which object to set the playerweapon to.
		public void ChangeWeapon() 
		{
			if (_currentWeapon == "Sword") 
			{ 
				_playerWeapon = new Sword(); // default weapon
			}

			if (_currentWeapon == "Seed")
			{
				_playerWeapon = new Seed();
			}

			if (_currentWeapon == "FireBall")
			{
				_playerWeapon = new FireBall();
			}

			if (_currentWeapon == "WaterBall")
			{
				_playerWeapon = new WaterBall();
			}
		}








		// ANIMATION

		public void Animation(GameTime gameTime, ref int timeSinceLastFrame)
		{
			_hasAnimationReset = false;

			if (timeSinceLastFrame >= _msPerFrame) // every time gametime has reached msPerFrame increment by one frame
			{
				_currentPlayerFrame.X += 1;
				_hasAnimationReset = true; // reset animation from next frame.
			}

			if (_currentPlayerFrame.X >= _sheetSize.X) // if increment larger than sheet size then go back to start
			{
				_currentPlayerFrame.X = 0;
				_hasAnimationReset = true;
			}
		}






		/// GetPlayerGraphic
		public string GetPlayerGraphic() { // Returns string denoting which sprite will be used, depending on direction.
			if (_orientation == 'U') { 
				return "Player_Walk_Up";
			}
			if (_orientation == 'D')
			{
				return "Player_Walk_Down";
			}
			if (_orientation == 'L')
			{
				return "Player_Walk_Left";
			}
			if (_orientation == 'R')
			{
				return "Player_Walk_Right";
			}
			else {
				return "Player_Walk_Right"; // default is Right incase of error with orientation.
			}
		}






		/// PLAYER COLLISION

		public bool PlayerEnemyCollision(AreaInterface areaPoly)
		{
			// Cycle through each enemy in the EnemyList, draw a rectangle around each one and check for player intersection.
			for (int i = 0; i < areaPoly.EnemyList.Count; i++)
			{
				Rectangle CurrentRect = new Rectangle((int)areaPoly.EnemyList[i].EnemyCoordinates.X, (int)areaPoly.EnemyList[i].EnemyCoordinates.Y, areaPoly.EnemyList[i].Width, areaPoly.EnemyList[i].Height);
				// draw a rectangle around the current enemy in the list

				if (Rect().Intersects(CurrentRect) == true)
				{
					if (_hitTime < 1) // only allow certain amount of hits by enemy at one time.
					{
						_playerHit = true;
					}

					_allowEntityMovement = false;
					entityReaction(); // call method for player to react to collision.
					_allowEntityMovement = true;
					return true; // collision
				}
			}
			return false; // no collision

		}



		public bool PlayerEnemyWeaponCollision(AreaInterface areaPoly)
		{ // player hit by enemy weapon

			List<EnemyInterface> EnemyList = areaPoly.EnemyList;

			// Cycle through EnemyList, draw a rectangle around the enemies weapon and check for player intersection.
			for (int i = 0; i < EnemyList.Count; i++)
			{

				Rectangle CurrentRect = new Rectangle((int)EnemyList[i].EnemyWeapon.GetWeaponCoordinates().X, (int)EnemyList[i].EnemyWeapon.GetWeaponCoordinates().Y, EnemyList[i].EnemyWeapon.GetWeaponWidth(), EnemyList[i].EnemyWeapon.GetWeaponHeight());

				try
				{
					if (Rect().Intersects(CurrentRect) == true && areaPoly.EnemyList[i].DrawWeapon == true)
					{

						if (_hitTime < 1) // only allow certain amount of hits by weapon at one time.
						{
							_playerHit = true;
						}

						_allowEntityMovement = true;
						return true;
					}
				}
				catch { 
					return false;
				}
			}
			return false;

		}

		/// END OF PLAYER COLLISION





		public void PlayerGenCoordinate(AreaInterface areaInt)
		{ // Generate random new player coordaintes within bounds
			List<Rectangle> RectList = areaInt.CollisionRects; // List of all rectangles drawn around 48x48 structure blocks.

			Random R = new Random();
			Vector2 FinalCoordinates;
			bool useCoordinates = true;

			while (true) // endless loop until break, the loop will break if coordaintes are allowed.
			{

				useCoordinates = true;

				// X and Y constraints
				int X = R.Next(48, 300);
				int Y = R.Next(240, 576);
				Vector2 PossibleCoordinates = new Vector2(X, Y); // proposed random coordaintes

				Rectangle NewPlayerRect = new Rectangle(X, Y, _Width + 30, _Height + 30); // form a rectangle around the enemy for collision purposes, allows 10 pixels around it

				// For loop to prevent spawning of enemies on other objects
				for (int i = 0; i < RectList.Count; i++)
				{
					if (NewPlayerRect.Intersects(RectList[i]))
					{
						useCoordinates = false; // Do not use coordinates if they are within a 48x48 structure block.
					}
				}

				if (useCoordinates == true) // use the coordaintes
				{
					FinalCoordinates = PossibleCoordinates;
					_entityPos = FinalCoordinates;
					_drawPos = FinalCoordinates;
					break;
				}
			}
		}








		// NOTE: PLAYER ITEM COLLISION IS IN THE ITEM MANAGER - instantiated as mainIM in Game1 class.



		// MAIN FUNCTIONS


		public void Update(GameTime gameTime, GamePadState state, AreaInterface areaPoly, InformationDisplay mainID)
        {


			int playerSpeed = 4; // player speed set

			if (_hasAnimationReset == true)
			{
				timeSinceLastFrame = 0; // resets animation
			}


			// CHECK MOVE MANAGER FUNCTIONS RETURN WHETHER THE PLAYER CAN MOVE IN THE DIRECTION THEY ARE FACING
			bool checkMove = false;


			checkMove = areaPoly.checkMoveManager(Rect()); // check whether the move is valid




			// CHECK EXIT MANAGER FUNCTIONS RETURN WHETHER THE PLAYER WILL MOVE INTO ANOTHER SECTION OF THE OVERWORLD OR DUNGEON
			string checkExit = "";

			if (mainID.EnergyBarrierStatus == false) // CHANGE THIS TO JUST FALSE
			{
				if (_inOverworld == true)
				{
					checkExit = areaPoly.checkExitManager(Rect(), CurrentOWSec); // checks to see if the player is near an exit
				}
				else 
				{ 
					checkExit = areaPoly.checkExitManager(Rect(), CurrentDUNSec); // checks to see if the player is near an exit
				}
			}


			// COLLISION WITH ENEMY ENTITIES
			bool checkEnemyCollision = false;

			if (areaPoly.EnemyList != null)
			{
				if (areaPoly.EnemyList.Count > 0)
				{
					checkEnemyCollision = PlayerEnemyCollision(areaPoly);

					PlayerEnemyWeaponCollision(areaPoly); // collisions with enemy weapons
				}
			}




			if (checkMove == false && checkEnemyCollision == false) { _drawPos = _entityPos; } // set draw coordaintes to equal the entity coordinates as they are valid.
			if ((checkMove == true || checkEnemyCollision == true) && _allowEntityDirChange == true) { _entityPos = _drawPos; } // Opposite to previous comment as they are not valid.


			// If statements and logic for the player traveling between areas

			if (_inOverworld == true) // if the player is in the overworld
			{

				bool checkDunEntrance = areaPoly.checkDunEntranceManager(_entityPos, _Height, _Width, ref _currentDungeonNumber); // check if the player has gone through dungeon entrance

				if (checkDunEntrance == true)
				{
					if (areaPoly.EnemyList != null)
					{
						areaPoly.EnemyList.Clear(); // clear the enemy list
					}

					_inDungeon = true; // enter the dungeon 
					_inOverworld = false;
					_inBossLevel = false;
					_hasChangedArea = true;

					// default perameters/values for dungeons

					_currentDUNSec = new Vector2(2, 3);
					_previousDUNSec = new Vector2(2, 3);

					if (_currentDungeonNumber == 1)
					{
						_entityPos = new Vector2(288 - _Width, 650);
						_drawPos = _entityPos;
					}

					if (_currentDungeonNumber == 2)
					{
						_entityPos = new Vector2(288 - _Width, 650);
						_drawPos = _entityPos;
					}

					if (_currentDungeonNumber == 3)
					{
						_entityPos = new Vector2(288 - _Width, 650);
						_drawPos = _entityPos;
					}
				}
			}


			if (_inDungeon == true) // if the player is in a dungeon
			{
				bool checkDunExit = areaPoly.checkDunExitManager(Rect()); // check to see if player has collided with dungeon exit

				if (checkDunExit == true)
				{
					if (areaPoly.EnemyList != null)
					{
						areaPoly.EnemyList.Clear(); // clear the enemy list
					}

					// change area to overworld
					_inDungeon = false;
					_inOverworld = true;
					_inBossLevel = false;
					_hasChangedArea = true;


					// Set default entity position for overworld section to avoid collisions with structure
					if ((int)_currentOWSec.X == 4 && (int)_currentOWSec.Y == 2)
					{
						_entityPos = new Vector2(366, 496);
					}

					if ((int)_currentOWSec.X == 1 && (int)_currentOWSec.Y == 0)
					{
						_entityPos = new Vector2(366, 496);
					}

					if ((int)_currentOWSec.X == 0 && (int)_currentOWSec.Y == 4)
					{
						_entityPos = new Vector2(510, 544);
					}

					_drawPos = _entityPos;
				}


				bool checkBossLevelEntrance = areaPoly.checkBossLevelEntranceManager(Rect()); // check if player rectangle collides with BossLevel entrance

				if (checkBossLevelEntrance == true && mainID.EnergyBarrierStatus == false)
				{
					// Change area to BossLevel
					_inDungeon = false;
					_inOverworld = false;
					_inBossLevel = true;
					_hasChangedArea = true;

					_entityPos = new Vector2(((720 / 2) - _Width), 520); // Default position when entering a boss level
				}
			}

			if (_inBossLevel == true) // player is located in BosLevel
			{
				if (areaPoly.CheckCompletedDungeon() == true) // check to see if dungeon has been completed
				{
					_completedDungeons[areaPoly.GetBossLevelNumber() - 1] = true;
					_justCompletedDungeon = true;

					if (areaPoly.EnemyList != null)
					{
						areaPoly.EnemyList.Clear(); // clear the enemy list
					}

					// Change location to the Overworld
					_inDungeon = false;
					_inOverworld = true;
					_inBossLevel = false;
					_hasChangedArea = true;


					// Default values when exiting BossLevel
					if ((int)_currentOWSec.X == 4 && (int)_currentOWSec.Y == 2)
					{
						_entityPos = new Vector2(366, 496);
					}

					if ((int)_currentOWSec.X == 1 && (int)_currentOWSec.Y == 0)
					{
						_entityPos = new Vector2(366, 496);
					}

					if ((int)_currentOWSec.X == 0 && (int)_currentOWSec.Y == 4)
					{
						_entityPos = new Vector2(510, 544);
					}

					_drawPos = _entityPos;
				}
			}


			// Player weapon hit detection
			if (_playerHit == true) {

				if (_hitTime < 1) {
					_Health -= 10; // deduct 10 health from the player

					if (_Health <= 0) {
						_Health = 0;
						mainID.GameOver = true; // health is 0 so GameOver.
					}
				}

				// Animation for player hit
				_hitTime += gameTime.ElapsedGameTime.Milliseconds; // add time to variable
				timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds; // add time to variable
				Animation(gameTime, ref timeSinceLastFrame); // animate through the sprite sheet when moving

				// Check recovery
				if (_hitTime >= _recoveryTime) {
					_playerHit = false;
					_hitTime = 0;
				}
			}



			// Switch Player Weapons
			if (_allowEntityMovement == true && mainID.RegisterSelectPress == true && _playerWeaponFiring == false)
			{
				if (state.Buttons.LeftStick == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.S) == true)
				{
					WeaponOptNum++; // increase weapon option by 1

					while (true)
					{
						if (WeaponOptNum > 4 || WeaponOptNum < 1)
						{
							WeaponOptNum = 1; // Reset Weapon Option Number to 1
						}

						// Change player weapon based on Weapon Option Number (Changed when player uses S key or SELECT button)
						if (WeaponOptNum == 1) // Default weapon, accessible by all players
						{
							_currentWeapon = "Sword";
							break;
						}
						// For the next weapons, they are only avaliable on the condition that a specific dungeon has been completed.
						if (WeaponOptNum == 2 && CompletedDungeons[0] == true)
						{
							_currentWeapon = "Seed";
							break;
						}
						if (WeaponOptNum == 3 && CompletedDungeons[1] == true)
						{
							_currentWeapon = "FireBall";
							break;
						}
						if (WeaponOptNum == 4 && CompletedDungeons[2] == true)
						{
							_currentWeapon = "WaterBall";
							break;
						}

						WeaponOptNum++; // increment the option by one, go back through loop.
					}

					ChangeWeapon(); // Change the weapon and set objects
					mainID.RegisterSelectPress = false; // Only allow one press of select or S key/button at a time.
				}
			}






			/// Player Movement and Overworld/Dungeon Section Exits

			if (_allowEntityDirChange == true && _allowEntityMovement == true && _entityPos == _drawPos)
			{ // If movement is allowed and player's coordinates are valid.
				
				if (state.IsButtonDown(Buttons.LeftThumbstickUp) || Keyboard.GetState().IsKeyDown(Keys.Up) || state.IsButtonDown(Buttons.LeftThumbstickDown) || Keyboard.GetState().IsKeyDown(Keys.Down)
					  || state.IsButtonDown(Buttons.LeftThumbstickLeft) || Keyboard.GetState().IsKeyDown(Keys.Left) || state.IsButtonDown(Buttons.LeftThumbstickRight)
					  || Keyboard.GetState().IsKeyDown(Keys.Right))
				{ // If any movement key is currently being pressed
					timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds; // add time to time since last frame
					Animation(gameTime, ref timeSinceLastFrame); // Call animation function

					// use information from CheckExit function if a value was returned
					if (checkExit == "EXIT U" || checkExit == "EXIT D" || checkExit == "EXIT L" || checkExit == "EXIT R")
					{
						if (_inOverworld == true)
						{
							_previousOWSec = new Vector2((int)_currentOWSec.X, (int)_currentOWSec.Y); // set previous overworld section
							areaPoly.ChangeSec = true; // changing section
							areaPoly.GeneratePreStructure((int)_previousOWSec.X, (int)_previousOWSec.Y); // Generate the structure of the previous section
							_visitedOWSections[(int)_previousOWSec.X, (int)_previousOWSec.Y] = true; // the player has now visited the section
						}

						if (_inDungeon == true)
						{
							_previousDUNSec = new Vector2((int)_currentDUNSec.X, (int)_currentDUNSec.Y); // set previous dungeon section
							areaPoly.ChangeSec = true; // changing section
							areaPoly.GeneratePreStructure((int)_previousDUNSec.X, (int)_previousDUNSec.Y); // Generate the structure of the previous section
							// the player has now visited the section, check which dungeon number section is in.
							if (_currentDungeonNumber == 1)
							{
								_visitedDUN1Sections[(int)_previousDUNSec.X, (int)_previousDUNSec.Y] = true;
							}
							if (_currentDungeonNumber == 2)
							{
								_visitedDUN2Sections[(int)_previousDUNSec.X, (int)_previousDUNSec.Y] = true;
							}
							if (_currentDungeonNumber == 3)
							{
								_visitedDUN3Sections[(int)_previousDUNSec.X, (int)_previousDUNSec.Y] = true;
							}
						}
					}
				}


				// MOVING BETWEEN SECTIONS OF THE OVERWORLD OR DUNGEONS

				// Direction depending on button pressed.

				if (state.IsButtonDown(Buttons.LeftThumbstickUp) || Keyboard.GetState().IsKeyDown(Keys.Up)) // this is the d-pad on some controllers
				{
					_orientation = 'U'; // player is facing upwards

					if (checkExit == "EXIT U") // player exits the overworld section up
					{
						if (_inOverworld == true)
						{
							_currentOWSec = new Vector2((int)_currentOWSec.X, (int)_currentOWSec.Y - 1); // move upwards by a section
						}

						if (_inDungeon == true)
						{
							_currentDUNSec = new Vector2((int)_currentDUNSec.X, (int)_currentDUNSec.Y - 1); // move upwards by a section
						}

						_entityPos = new Vector2(_entityPos.X, 670 - _Height); // Set location player will spawn in the new section

						areaPoly.ChangeDirection = 'U'; // which direction is the new section in.
					}

					if (checkMove == false && checkEnemyCollision == false && mainID.GameOver == false)
					{
						_entityPos = new Vector2(_entityPos.X, _entityPos.Y - playerSpeed); // allow the player to move upwards 3 pixels at a time for each call of function
					}
				}


				else if (state.IsButtonDown(Buttons.LeftThumbstickDown) || Keyboard.GetState().IsKeyDown(Keys.Down))
				{
					_orientation = 'D';

					if (checkExit == "EXIT D") // player exits the overworld section down
					{
						if (_inOverworld == true)
						{
							_currentOWSec = new Vector2((int)_currentOWSec.X, (int)_currentOWSec.Y + 1); // move downwards by a section
						}

						if (_inDungeon == true)
						{
							_currentDUNSec = new Vector2((int)_currentDUNSec.X, (int)_currentDUNSec.Y + 1); // move downwards by a section
						}

						_entityPos = new Vector2(_entityPos.X, 126); // Set location player will spawn in the new section
						areaPoly.ChangeDirection = 'D';
					}

                    if (checkMove == false && checkEnemyCollision == false && mainID.GameOver == false)
					{
						_entityPos = new Vector2(_entityPos.X, _entityPos.Y + playerSpeed);
					}
				}

				else if (state.IsButtonDown(Buttons.LeftThumbstickLeft) || Keyboard.GetState().IsKeyDown(Keys.Left))
				{

					_orientation = 'L';

					if (checkExit == "EXIT L") // player exits the overworld section left
					{
						if (_inOverworld == true)
						{
							_currentOWSec = new Vector2((int)_currentOWSec.X - 1, (int)_currentOWSec.Y); // move left by a section
						}

						if (_inDungeon == true)
						{
							_currentDUNSec = new Vector2((int)_currentDUNSec.X - 1, (int)_currentDUNSec.Y); // move left by a section
						}

						_entityPos = new Vector2(680, _entityPos.Y); // Set location player will spawn in the new section
						areaPoly.ChangeDirection = 'L';
					}


                    if (checkMove == false && checkEnemyCollision == false && mainID.GameOver == false)
					{
						_entityPos = new Vector2(_entityPos.X - playerSpeed, _entityPos.Y);
					}

				}

				else if (state.IsButtonDown(Buttons.LeftThumbstickRight) || Keyboard.GetState().IsKeyDown(Keys.Right))
				{

					_orientation = 'R';

					if (checkExit == "EXIT R") // player exits the overworld section right
					{
						if (_inOverworld == true)
						{
							_currentOWSec = new Vector2((int)_currentOWSec.X + 1, (int)_currentOWSec.Y); // move right by a section
						}

						if (_inDungeon == true)
						{
							_currentDUNSec = new Vector2((int)_currentDUNSec.X + 1, (int)_currentDUNSec.Y); // move right by a section
						}

						_entityPos = new Vector2(30, _entityPos.Y); // Set location player will spawn in the new section
						areaPoly.ChangeDirection = 'R';
					}

                    if (checkMove == false && checkEnemyCollision == false && mainID.GameOver == false)
					{
						_entityPos = new Vector2(_entityPos.X + playerSpeed, _entityPos.Y);
					}
				}
			}

			_allowEntityDirChange = true;

			/// END OF PLAYER MOVEMENT AND SECTION EXITS




			// PLAYER WEAPON FIRE

			if (_allowWeaponFire == true && areaPoly.ChangeSec == false && mainID.GameOver == false)
			{

				_timeSincePlayerWeaponFire += gameTime.ElapsedGameTime.TotalMilliseconds; // add time to player weapon fire


				if (state.IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.B)) // A button is Keys.B on controller used for development
				{
					if (_newPlayerWeaponFire == true)
					{
						_timeSincePlayerWeaponFire = 0; // reset timer
						_playerWeaponFiring = true;
						_drawWeapon = true;
						if (_currentWeapon == "Sword")
						{
							_playerWeapon.SetWeaponCoordinates(_entityPos); // set the weapons coordinates to fire from the players location
							_playerWeapon.SetWeaponOrientation(_orientation); // Set the weapon ordientation in the direction the player is facing
						}

						if (_currentWeapon == "Seed")
						{
							_playerWeapon.SetWeaponCoordinates(_entityPos); // set the weapons coordinates to fire from the players location
							_playerWeapon.SetWeaponOrientation(_orientation); // Set the weapon ordientation in the direction the player is facing
						}

						if (_currentWeapon == "FireBall")
						{
							_playerWeapon.SetWeaponCoordinates(_entityPos); // set the weapons coordinates to fire from the players location
							_playerWeapon.SetWeaponOrientation(_orientation); // Set the weapon ordientation in the direction the player is facing
						}

						if (_currentWeapon == "WaterBall")
						{
							_playerWeapon.SetWeaponCoordinates(_entityPos); // set the weapons coordinates to fire from the players location
							_playerWeapon.SetWeaponOrientation(_orientation); // Set the weapon ordientation in the direction the player is facing
						}
					}
				}


				// Fire Player Weapon
				if (_timeSincePlayerWeaponFire <= 410 && _playerWeaponFiring == true)
				{
					_newPlayerWeaponFire = false;

					_playerWeapon.FireWeapon(); // Fire the weapon

					// Set coordinates of player weapon based on direction
					if (_playerWeapon.GetWeaponOrientation() == 'U')
					{
						_playerWeapon.SetWeaponCoordinates(new Vector2(_playerWeapon.GetWeaponCoordinates().X, _playerWeapon.GetWeaponCoordinates().Y - 7));
					}

					if (_playerWeapon.GetWeaponOrientation() == 'D')
					{
						_playerWeapon.SetWeaponCoordinates(new Vector2(_playerWeapon.GetWeaponCoordinates().X, _playerWeapon.GetWeaponCoordinates().Y + 7));
					}

					if (_playerWeapon.GetWeaponOrientation() == 'L')
					{
						_playerWeapon.SetWeaponCoordinates(new Vector2(_playerWeapon.GetWeaponCoordinates().X - 7, _playerWeapon.GetWeaponCoordinates().Y));
					}

					if (_playerWeapon.GetWeaponOrientation() == 'R')
					{
						_playerWeapon.SetWeaponCoordinates(new Vector2(_playerWeapon.GetWeaponCoordinates().X + 7, _playerWeapon.GetWeaponCoordinates().Y));
					}
				}
				if (_timeSincePlayerWeaponFire > 400 && _timeSincePlayerWeaponFire <= 450 && _playerWeaponFiring == true) // stop displaying and firing weapon
				{
					_playerWeaponFiring = false;
					_drawWeapon = false;
				}
				if (_timeSincePlayerWeaponFire > 500 && _playerWeaponFiring == false) // weapon cooldown
				{
					_timeSincePlayerWeaponFire = 0; // reset
					_newPlayerWeaponFire = true;
				}
			}
		}


		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Draw player related graphics
			if (_drawWeapon == true)
			{
				spriteBatch.Draw(_PlayerWeaponGraphic, _playerWeapon.GetWeaponCoordinates());
			}

			// The next two graphics will only be drawn within a specified rectangle, this allows a specific frame of an animation to be displayed.
			if (_playerHit == true)
			{
				spriteBatch.Draw(_PlayerHitGraphic, _drawPos, new Rectangle((_currentPlayerFrame.X * _Width), (_currentPlayerFrame.Y * _Height),
																		_Width, _Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			}
			else { 
				spriteBatch.Draw(_PlayerGraphic, _drawPos, new Rectangle((_currentPlayerFrame.X * _Width), (_currentPlayerFrame.Y * _Height),
																		_Width, _Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			}
		}
	}
}
