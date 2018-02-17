using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;


namespace ChevronShards
{
	public class OverworldManager : Area
	{
		/*
		 * OVERWORLD MANAGER CLASS
		 * This class is for managing all the sections of the overworld
		 * 
		 * EnemyList and ItemList are properties of this class.
		 */

		private Texture2D OverworldBackground;
		private Texture2D PreOverworldBackground;
		private Texture2D Tree;
		private Texture2D Tree_Brown;
		private Texture2D Desert_Plant;
		private Texture2D DungeonExt;

		// allows the LoadContent method to find the name of the texture.
		private string _BackgroundTexName;
		private string _preBackgroundTexName;
		private string _DunExtTexName;
		private string _SectionType;
		private string _PreSectionType;

		// Stores X and Y coordinates of Section used for text file.
		private int _SecX;
		private int _SecY;

		private bool _HasDungeonExterior; // does section have a dungeon exterior.
		private int _DungeonNumber;
		private Vector2 _DungeonExtPos;

		// Store item information from text file.
		private string _ItemTypeA;
		private string _ItemTypeB;
		private int _ItemAAmount;
		private int _ItemBAmount;

		private bool _animationRunning = false;

		private bool _ChangeToDungeon;



		/// LoadContent
		/// Load content data into Texture variables.
		public override void LoadContent(ContentManager Content) 
		{

			OverworldBackground = Content.Load<Texture2D>(GetBackgroundTexName());

			if (_changeSec == true) // Only need texture if the player is changing sections.
			{
				PreOverworldBackground = Content.Load<Texture2D>(GetPreBackgroundTexName());
			}
			Tree = Content.Load<Texture2D>("Tree");
			Tree_Brown = Content.Load<Texture2D>("Brown Tree");

			Desert_Plant = Content.Load<Texture2D>("Desert Plant");

			if (HasDungeonExt() == true)
			{
				DungeonExt = Content.Load<Texture2D>(GetDungeonExtTextureName());
			}
		}



		/// GetSectionType
		/// Returns what type of section teh section is e.g. Forest or Desert.
		public string GetSectionType() {
			return _SectionType;
		}

		/// GetPreviousSectionType
		/// Returns what type of section the previous section is is e.g. Forest or Desert.
		public string GetPreviousSectionType()
		{
			return _PreSectionType; // What type was section before moving into new section.
		}

		/// GetBackgroundTexName
		/// Returns (depending on type) the texture name for the background of the overworld.
		public string GetBackgroundTexName()
		{

			if (_SectionType == "Forest") {
				return "OverworldBackground1";
			}

			if (_SectionType == "DryForest")
			{
				return "OverworldBackground2";
			}

			if (_SectionType == "Riverside")
			{
				return "OverworldBackground3";
			}

			if (_SectionType == "Desert")
			{
				return "OverworldBackground4";
			}

			return null;

		}

		/// GetPreBackgroundTexName
		/// Returns (depending on type) the texture name for the background of the overworld used for the previous section.
		public string GetPreBackgroundTexName()
		{

			if (_PreSectionType == "Forest")
			{
				return "OverworldBackground1";
			}

			if (_PreSectionType == "DryForest")
			{
				return "OverworldBackground2";
			}

			if (_PreSectionType == "Riverside")
			{
				return "OverworldBackground3";
			}

			if (_PreSectionType == "Desert")
			{
				return "OverworldBackground4";
			}

			return null;
		}


		/// HasDungeonExterior
		/// Returns whether the overworld section contains a dungeon exterior.
		public bool HasDungeonExt() {
			return _HasDungeonExterior;
		}

		/// GetDungeonTextureName
		/// Return texture name for the dungeon exterior.
		public string GetDungeonExtTextureName() 
		{
			return ("DungeonExterior"+(char)_DungeonNumber);
		}

		/// GetItemType
		/// Returns the name of the item after using the parameter of a char from the text file.
		public string GetItemType(char c) {

			if (c == 'C') {
				return "Coin";
			}

			if (c == 'H')
			{
				return "Heart";
			}


			return "";
		}


