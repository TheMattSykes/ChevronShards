using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{

	class EnemyManager
	{
		public void LoadContent(ContentManager Content, AreaInterface areaInt)
		{

			// ENEMY GRAPHICS AND ENEMY WEAPON GRAPHICS
			if (areaInt.EnemyList != null) // If the enemy list is not null load the enemy texture into the Texture variable for each enemy in the list.
			{
				if (areaInt.EnemyList.Count != 0)
				{
					for (int i = 0; i < areaInt.EnemyList.Count; i++) // 0 to enemyList size
					{
						areaInt.EnemyList[i].EnemyTexture = Content.Load<Texture2D>(areaInt.EnemyList[i].GetEnemyTextureName());

						// Set weapon texture based on object parameters
						areaInt.EnemyList[i].EnemyWeapon.WeaponTexture = Content.Load<Texture2D>(areaInt.EnemyList[i].EnemyWeapon.GetWeaponTextureName(areaInt.EnemyList[i].Orientation));
					}
				}
			}
		}


		/// EnemyCollision Method:
		/// A for loop goes through a list of enemies forming a rectangle with their coordinates and size, this is compared to another rectangle to see if a intersection occurs.

		public bool EnemyAndPlayerCollision(int ListValue, Player mainPlayer, AreaInterface areaInt) 
		{

			List<EnemyInterface> EnemyList = areaInt.EnemyList;
			Rectangle entityRect = (EnemyList[ListValue].EnemyRectangle);

			if (entityRect.Intersects(mainPlayer.Rect())) {
				return true; // Collision between enemy and player detected
			}

			for (int i = 0; i < EnemyList.Count; i++)
			{
				Rectangle CurrentRect = new Rectangle((int)areaInt.EnemyList[i].EnemyCoordinates.X, (int)EnemyList[i].EnemyCoordinates.Y, EnemyList[i].Width, EnemyList[i].Height);
				// Draw rectangle around current enemy in the list

				if (entityRect.Intersects(CurrentRect) == true && i != ListValue)
				{
					return true; // collision between two enemies detected
				}
			}
			return false; // no collision detected
		
		}




		public void Update(GameTime gameTime, AreaInterface areaInt, Player mainPlayer, InformationDisplay mainID)
		{

			Random R = new Random(); // Random generator

			for (int i = 0; i < areaInt.EnemyList.Count; i++) // goes through the EnemyList which has the objects of all the enimies in an area.
			{
				List<EnemyInterface> EnemyList = areaInt.EnemyList;

				Rectangle WeaponRect = EnemyList[i].EnemyWeapon.GetWeaponRect();


				/// Enemy Weapons

				if (EnemyList[i].EnemyWeapon.WeaponFireTimeMax != 0) // maximum weapon fire time cannot be 0
				{
					EnemyList[i].EnemyWeapon.WeaponFireTime = EnemyList[i].EnemyWeapon.WeaponFireTime + gameTime.ElapsedGameTime.Milliseconds; // Increment weapon fire time
				}

				if (areaInt.EnemyList[i].EnemyWeapon.WeaponFireTime > 0 && areaInt.EnemyList[i].EnemyWeapon.WeaponFireTime <= areaInt.EnemyList[i].EnemyWeapon.WeaponFireTimeMax)
				{
					areaInt.EnemyList[i].DrawWeapon = true; // Draw the enemies weapon

					// Set the weapon coordinates to the position of the enemy depending on direction.
					if (EnemyList[i].EnemyWeapon.GetWeaponOrientation() == 'U')
					{
						EnemyList[i].EnemyWeapon.SetWeaponCoordinates(new Vector2(areaInt.EnemyList[i].EnemyWeapon.GetWeaponCoordinates().X, EnemyList[i].EnemyWeapon.GetWeaponCoordinates().Y - 7));
					}

					if (EnemyList[i].EnemyWeapon.GetWeaponOrientation() == 'D')
					{
						EnemyList[i].EnemyWeapon.SetWeaponCoordinates(new Vector2(EnemyList[i].EnemyWeapon.GetWeaponCoordinates().X, EnemyList[i].EnemyWeapon.GetWeaponCoordinates().Y + 7));
					}

					if (EnemyList[i].EnemyWeapon.GetWeaponOrientation() == 'L')
					{
						EnemyList[i].EnemyWeapon.SetWeaponCoordinates(new Vector2(EnemyList[i].EnemyWeapon.GetWeaponCoordinates().X - 7, EnemyList[i].EnemyWeapon.GetWeaponCoordinates().Y));
					}

					if (EnemyList[i].EnemyWeapon.GetWeaponOrientation() == 'R')
					{
						EnemyList[i].EnemyWeapon.SetWeaponCoordinates(new Vector2(EnemyList[i].EnemyWeapon.GetWeaponCoordinates().X + 7, EnemyList[i].EnemyWeapon.GetWeaponCoordinates().Y));
					}
				}




				/// Enemy Out of bounds checker

				if (EnemyList[i].DrawCoordinates.X <= 0 && EnemyList[i].DrawCoordinates.X >= 768 && EnemyList[i].DrawCoordinates.Y <= 0 && EnemyList[i].DrawCoordinates.Y >= 720)
				{
					EnemyList.RemoveAt(i); // remove the enemy if it is outside the parameters (coordiante range) of the area
				}


				EnemyList[i].IsMoving = true; // Set the enemy to moving status


				// Rectangles drawn around the possible coordinates and the draw coordinates of the enemy
				Rectangle EnemyRectangle = new Rectangle((int)EnemyList[i].EnemyCoordinates.X, (int)EnemyList[i].EnemyCoordinates.Y, EnemyList[i].Width, EnemyList[i].Height);
				Rectangle EnemyDrawRectangle = new Rectangle((int)EnemyList[i].EnemyCoordinates.X, (int)EnemyList[i].DrawCoordinates.Y, EnemyList[i].Width, EnemyList[i].Height);

				Vector2 TempCoordinates = EnemyList[i].EnemyCoordinates; // allows new coordinates to be set in another class

				int eGameTime = EnemyList[i].EnemyHitTime;

				if (EnemyList[i].BeenHit == true) 
				{ 
					EnemyList[i].EnemyHitTime = (eGameTime + gameTime.ElapsedGameTime.Milliseconds);

					if (eGameTime > 200) // Recovery time over
					{
						EnemyList[i].EnemyHitTime = 0;
						eGameTime = 0;
						EnemyList[i].BeenHit = false;
					}
				}



				/// Enemy Logic and Movement
				bool AllowDirectionChange = EnemyList[i].AllowDirChange;
				bool AllowMovement = EnemyList[i].AllowMovement;

				// collision checker

				bool checkMove = false; // false for no collision


				checkMove = areaInt.checkMoveManager(EnemyRectangle); // check if enemy rectangle collides with area structure


				bool checkEnemyAndPlayerCollision = EnemyAndPlayerCollision(i, mainPlayer, areaInt); // check collision between enemy and player


				EnemyList[i].EnemyCoordinates = TempCoordinates; // Set the preliminary coordinates to the temporary coordinates (possible coordinates to use).

				if (checkMove == false && checkEnemyAndPlayerCollision == false) { EnemyList[i].DrawCoordinates = EnemyList[i].EnemyCoordinates; }
				if ((checkMove == true || checkEnemyAndPlayerCollision == true) && EnemyList[i].AllowDirChange == true) { EnemyList[i].entityReaction(); EnemyList[i].EnemyCoordinates = EnemyList[i].DrawCoordinates; }

				EnemyList[i].AllowDirChange = AllowDirectionChange; // (Dis)Allow the enemy to change direction
				EnemyList[i].AllowMovement = AllowMovement; // (Dis)Allow the enemy to move


				EnemyList[i].EnemyGameTime = (EnemyList[i].EnemyGameTime + gameTime.ElapsedGameTime.Milliseconds);


				if (AllowDirectionChange == true)
				{
					// Change direction at random time interval
					if (EnemyList[i].EnemyGameTime >= R.Next(1200,1500))
					{
						int orientationNum = R.Next(0, 4);
						if (orientationNum == 0) { EnemyList[i].Orientation = 'R'; }
						if (orientationNum == 1) { EnemyList[i].Orientation = 'L'; }
						if (orientationNum == 2) { EnemyList[i].Orientation = 'U'; }
						if (orientationNum == 3) { EnemyList[i].Orientation = 'D'; }

						EnemyList[i].EnemyGameTime = 0;
					}
				}




                EnemyList[i].Update(gameTime, R, checkMove, checkEnemyAndPlayerCollision, mainPlayer, eGameTime); // update each enemy in the list



				EnemyList[i].AllowDirChange = true;

				if (EnemyList[i].Health <= 0) // kill enemy if health <= 0
                {
                    mainID.EntityKilled = true;
                    mainID.KilledLocation = EnemyList[i].DrawCoordinates; // show kill animation
                    EnemyList.RemoveAt(i); // remove enemy from list
                }
			}
		}




		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, AreaInterface areaInt)
		{
			List<EnemyInterface> EnemyList = areaInt.EnemyList;

			if (EnemyList != null) // If the enemy list is not empty
			{
				for (int i = 0; i < EnemyList.Count; i++)
				{ // cycle through list of enemies and draw textures at denoted coordinates
					if (EnemyList[i].DrawWeapon == true)
					{
						spriteBatch.Draw(EnemyList[i].EnemyWeapon.WeaponTexture, EnemyList[i].EnemyWeapon.GetWeaponCoordinates());
					}


					if (EnemyList[i].EnemyHitTime < 1)
					{
						spriteBatch.Draw(EnemyList[i].EnemyTexture, EnemyList[i].DrawCoordinates);
					}
					else {
						spriteBatch.Draw(EnemyList[i].EnemyTexture, EnemyList[i].DrawCoordinates, color: Color.Red); // Red transparency superimposed on enemy when hit
					}
				}
			}
		}
	}
}