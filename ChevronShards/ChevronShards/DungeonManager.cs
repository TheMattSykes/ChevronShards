using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;


namespace ChevronShards
{
	public class DungeonManager : Area
	{

		private bool _animationRunning = false;

		private int _DunNum; // Dungeon Number

		// Stores X and Y coordinates of Section used for text file.
		private int _DunX;
		private int _DunY;
		private int _preDunX;
		private int _preDunY;

		private bool _BossLevelEntrance;
		private char _BossLevelChangeDirection;

		// Store item information from text file.
		private string _ItemTypeA;
		private string _ItemTypeB;
		private int _ItemAAmount;
		private int _ItemBAmount;

		private Texture2D DungeonBack;
		private Texture2D PreDungeonBack; // for animation purposes
		private Texture2D Brick;


		/// LoadContent
		/// Load content data into Texture variables.
		public override void LoadContent(ContentManager Content)
		{
			DungeonBack = Content.Load<Texture2D>(GetTextureNameManager());

			if (_changeSec == true)
			{
				this.PreDungeonBack = Content.Load<Texture2D>(GetPreTextureNameManager());
			}
			Brick = Content.Load<Texture2D>("Gray_Bricks");
		}


		/// DUNGEON TEXTURE MANAGER
		/// This function returns the name of the dungeon section's texture
		public string GetTextureNameManager()
		{
			return ("Dungeon" + _DunNum + "Background");
		}

		/// PREVIOUS DUNGEON TEXTURE MANAGER
		/// This function returns the name of the dungeon section's texture
		public string GetPreTextureNameManager()
		{
			return ("Dungeon" + _DunNum + "Background");
		}

		/// GetItemType
		/// Returns the name of the item after using the parameter of a char from the text file.
		public string GetItemType(char c)
		{

			if (c == 'C')
			{
				return "Coin";
			}

			if (c == 'H')
			{
				return "Heart";
			}


			return "";
		}

		/// Dungeon Section Generator
		/// Reads from the relevent Dungeon Section text file, converts each line into usable information as commented below.
		public override void GenerateStructure(params int[] values)
		{
			_DunNum = values[0];

			_DunX = values[1];
			_DunY = values[2];

			_preDunX = _DunX;
			_preDunY = _DunY;

			string line;
			string FileName = "Dungeon"+_DunNum+"_" + _DunX + "_" + _DunY + ".txt";

			int Count = 0;

			int Row = 0;
			int Column = 0;

			StreamReader Reader = new StreamReader(FileName);

			while ((line = Reader.ReadLine()) != null)
			{
				if (Count == 0) // First Line states the type of Enemy with a character and then a number 0-9 of how many there are.
				{
					if (line[0] == 'N') // no enemies
					{
						_enemyType = null;

						_enemyAmount = 0;
					}

					if (line[0] == 'F')
					{
						_enemyType = "Firmeleon";

						_enemyAmount = (int)line[1] - 48; // converted from character so -48 to get intended number.
					}

					if (line[0] == 'L')
					{
						_enemyType = "Leafen";

						_enemyAmount = (int)line[1] - 48;
					}

					if (line[0] == 'S')
					{
						_enemyType = "Seafen";

						_enemyAmount = (int)line[1] - 48;
					}
				}

				if (Count == 1) // Second Line states the type of Item with a character and then a number 0-9 of how many there are.
				{
					if (line[0] == 'N')
					{
						_itemList.Clear();
						_itemRects.Clear();

						_ItemTypeA = null;

						_ItemAAmount = 0;
					}

					if (line[0] != 'N')
					{
						_ItemTypeA = GetItemType(line[0]);

						_ItemAAmount = line[1] - 48;
					}

					if (line.Length > 2)
					{
						if (line[2] == 'N')
						{
							_ItemTypeB = null;
							_ItemBAmount = 0;
						}

						if (line[2] != 'N')
						{
							_ItemTypeB = GetItemType(line[2]);

							_ItemBAmount = line[3] - 48;
						}
					}
				}

				if (Count == 2) // Second Line states the type of Item with a character and then a number 0-9 of how many there are.
				{
					if (line[0] == 'N')
					{
						_BossLevelEntrance = false;
						_BossLevelChangeDirection = 'N';
					}

					else {
						_BossLevelEntrance = true;
						_BossLevelChangeDirection = line[0];
					}
				}

				if (Count >= 3) // The Final Lines denote the structure of the overworld with a 1 indicating a block e.g. a tree is present, or a 0 for a blank space.
				{
					for (int i = 0; i < line.Length; i++)
					{
						if (line[i] == '1')
						{
							_Structure[Row, Column] = true;
						}
						if (line[i] == '0')
						{
							_Structure[Row, Column] = false;
						}
						Row++;
					}

					Row = 0;
					Column++;
				}

				Count++;
			}

			Reader.Close();
			GenerateRectangleCollisions();
		}


