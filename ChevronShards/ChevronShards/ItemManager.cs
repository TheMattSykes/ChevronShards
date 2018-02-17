using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	class ItemManager
	{

		// Load textures into Texture variables
		public void LoadContent(ContentManager Content, AreaInterface areaInt) 
		{
			List<ItemInterface> ItemList = areaInt.ItemList;

			if (ItemList.Count != 0) // If item list is not empty
			{
				for (int i = 0; i < ItemList.Count; i++) // cycle through Item list and allocate textures
				{
					ItemList[i].Texture = Content.Load<Texture2D>(ItemList[i].TextureName);
				}
			}
	
		}


		/// UPDATE METHOD

		public void update(AreaInterface areaInt, Player mainPlayer)
		{
			List<ItemInterface> ItemList = areaInt.ItemList;

			if (ItemList != null)
			{
				Rectangle playerRect = mainPlayer.Rect();

				for (int i = 0; i < ItemList.Count; i++)
				{ // Cycle throught Items list to check for collisions
					Rectangle ItemRect = new Rectangle((int)ItemList[i].ItemCoordinates.X, (int)ItemList[i].ItemCoordinates.Y, ItemList[i].Width, ItemList[i].Height);

					if (ItemRect.Intersects(playerRect)) { // If rectangles collide then run following logic

						if (ItemList[i].Name == "Coin")
						{
							mainPlayer.CoinCount = (mainPlayer.CoinCount + 1);
						}

						if (ItemList[i].Name == "Heart")
						{
							mainPlayer.Health = mainPlayer.Health + 15;
						}

						ItemList.RemoveAt(i);
					}
				}

			}

		}


		/// DRAW METHOD
		public void Draw(SpriteBatch spriteBatch, AreaInterface areaInt)
		{
			List<ItemInterface> ItemList = areaInt.ItemList;

			if (ItemList != null)
			{
				for (int i = 0; i < ItemList.Count; i++)
				{ // Cycle through Item List drawing their texture at their denoted coordinates
					if (ItemList.Count != 0)
					{
						spriteBatch.Draw(ItemList[i].Texture, ItemList[i].ItemCoordinates);
					}
				}
			}

		}
	}
}