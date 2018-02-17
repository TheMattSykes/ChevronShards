using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	public interface AreaInterface
	{ // set the structure for the Area class.
		List<Rectangle> CollisionRects { get; }

		List<EnemyInterface> EnemyList { get; }

		List<Rectangle> EnemyRects { get; }

		List<ItemInterface> ItemList { get; }

		List<Rectangle> ItemRects { get; }

		bool ChangeSec { get; set; }

		char ChangeDirection { get; set; }

		void GenerateStructure(params int[] values);

		void GeneratePreStructure(params int[] values);

		void GenerateRectangleCollisions();

		void LoadContent(ContentManager Content);

		bool checkMoveManager(Rectangle entityRect);

		string checkExitManager(Rectangle playerRect, Vector2 Section);

		void GenerateEnemies(Rectangle playerRect, InformationDisplay mainID);

		void GenerateItems(Player mainPlayer, AreaInterface areaInt, InformationDisplay mainID);

		bool checkDunEntranceManager(Vector2 playerPos, int playerHeight, int playerWidth, ref int DungeonNumber);

		bool checkDunExitManager(Rectangle playerRect);

		bool checkBossLevelEntranceManager(Rectangle PlayerRect);

		void SetBossLevelNumber(int num);

		int GetBossLevelNumber();

		bool CheckCompletedDungeon();

		void update();

		void Draw(SpriteBatch spriteBatch, Player mainPlayer, InformationDisplay mainID);
	}

	public abstract class Area : AreaInterface
	{ // this class will not be directly instantiated.
		
		protected bool[,] _Structure = new bool[16, 13]; // Structure of the area.
		protected bool[,] _PreviousStructure = new bool[16, 13]; // Section of the area the player is moving away from.

		protected List<Rectangle> _collisionRects = new List<Rectangle>(); // List stores rectangles, these are used for collision detection.
		public List<Rectangle> CollisionRects { get { return _collisionRects; } }

		protected List<EnemyInterface> _enemyList = new List<EnemyInterface>(); // List of enemies through the interface, requires dynamic data structure as there is no maximum.
		public List<EnemyInterface> EnemyList { get { return _enemyList; } }

		protected List<Rectangle> _enemyRects = new List<Rectangle>(); // List of rectangles around the enemies, also used for collision detection.
		public List<Rectangle> EnemyRects { get { return _enemyRects; } }

		protected List<ItemInterface> _itemList = new List<ItemInterface>(); // List of items through the interface, requires dynamic data structure as there is no maximum.
		public List<ItemInterface> ItemList { get { return _itemList; } }

		protected List<Rectangle> _itemRects = new List<Rectangle>(); // List of rectangles around the items, also used for collision detection so items can be collected.
		public List<Rectangle> ItemRects { get { return _itemRects; } }


		protected bool _changeSec; // Is the player changing which section they are in.
		public bool ChangeSec { get { return _changeSec; } set { _changeSec = value; } }

		protected char _changeDirection; // Which direction is the next section in.
		public char ChangeDirection { get { return _changeDirection; } set { _changeDirection = value; } }


		protected int _enemyAmount;
		protected string _enemyType;

		// Vector positions denoting the starting position of overworld slide animations depending on direction
		protected Vector2 _DefaultPos = new Vector2(0, 96);
		protected Vector2 _DefaultNewPosR = new Vector2(768, 96);
		protected Vector2 _DefaultNewPosL = new Vector2(-767, 96);
		protected Vector2 _DefaultNewPosU = new Vector2(0, -527);
		protected Vector2 _DefaultNewPosD = new Vector2(0, 719);

		// Common Rectangles which are used to prevent exit from the sections not leading to another section.
		protected Rectangle TopSide = new Rectangle(0, 96, 768, 1);
		protected Rectangle BottomSide = new Rectangle(0, 719, 768, 1);
		protected Rectangle LeftSide = new Rectangle(0, 96, 1, 720);
		protected Rectangle RightSide = new Rectangle(767, 0, 1, 720);

		// Rectangles which if collided with will move into the section denoted by direction in name.
		protected Rectangle TopExit = new Rectangle(0, 96, 768, 20);
		protected Rectangle BottomExit = new Rectangle(0, 700, 768, 20);
		protected Rectangle LeftExit = new Rectangle(0, 96, 20, 720);
		protected Rectangle RightExit = new Rectangle(748, 0, 20, 720);


		public virtual void GenerateStructure(params int[] values) { } // inherited function, called from the AreaInterface version.

		public virtual void GeneratePreStructure(params int[] values) { } // inherited function, called from the AreaInterface version.


		/// GenerateRectangleCollisions
		/// Uses a nested for loop to go through 2D array, if there is a 48x48 block in the structure of the section
		/// then the program will add a rectangle of that side to a list; this can later denote collision detection.
		public virtual void GenerateRectangleCollisions()
		{

			_collisionRects.Clear(); // clear if regenerating

			_collisionRects.Add(TopSide);
			_collisionRects.Add(BottomSide);
			_collisionRects.Add(LeftSide);
			_collisionRects.Add(RightSide);

			int x = 0;
			int y;

			for (int row = 0; row < 16; row++)
			{
				y = 96;

				for (int column = 0; column < 13; column++)
				{
					if (_Structure[row, column] == true)
					{
						_collisionRects.Add(new Rectangle(x, y, 48, 48)); // add 48x48 block to list as denoted by xy coordinates.
					}
					y += 48;
				}
				x += 48;
			}
		}


		/// checkMoveManager
		/// Goes through collisionRects list and if the player intersects that rectange then returns true.

		public bool checkMoveManager(Rectangle entityRect)
		{


			for (int i = 0; i < _collisionRects.Count; i++)
			{
				if (entityRect.Intersects(_collisionRects[i]))
				{

					return true;
				}
			}

			return false;
		}


		/// checkExitManager
		/// A METHOD FOR CHECKING WHERE THE EXITS TO EACH PART OF THE OVERWORLD ARE.

		public virtual string checkExitManager(Rectangle playerRect, Vector2 Section)
		{
			if (playerRect.Intersects(BottomExit))
			{
				return "EXIT D";
			}
			if (playerRect.Intersects(LeftExit))
			{
				return "EXIT L";
			}
			if (playerRect.Intersects(RightExit))
			{
				if (Section.X > 3) // some areas do not feature boundaries, this prevents exit from the overworld.
				{
					return null;
				}

				return "EXIT R";
			}
			if (playerRect.Intersects(TopExit))
			{
				if (Section.Y < 1) // some areas do not feature boundaries, this prevents exit from the overworld.
				{
					return null;
				}

				return "EXIT U";
			}

			return null;
		}




		public virtual bool checkDunEntranceManager(Vector2 playerPos, int playerHeight, int playerWidth, ref int DungeonNumber)
		{
			return false; // no dungeon entrances present by default
		}


		/// GenerateEnemies
		/// Uses values gained previously from the text file and adds them to the enemyList.
		public virtual void GenerateEnemies(Rectangle playerRect, InformationDisplay mainID)
		{
			if (mainID.EraseList == true)
			{
				_enemyList.Clear();
				_itemRects.Clear();
				_enemyRects.Clear();
				mainID.EraseList = false;
			}

			if (_enemyAmount > 0)
			{
				if (mainID.NewEnemyList == true)
				{
					_enemyList.Clear();
					_itemRects.Clear();

					Random R = new Random();
					int a = R.Next(_enemyAmount, _enemyAmount + 1); // random amount of enemies between that stated in the text file and +1.

					for (int i = 0; i <= a; i++)
					{
						int orientationNum = R.Next(0, 3); // generate random number between 0 and 3.

						if (_enemyType == "Leafen")
						{
							_enemyList.Add(new Leafen()); // add new enemy to the enemyList.
						}
						if (_enemyType == "Firmeleon")
						{
							_enemyList.Add(new Firmeleon());
						}

						if (_enemyType == "Seafen")
						{
							_enemyList.Add(new Seafen());
						}

						// Random number generated denotes what direction the enemy will face.
						if (orientationNum == 0) { _enemyList[i].Orientation = 'R'; }
						if (orientationNum == 1) { _enemyList[i].Orientation = 'L'; }
						if (orientationNum == 2) { _enemyList[i].Orientation = 'U'; }
						if (orientationNum == 3) { _enemyList[i].Orientation = 'D'; }

					}

					for (int i = 0; i < _enemyList.Count; i++)
					{
						// Generate (within bounds) random coordinates for the enemy.
						Vector2 coordinates = _enemyList[i].EnemyGenCoordinate(_enemyList[i].Width, _enemyList[i].Height, playerRect, _collisionRects, ref _itemRects);
						_enemyList[i].EnemyCoordinates = coordinates;
					}

					mainID.NewEnemyList = false;
				}
			}
		}

		public virtual void GenerateItems(Player mainPlayer, AreaInterface areaInt, InformationDisplay mainID) { }

		public virtual bool checkDunExitManager(Rectangle playerRect) { return false; }

		public virtual bool checkBossLevelEntranceManager(Rectangle PlayerRect) { return false; }

		public virtual void SetBossLevelNumber(int num) { }

		public virtual int GetBossLevelNumber() { return 0; }

		public virtual bool CheckCompletedDungeon() { return false; }

		public virtual void LoadContent(ContentManager Content) { }

		public virtual void update(){ }

		public virtual void Draw(SpriteBatch spriteBatch, Player mainPlayer, InformationDisplay mainID) { }
	}
}