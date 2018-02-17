using System;
using Microsoft.Xna.Framework;

namespace ChevronShards
{
	class Boss : Enemy
	{

		public Boss()
		{
			// Set default values
			_Type = "Boss";
			_Speed = 1;
			_Width = 122;
			_Height = 119;
			_HealthMax = 200;
			_WeaponType = "FireBall";

			_EnemyWeapon = new FireBall(); // Instantiate static enemy weapon
		}

        public override void Update(GameTime gameTime, Random R, bool checkMove, bool checkEnemyAndPlayerCollision, Player mainPlayer, int eGameTime)
        {
            if (_EnemyWeapon.WeaponFireTime >= (_EnemyWeapon.WeaponFireTimeMax + 800) || _EnemyWeapon.WeaponFireTimeMax == 0) // Reset weapon
            {
                _EnemyWeapon.WeaponFireTimeMax = R.Next(1000, 1500); // Maximum time the weapon will fire

                _EnemyWeapon.SetWeaponCoordinates(_EnemyCoordinates); // Set the weapon coordinates to the position of the enemy
                _EnemyWeapon.SetWeaponOrientation(_orientation); // Set the direction of the weapon as the same as the enemy

                _EnemyWeapon.WeaponFireTime = 0;
            }

			if (_EnemyWeapon.WeaponFireTime >= 800) // After 800ms
			{
				_DrawWeapon = false; // Stop Drawing Weapon
			}


            /// Enemy movement depending on the direction the enemy is facing
            if (_AllowDirChange == true && _AllowMovement == true && checkMove == false && checkEnemyAndPlayerCollision == false && _DrawCoordinates == _EnemyCoordinates)
            {
                if (_IsMoving == true) // If the enemy is moving
                {
					// Move the enemy towards the player depending on position.
					// The program checks whether the XY coordinates are less or greater than the bosses coordiantes and moves in the opposite direction towards the player.
                    if (_EnemyCoordinates.X < mainPlayer.Rect().X) // If bosses coordinates < X position of player rectangle
                    {
                        int newX = (int)_EnemyCoordinates.X + _Speed;
                        int newY = (int)_EnemyCoordinates.Y;

                        _EnemyCoordinates = (new Vector2(newX, newY)); // Set coordaintes
                    }

                    if (_EnemyCoordinates.X > mainPlayer.Rect().X)
                    {
                        int newX = (int)_EnemyCoordinates.X - _Speed;
                        int newY = (int)_EnemyCoordinates.Y;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }

                    if (_EnemyCoordinates.Y < mainPlayer.Rect().Y)
                    {
                        int newX = (int)_EnemyCoordinates.X;
                        int newY = (int)_EnemyCoordinates.Y + _Speed;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }

                    if (_EnemyCoordinates.Y > mainPlayer.Rect().Y)
                    {
                        int newX = (int)_EnemyCoordinates.X;
                        int newY = (int)_EnemyCoordinates.Y - _Speed;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }
                }
            }


			Rectangle EnemyDrawRectangle = new Rectangle((int)_EnemyCoordinates.X, (int)_DrawCoordinates.Y, _Width, _Height); // Draw rectangle around the Boss



			/// Enemy Weapon Collision Detection and Reaction
			if (EnemyDrawRectangle.Intersects(mainPlayer.PlayerWeapon.GetWeaponRect()) == true && mainPlayer.PlayerWeaponFiring == true)
			{ // If the enemy collides with the player's weapon
				_BeenHit = true;

				int EnemyHealth = _Health;

				if (eGameTime == 0) // If the enemy game time is 0
				{
					// Different levels of damage to enemy health depending on weapon used
					if (mainPlayer.CurrentWeapon == "FireBall")
					{
						_Health = (EnemyHealth - 5);
					}
					if (mainPlayer.CurrentWeapon == "WaterBall")
					{
						_Health = (EnemyHealth - 12);
					}
					if (mainPlayer.CurrentWeapon == "Seed")
					{
						_Health = (EnemyHealth - 3);
					}
					if (mainPlayer.CurrentWeapon == "Sword")
					{
						_Health = (EnemyHealth - 8);
					}
				}

				_EnemyHitTime = (eGameTime + gameTime.ElapsedGameTime.Milliseconds); // Increment enemy hit time.
			}
        }
	}
}