		/// Overworld Section Generator
		/// Reads from the relevent Dungeon Section text file, converts each line into usable information as commented below.
		/// The file will denote the structure of the overworld, the file writes the structure in rows and columns under binary form.
		public override void GenerateStructure(params int[] values)
		{

			_SecX = values[0];
			_SecY = values[1];

			_HasDungeonExterior = false;

			string line;
			string FileName = "Overworld_" + _SecX + "_" + _SecY + ".txt";

			int Count = 0;

			int Row = 0;
			int Column = 0;

 			StreamReader Reader = new StreamReader(FileName);

			while ((line = Reader.ReadLine()) != null)
			{

				if (Count == 0) // First Line denotes what type of overworld section it is.
				{
					_SectionType = line;
				}

				if (Count == 1) // Second Line states the type of Enemy with a character and then a number 0-9 of how many there are.
				{
					if (line[0] == 'N') // No enemies
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

				if (Count == 2) // Third Line states the type of Item with a character and then a number 0-9 of how many there are.
				{
					if (line[0] == 'N') // No items present in section.
					{
						_itemList.Clear();
						_itemRects.Clear();

						_ItemTypeA = null;

						_ItemAAmount = 0;
					}

					if (line[0] != 'N'){
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

				if (Count == 3) // Fourth Line denotes if a dungeon entrance is present folled by 2x 3 digit coordinates.
				{

					if (line[0] != 'N') // If text file denotes there is a dungeon extrance present.
					{
						_HasDungeonExterior = true;

						_DungeonNumber = line[0];

						char[] StringFormerX = { line[1], line[2], line[3] };

						string ExteriorXCo = new String(StringFormerX);

						char[] StringFormerY = { line[4], line[5], line[6] };
						string ExteriorYCo = new String(StringFormerY);

						_DungeonExtPos = new Vector2(Convert.ToInt32(ExteriorXCo),Convert.ToInt32(ExteriorYCo));
					}
				}

				// setup
				if (Count >= 4) // The Final Lines denote the structure of the overworld with a 1 indicating a block e.g. a tree is present, or a 0 for a blank space.
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

		}


		/// GeneratePreStructure
		/// Does the same thing as the GenerateStruture method but instead stores data/values for the previous section.
		/// Some parts of the file are ommitted as they are not relevent to the sliding animation.
		public override void GeneratePreStructure(params int[] values) // for sliding animation
		{
			int PrevX = values[0];
			int PrevY = values[1];

			string line;
			string FileName = "Overworld_" + PrevX + "_" + PrevY + ".txt";

			int Count = 0;

			int Row = 0;
			int Column = 0;

			StreamReader Reader = new StreamReader(FileName);

			while ((line = Reader.ReadLine()) != null)
			{

				if (Count == 0) // First Line denotes what type of overworld section it is.
				{
					_PreSectionType = line;
				}

				if (Count >= 4) // The Final Lines denote the structure of the overworld with a 1 indicating a block e.g. a tree is present, or a 0 for a blank space.
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
								if (mainPlayer.VisitedOWSections[(int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y] == false) // if section has not been visited
								{
									_itemList.Add(new Coin()); // add Coin object to list
								}
								else {
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
								_itemList[i].DrawItem = true;
							}
						}

						for (int i = 0; i < _itemList.Count; i++)
						{
							// Generate random coordinate in bounds.
							Vector2 coordinates = _itemList[i].ItemGenCoordinate(i,mainPlayer,areaInt);

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
								if (mainPlayer.VisitedOWSections[(int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y] == false) // if section has not been visited
								{
									_itemList.Add(new Coin()); // add Coin object to list
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
								_itemList[i].DrawItem = true;
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


		public override void GenerateRectangleCollisions()
		{
			base.GenerateRectangleCollisions(); // run base version of method

			// Some overworld sections have dungeon exteriors, this part of the method adds the collisions to the reactangle list.
			if (_HasDungeonExterior == true) {
				_collisionRects.Add(new Rectangle((int)_DungeonExtPos.X,(int)_DungeonExtPos.Y,288,109)); // Top of dungeon exterior
				_collisionRects.Add(new Rectangle((int)_DungeonExtPos.X, (int)_DungeonExtPos.Y+109, 106, 83)); // Left of dungeon exterior
				_collisionRects.Add(new Rectangle((int)_DungeonExtPos.X+180, (int)_DungeonExtPos.Y+109, 106, 83)); // Right of dungeon exterior
			}
		}




		// A METHOD FOR CHECKING WHERE THE EXITS TO EACH PART OF THE OVERWORLD ARE.

		public override bool checkDunEntranceManager(Vector2 playerPos, int playerHeight, int playerWidth, ref int DungeonNumber)
		{
			if (_HasDungeonExterior == true)
			{
				Rectangle DungeonEntranceRect = new Rectangle((int)_DungeonExtPos.X + 106, (int)_DungeonExtPos.Y + 109, 74, 84);
				Rectangle TempPlayerRectangle = new Rectangle((int)playerPos.X, (int)playerPos.Y + 40, playerWidth, playerHeight);

				if (TempPlayerRectangle.Intersects(DungeonEntranceRect))
				{
					DungeonNumber = _DungeonNumber - 48;
					return true;
				}
			}

			return false;
		}


		/// Update
		public override void update()
		{
			int changeSpeed = 12;

			if (_changeSec == true)
			{
				if (_changeDirection == 'R') // If the player is walking into a right section
				{
					_DefaultPos.X -= changeSpeed; // animation purposes
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
			else { // default poisitons used to draw new sections when the sliding animation is running.
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
					spriteBatch.Draw(PreOverworldBackground, new Vector2(_DefaultPos.X, _DefaultPos.Y)); // draw the previous overworld section
					spriteBatch.Draw(OverworldBackground, new Vector2(_DefaultNewPosR.X, _DefaultNewPosR.Y)); // draw the new overworld section
				}

				if (_changeDirection == 'L') // If the player is walking into a left section
				{
					spriteBatch.Draw(PreOverworldBackground, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(OverworldBackground, new Vector2(_DefaultNewPosL.X, _DefaultNewPosL.Y));
				}

				if (_changeDirection == 'U') // If the player is walking into a upwards section
				{
					spriteBatch.Draw(PreOverworldBackground, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(OverworldBackground, new Vector2(_DefaultNewPosU.X, _DefaultNewPosU.Y));
				}

				if (_changeDirection == 'D') // If the player is walking into a downwards section
				{
					spriteBatch.Draw(PreOverworldBackground, new Vector2(_DefaultPos.X, _DefaultPos.Y));
					spriteBatch.Draw(OverworldBackground, new Vector2(_DefaultNewPosD.X, _DefaultNewPosD.Y));
				}


				// SLIDING ANIMATION
				// When the player moves into a new section, the new section will slide into place in the opposite direction the player is travelling.

				int x = 0;

				// Set default X values.
				if (_changeDirection == 'U') {
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
							// Draw different graphic depending on section type.
							if (_SectionType == "Forest" || _SectionType == "Riverside")
							{
								spriteBatch.Draw(Tree, new Vector2(x, y));
							}

							if (_SectionType == "DryForest")
							{
								spriteBatch.Draw(Tree_Brown, new Vector2(x, y));
							}

							if (_SectionType == "Desert")
							{
								spriteBatch.Draw(Desert_Plant, new Vector2(x, y));
							}
						}
						y += 48;
					}
					x += 48; // each graphic size 48.
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
							if (_PreSectionType == "Forest" || _PreSectionType == "Riverside")
							{
								spriteBatch.Draw(Tree, new Vector2(x, y));
							}

							if (_PreSectionType == "DryForest")
							{
								spriteBatch.Draw(Tree_Brown, new Vector2(x, y));
							}

							if (_PreSectionType == "Desert")
							{
								spriteBatch.Draw(Desert_Plant, new Vector2(x, y));
							}
						}
						y += 48;
					}

					x += 48;
				}

			}

			// Draw overworld section while slide animation is not running
			else {

				spriteBatch.Draw(OverworldBackground, new Vector2(0, 96));

				int x = 0;
				int y;

				for (int row = 0; row < 16; row++)
				{
					y = 96;
					for (int column = 0; column < 13; column++)
					{
						if (_Structure[row, column] == true)
						{
							if (_SectionType == "Forest" || _SectionType == "Riverside")
							{
								spriteBatch.Draw(Tree, new Vector2(x, y));
							}

							if (_SectionType == "DryForest")
							{
								spriteBatch.Draw(Tree_Brown, new Vector2(x, y));
							}

							if (_SectionType == "Desert")
							{
								spriteBatch.Draw(Desert_Plant, new Vector2(x, y));
							}
						}
						y += 48;
					}

					x += 48;
				}

				if (_HasDungeonExterior == true) { 
					spriteBatch.Draw(DungeonExt, _DungeonExtPos); // draw dungeon exterior.
				}
			}

		}
	}
}