		/// GeneratePreStructure
		/// Does the same thing as the GenerateStruture method but instead stores data/values for the previous section.
		/// Some parts of the file are ommitted as they are not relevent to the sliding animation.
		public override void GeneratePreStructure(params int[] values) // for sliding animation
		{
			string line;
			string FileName = "Dungeon" + _DunNum + "_" + _preDunX + "_" + _preDunY + ".txt";

			int Count = 0;

			int Row = 0;
			int Column = 0;

			StreamReader Reader = new StreamReader(FileName);

			while ((line = Reader.ReadLine()) != null)
			{
				if (Count >= 3) // The Final Lines denote the structure of the overworld with a 1 indicating a block e.g. a tree is present, or a 0 for a blank space.
				{
					for (int i = 0; i < line.Length; i++)
					{
						if (line[i] == '1')
						{
							_PreviousStructure[Row, Column] = true;
						}
						if (line[i] == '0')
						{
							_PreviousStructure[Row, Column] = false;
						}
						Row++;
					}

					Row = 0;
					Column++;
				}

				Count++;
			}

			Reader.Close();
		}



		/// GenerateItems
		/// Uses information gained previously from the text file, adds items to itemList through a for loop.
		public override void GenerateItems(Player mainPlayer, AreaInterface areaInt, InformationDisplay mainID)
		{

			if (mainID.EraseList == true)
			{
				_itemList.Clear();
				_itemRects.Clear();
				mainID.EraseList = false;
			}


			if (_ItemTypeA != null || _ItemTypeB != null && _ItemAAmount > 0)
			{
				if (_ItemTypeA != null)
				{
					if (mainID.NewItemList == true)
					{
						for (int i = 0; i <= _ItemAAmount; i++)
						{
							if (_ItemTypeA == "Coin")
							{
								if (_DunNum == 1)
								{
									if (mainPlayer.VisitedDUN1Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else if (_DunNum == 2)
								{
									if (mainPlayer.VisitedDUN2Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else if (_DunNum == 3)
								{
									if (mainPlayer.VisitedDUN3Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else 
								{
									_ItemTypeA = null;
									_ItemAAmount = 0;
								}
							}

							if (_ItemTypeA == "Heart")
							{
								_itemList.Add(new Heart()); // add Heart object to list
							}

							if (_ItemTypeA != null)
							{
								try
								{
									_itemList[i].DrawItem = true;
								}
								catch
								{
									_ItemTypeA = null;
								}
							}
						}

						for (int i = 0; i < _itemList.Count; i++)
						{
							// Generate random coordinate in bounds.
							Vector2 coordinates = _itemList[i].ItemGenCoordinate(i, mainPlayer, areaInt);

							_itemList[i].ItemCoordinates = coordinates;
						}
					}
				}

				if (_ItemTypeB != null)
				{
					if (mainID.NewItemList == true)
					{

						for (int i = 0; i <= _ItemBAmount; i++)
						{
							if (_ItemTypeB == "Coin")
							{
								if (_DunNum == 1)
								{
									if (mainPlayer.VisitedDUN1Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else if (_DunNum == 2)
								{
									if (mainPlayer.VisitedDUN2Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else if (_DunNum == 3)
								{
									if (mainPlayer.VisitedDUN3Sections[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] == false) // if section has not been visited
									{
										_itemList.Add(new Coin()); // add Coin object to list
									}
								}
								else {
									_ItemTypeB = null;
									_ItemBAmount = 0;
								}
							}

							if (_ItemTypeB == "Heart")
							{
								_itemList.Add(new Heart()); // add Heart object to list
							}

							if (_ItemTypeB != null)
							{
								try
								{
									_itemList[i].DrawItem = true;
								}
								catch
								{
									_ItemTypeB = null;
								}
							}

						}

						for (int i = 0; i < _itemList.Count; i++)
						{
							// Generate random coordinate in bounds.
							Vector2 coordinates = _itemList[i].ItemGenCoordinate(i, mainPlayer, areaInt);

							_itemList[i].ItemCoordinates = (coordinates);
						}
					}
				}

				mainID.NewItemList = false;
			}
		}



		/// checkExitManager
		/// Method that checks whether the player is colliding with an exit rectangle.
		/// Returns string including travel direction.
		public override string checkExitManager(Rectangle playerRect, Vector2 Section)
		{
			if (_DunY != 0 && _BossLevelChangeDirection != 'U') // checks there is no boss level in travel direction and that Dun coordinate is allowed.
			{
				if (playerRect.Intersects(TopExit))
				{
					return "EXIT U";
				}
			}

			if (_DunY != 3 && _BossLevelChangeDirection != 'D') // checks there is no boss level in travel direction and that Dun coordinate is allowed.
			{
				if (playerRect.Intersects(BottomExit))
				{
					return "EXIT D";
				}
			}

			if (_DunX != 0 && _BossLevelChangeDirection != 'L') // checks there is no boss level in travel direction and that Dun coordinate is allowed.
			{
				if (playerRect.Intersects(LeftExit))
				{
					return "EXIT L";
				}
			}

			if (_DunX != 3 && _BossLevelChangeDirection != 'R') // checks there is no boss level in travel direction and that Dun coordinate is allowed.
			{
				if (playerRect.Intersects(RightExit))
				{
					return "EXIT R";
				}
			}

			return "";
		}


		/// checkDunExitManager
		/// Returns a boolean value whether the player's rectangle has intersected with the exit rectangle.
		public override bool checkDunExitManager(Rectangle playerRect)
		{
			Rectangle ExitRect = new Rectangle(0, 700, 768, 22); // Exit rectangle is static for all dungeons.

			if (_DunX == 2 && _DunY == 3)
			{
				if (playerRect.Intersects(ExitRect))
				{
					return true;
				}
			}

			return false;
		}


		/// A METHOD FOR CHECKING WHERE ENTRANCES TO THE BOSS BATTLES ARE.
		/// This is denoted by a collision with the exit rectangle and the change direction.
		public override bool checkBossLevelEntranceManager(Rectangle PlayerRect)
		{

			if (_BossLevelEntrance == true)
			{

				if (_BossLevelChangeDirection == 'U')
				{
					if (PlayerRect.Intersects(TopExit))
					{

						return true;
					}
				}

				if (_BossLevelChangeDirection == 'D')
				{
					if (PlayerRect.Intersects(BottomExit))
					{
						return true;
					}
				}

				if (_BossLevelChangeDirection == 'L')
				{
					if (PlayerRect.Intersects(LeftExit))
					{
						return true;
					}
				}

				if (_BossLevelChangeDirection == 'R')
				{
					if (PlayerRect.Intersects(RightExit))
					{
						return true;
					}
				}
			}

			return false;
		}





		/// UPDATE METHOD FOR DUNGEONMANAGER
		public override void update()
		{

			int changeSpeed = 12;

			if (_changeSec == true)
			{

				if (_changeDirection == 'R') // If the player is walking into a right section
				{

					_DefaultPos.X -= changeSpeed; // slide animation purposes
					_DefaultNewPosR.X -= changeSpeed;

					if (_DefaultNewPosR.X <= 0)
					{
						_changeSec = false;
					}
				}

				if (_changeDirection == 'L') // If the player is walking into a left section
				{
					_DefaultPos.X += changeSpeed;
					_DefaultNewPosL.X += changeSpeed;

					if (_DefaultPos.X >= 768)
					{
						_changeSec = false;
					}
				}

				if (_changeDirection == 'U') // If the player is walking into a upwards section
				{
					_DefaultPos.Y += changeSpeed;
					_DefaultNewPosU.Y += changeSpeed;

					if (_DefaultPos.Y >= 720)
					{
						_changeSec = false;
					}
				}

				if (_changeDirection == 'D') // If the player is walking into a downwards section
				{
					_DefaultPos.Y -= changeSpeed;
					_DefaultNewPosD.Y -= changeSpeed;

					if (_DefaultPos.Y <= -527)
					{
						_changeSec = false;
					}
				}
			}
			else {
				_DefaultPos = new Vector2(0, 96);
				_DefaultNewPosR = new Vector2(768, 96);
				_DefaultNewPosL = new Vector2(-767, 96);
				_DefaultNewPosU = new Vector2(0, -527);
				_DefaultNewPosD = new Vector2(0, 719);
			}
		}




		/// Draw
		public override void Draw(SpriteBatch spriteBatch, Player mainPlayer, InformationDisplay mainID)
		{

			if (_changeSec == true)
			{
				mainPlayer.DrawWeapon = false;
				mainID.NewEnemyList = true;
				mainID.EraseList = true;


				if (_changeDirection == 'R') // If the player is walking into a right section
				{
					spriteBatch.Draw(PreDungeonBack, new Vector2(_DefaultPos.X, _DefaultPos.Y)); // draw the previous dungeon section
					spriteBatch.Draw(DungeonBack, new Vector2(_DefaultNewPosR.X, _DefaultNewPosR.Y)); // draw the new dungeon section
				}

				if (_changeDirection == 'L') // If the player is walking into a left section
				{
					spriteBatch.Draw(PreDungeonBack, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(DungeonBack, new Vector2(_DefaultNewPosL.X, _DefaultNewPosL.Y));
				}

				if (_changeDirection == 'U') // If the player is walking into a upwards section
				{
					spriteBatch.Draw(PreDungeonBack, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(DungeonBack, new Vector2(_DefaultNewPosU.X, _DefaultNewPosU.Y));
				}

				if (_changeDirection == 'D') // If the player is walking into a downwards section
				{
					spriteBatch.Draw(PreDungeonBack, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(DungeonBack, new Vector2(_DefaultNewPosD.X, _DefaultNewPosD.Y));
				}


				// SLIDING ANIMATION
				// When the player moves into a new section, the new section will slide into place in the opposite direction the player is travelling.

				int x = 0;

				// Set default X values.
				if (_changeDirection == 'U')
				{
					x = (int)_DefaultNewPosU.X;
				}

				if (_changeDirection == 'D')
				{
					x = (int)_DefaultNewPosD.X;
				}

				if (_changeDirection == 'L')
				{
					x = (int)_DefaultNewPosL.X;
				}

				if (_changeDirection == 'R')
				{
					x = (int)_DefaultNewPosR.X;
				}

				int y = 0;

				// For loop to go through structure array, if part of array is true then draw.
				for (int row = 0; row < 16; row++)
				{
					// Set default Y values.
					if (_changeDirection == 'U')
					{
						y = (int)_DefaultNewPosU.Y;
					}

					if (_changeDirection == 'D')
					{
						y = (int)_DefaultNewPosD.Y;

					}

					if (_changeDirection == 'L')
					{
						y = (int)_DefaultNewPosL.Y;
					}

					if (_changeDirection == 'R')
					{
						y = (int)_DefaultNewPosR.Y;
					}

					for (int column = 0; column < 13; column++)
					{
						if (_Structure[row, column] == true)
						{
							spriteBatch.Draw(Brick, new Vector2(x, y));
						}
						y += 48;
					}
					x += 48;
				}


				// Draw the position of the old section, works in the same manner as the new section animation.
				x = (int)_DefaultPos.X;

				y = 0;

				for (int row = 0; row < 16; row++)
				{
					y = (int)_DefaultPos.Y;

					for (int column = 0; column < 13; column++)
					{
						if (_PreviousStructure[row, column] == true)
						{
							spriteBatch.Draw(Brick, new Vector2(x, y));
						}
						y += 48;
					}

					x += 48;
				}
			}

			// Draw dungeon section while slide animation is not running
			else {

				spriteBatch.Draw(DungeonBack, new Vector2(0, 96));

				int x = 0;
				int y;

				for (int row = 0; row < 16; row++)
				{
					y = 96;
					for (int column = 0; column < 13; column++)
					{
						if (_Structure[row, column] == true)
						{
							spriteBatch.Draw(Brick, new Vector2(x, y));
						}
						y += 48;
					}

					x += 48;
				}
			}

		}
	}
}