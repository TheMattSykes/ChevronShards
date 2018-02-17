using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ChevronShards
{
	public interface EnemyInterface
	{ 
		// Denote structure, methods and variables for the Enemy Class
		string Type { get; set; }
		int Speed { get; }
		bool AllowDirChange { get; set; }
		bool AllowMovement { get; set; }
		bool IsMoving { get; set; }
		Texture2D EnemyTexture { get; set; }
		string EnemyTextureName { set; }

		Rectangle EnemyRectangle { get; }
		WeaponInterface EnemyWeapon { get; }

		Vector2 EnemyCoordinates { get; set; }
		Vector2 DrawCoordinates { get; set; }
		int EnemyGameTime { get; set; }
		int EnemyHitTime { get; set; }
		bool BeenHit { get; set; }
		string WeaponType { get; }
		bool DrawWeapon { get; set; }

		char Orientation { get; set; }
		int Height { get; set; }
		int Width { get; set; }
		int Health { get; set; }
		int HealthMax { set; }

		Vector2 EnemyGenCoordinate(int EnemyHeight, int EnemyWidth, Rectangle playerRect, List<Rectangle> RectList, ref List<Rectangle> EnemyRects); // THIS NEEDS TO CHANGE!  !!!!!!!!!!!!!!!!!!!

		string GetEnemyTextureName();

		void entityReaction();

        void Update(GameTime gameTime, Random R, bool checkMove, bool checkEnemyAndPlayerCollision, Player mainPlayer, int eGameTime);
	}

	public abstract class Enemy : Entity, EnemyInterface // Inherit Entity and the EnemyInterface
	{
		protected string _Type; // The type of enemy such as a Leafen
		public string Type { get { return _Type; } set { _Type = value; } }

		protected int _Speed; // The speed the enemy travels at
		public int Speed { get { return _Speed; } }

		protected bool _AllowDirChange; // Allow the enemy to change direction
		public bool AllowDirChange { get { return _AllowDirChange; } set { _AllowDirChange = value; } }

		protected bool _AllowMovement; // Allow the enemy to move
		public bool AllowMovement { get { return _AllowMovement; } set { _AllowMovement = value; } }

		protected bool _IsMoving; // Is the eney moving
		public bool IsMoving { get { return _IsMoving; } set { _IsMoving = value; } }

		// Enemy Texture details
		protected Texture2D _EnemyTexture;
		public Texture2D EnemyTexture { get { return _EnemyTexture; } set { _EnemyTexture = value;} }

		protected string _EnemyTextureName; // Name of the enemy texture
		public string EnemyTextureName { set { _EnemyTextureName = value; } }


		protected Rectangle _EnemyRectangle; // Rectangle drawn around the enemy used for collision and interaction
		public Rectangle EnemyRectangle { get { return _EnemyRectangle = new Rectangle((int)_EnemyCoordinates.X, (int)_EnemyCoordinates.Y, _Width, _Height); } }


		protected WeaponInterface _EnemyWeapon; // WeaponInterface to store enemies wepaon
		public WeaponInterface EnemyWeapon { get { return _EnemyWeapon; } }


		protected Vector2 _EnemyCoordinates; // Proposed position of enemy
		public Vector2 EnemyCoordinates { get { return _EnemyCoordinates; } set { _EnemyCoordinates = value; } }

		protected Vector2 _DrawCoordinates; // Drawn position of enemy
		public Vector2 DrawCoordinates { get { return _DrawCoordinates; } set { _DrawCoordinates = value; } }


		protected int _EnemyGameTime; // Animation time of enemy
		public int EnemyGameTime { get { return _EnemyGameTime; } set { _EnemyGameTime = value; } }

		protected int _EnemyHitTime; // How long since the enemy was hit by a weapon 
		public int EnemyHitTime { get { return _EnemyHitTime; } set { _EnemyHitTime = value; } }

		protected bool _BeenHit;
		public bool BeenHit { get { return _BeenHit; } set { _BeenHit = value; } }

		protected string _WeaponType;
		public string WeaponType { get { return _WeaponType; } }

		protected bool _FiringWeapon; // Is the weapon being fired
		public bool FiringWeapon { get { return _FiringWeapon; } set { _FiringWeapon = value; } }

		protected bool _DrawWeapon;
		public bool DrawWeapon { get { return _DrawWeapon; } set { _DrawWeapon = value; } }

		// Update method inherited by types of enemy
		public virtual void Update(GameTime gameTime, Random R, bool checkMove, bool checkEnemyAndPlayerCollision, Player mainPlayer, int eGameTime){ }

		public Enemy() 
		{
			// Set default values
			_AllowDirChange = true;
			_AllowMovement = true;
			_IsMoving = false;
			_DrawWeapon = false;
		}

		/// <summary>
		/// This function generates random coordinates that the enemies can spawn in, while in a certain overworld section, the function will loop until
		/// there are no rectangle collisions. I.E. The rectangle around the enemy collides with the boundaries, enemies or the player.
		///
		public Vector2 EnemyGenCoordinate(int EnemyHeight, int EnemyWidth, Rectangle playerRect, List<Rectangle> RectList, ref List<Rectangle> EnemyRects)
		{
			Random R = new Random();
			Vector2 FinalCoordinates;
			bool useCoordinates = true; // assume the coordiantes can be used until program indicates otherwise

			while (true)
			{

				useCoordinates = true;

				// Denote range of X and Y values
				int X = R.Next(48, 720);
				int Y = R.Next(192, 576);
				Vector2 PossibleCoordinates = new Vector2(X, Y); // proposed coordinate variable

				Rectangle NewEnemyRect = new Rectangle(X, Y, EnemyWidth+30, EnemyHeight+30); // form a rectangle around the enemy for collision purposes, allows 10 pixels around it

				if (NewEnemyRect.Intersects(playerRect))
				{
					useCoordinates = false;
				}

				// For loop goes through all the enemy rectangles to make sure that two enemies don't spawn on top of each other
				for (int i = 0; i < EnemyRects.Count; i++)
				{
					if (NewEnemyRect.Intersects(EnemyRects[i]))
					{
						useCoordinates = false;
					}
				}

				// For loop to prevent spawning of enemies on other objects
				for (int i = 0; i < RectList.Count; i++)
				{
					if (NewEnemyRect.Intersects(RectList[i]))
					{
						useCoordinates = false;
					}
				}

				if (useCoordinates == true) // use the coordaintes is valid
				{
					FinalCoordinates = PossibleCoordinates;
					_DrawCoordinates = FinalCoordinates;
					break;
				}
			}

			EnemyRects.Add(new Rectangle((int)FinalCoordinates.X,(int)FinalCoordinates.Y,EnemyWidth,EnemyHeight)); // Add the enemy rectangle to the list of enemy rectangles
			return FinalCoordinates;
		}

		// Enemy texture name returned as string, used in LoadContent.
		public virtual string GetEnemyTextureName()
		{
			return _EnemyTextureName;
		}
	}
}