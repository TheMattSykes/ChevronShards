using System;
using Microsoft.Xna.Framework;

namespace ChevronShards
{
    class Firmeleon : Enemy
    {
        public Firmeleon()
        {
			// Set default values
            _Type = "Firmeleon";
            _Health = 10;
            _HealthMax = 10;
            _Speed = 3;
            _Width = 36;
            _Height = 36;
            _WeaponType = "FireBall";

            _EnemyWeapon = new FireBall(); // Instantiate static enemy weapon
        }

        public override string GetEnemyTextureName()
        {
			// Return the string name of the enemies texture depending on the direction it is facing.
            if (_orientation == 'L')
            {
                return "Firmeleon";
            }
            else
            {
                return "Firmeleon_Right";
            }
        }

        public override void Update(GameTime gameTime, Random R, bool checkMove, bool checkEnemyAndPlayerCollision, Player mainPlayer, int eGameTime)
        {
            if (_EnemyWeapon.WeaponFireTime >= (_EnemyWeapon.WeaponFireTimeMax + 800) || _EnemyWeapon.WeaponFireTimeMax == 0) // Reset weapon
            {
                _EnemyWeapon.WeaponFireTimeMax = R.Next(1000, 3000);  // Maximum time the weapon will fire

                _EnemyWeapon.SetWeaponCoordinates(_EnemyCoordinates); // Set the weapon coordinates to the position of the enemy
                _EnemyWeapon.SetWeaponOrientation(_orientation); // Set the direction of the weapon as the same as the enemy

                _EnemyWeapon.WeaponFireTime = 0;
            }

			if (_EnemyWeapon.WeaponFireTime >= 800) // After 800ms
			{
				_DrawWeapon = false; // Stop Drawing Weapon
			}

            /// Enemy movement depending on the direction the enemy is facing
            /// 
            if (_AllowDirChange == true && _AllowMovement == true && checkMove == false && checkEnemyAndPlayerCollision == false && _DrawCoordinates == _EnemyCoordinates)
            {
                if (_IsMoving == true) // If the enemy is moving
                {
					// Set the XY coordaintes (vector) of the enemy depeding on which direction it is facing
                    if (_orientation == 'R')
                    {
                        int newX = (int)_EnemyCoordinates.X + _Speed;
                        int newY = (int)_EnemyCoordinates.Y;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }
                    if (_orientation == 'L')
                    {
                        int newX = (int)_EnemyCoordinates.X - _Speed;
                        int newY = (int)_EnemyCoordinates.Y;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }
                    if (_orientation == 'U')
                    {
                        int newX = (int)_EnemyCoordinates.X;
                        int newY = (int)_EnemyCoordinates.Y - _Speed;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }
                    if (_orientation == 'D')
                    {
                        int newX = (int)_EnemyCoordinates.X;
                        int newY = (int)_EnemyCoordinates.Y + _Speed;

                        _EnemyCoordinates = (new Vector2(newX, newY));
                    }
                }
            }

            Rectangle EnemyDrawRectangle = new Rectangle((int)_EnemyCoordinates.X, (int)_DrawCoordinates.Y, _Width, _Height); // Draw rectangle around enemy

            ///
            /// Enemy Weapon Collision Detection and Reaction
            ///
            if (EnemyDrawRectangle.Intersects(mainPlayer.PlayerWeapon.GetWeaponRect()) == true && mainPlayer.PlayerWeaponFiring == true)
            { // If the enemy collides with the player's weapon
                _BeenHit = true;

                int EnemyHealth = _Health;

                if (eGameTime == 0) // If the enemy game time is 0
                {
					// Different levels of damage to enemy health depending on weapon used
					if (mainPlayer.CurrentWeapon == "FireBall")
					{
						_Health = (EnemyHealth - 4);
					}
					if (mainPlayer.CurrentWeapon == "WaterBall")
					{
						_Health = (EnemyHealth - 10);
					}
					if (mainPlayer.CurrentWeapon == "Seed")
					{
						_Health = (EnemyHealth - 1);
					}
					if (mainPlayer.CurrentWeapon == "Sword")
					{
						_Health = (EnemyHealth - 5);
					}
                }

                _EnemyHitTime = (eGameTime + gameTime.ElapsedGameTime.Milliseconds); // Increment enemy hit time.
            }
        }
	}
}