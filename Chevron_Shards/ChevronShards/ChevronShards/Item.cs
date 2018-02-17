using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ChevronShards
{
	public interface ItemInterface
	{
		int Height { get; set; }
		int Width { get; set; }
		string Name { get; set; }
		Texture2D Texture { get; set; }
		string TextureName { get; set; }
		Rectangle ItemRect { get; set; }
		bool DrawItem { get; set; }
		Vector2 ItemCoordinates { get; set; }
		Vector2 ItemGenCoordinate(int ItemValue, Player mainPlayer, AreaInterface areaInt);
	}

	public class Item : ItemInterface
	{
		// Interface to denote structure, methods and parameters for Item abstract class

		protected int _Height;
		public int Height { get { return _Height; } set { _Height = value; } }

		protected int _Width;
		public int Width { get { return _Width; } set { _Width = value; } }

		protected string _Name; // Name of item
		public string Name { get { return _Name; } set { _Name = value; } }

		protected Texture2D _Texture;
		public Texture2D Texture { get { return _Texture; } set { _Texture = value; } }

		protected string _TextureName;
		public string TextureName { get { return _TextureName; } set { _TextureName = value; } }

		protected Rectangle _ItemRect; // Rectangle drawn around Item
		public Rectangle ItemRect { get { return _ItemRect; } set { _ItemRect = value; } }

		protected bool _DrawItem; // Should be item be drawn
		public bool DrawItem { get { return _DrawItem; } set { _DrawItem = value; } }

		protected Vector2 _ItemCoordinates;
		public Vector2 ItemCoordinates { get { return _ItemCoordinates; } set { _ItemCoordinates = value; } }



		public Vector2 ItemGenCoordinate(int ItemValue, Player mainPlayer, AreaInterface areaInt)
		{ // Generates random Item coordaintes which must not intersect with the area structure or player when spawned and returns them
			List<ItemInterface> ItemList = areaInt.ItemList; // List of items
			Rectangle playerRect = mainPlayer.Rect();
			List<Rectangle> RectList = areaInt.CollisionRects;
			List<Rectangle> ItemRects = areaInt.ItemRects; 

			Random R = new Random();
			Vector2 FinalCoordinates;
			bool useCoordinates = true;

			while (true) // loop runs until break
			{

				useCoordinates = true;

				int X = R.Next(60, 700);
				int Y = R.Next(200, 566);
				Vector2 PossibleCoordinates = new Vector2(X, Y);


				Rectangle NewItemRect = new Rectangle(X, Y, ItemList[ItemValue].Width + 30, ItemList[ItemValue].Height + 30); // form a rectangle around the enemy for collision purposes, allows 10 pixels around it

				if (NewItemRect.Intersects(playerRect))
				{
					useCoordinates = false;
				}

				// For loops cycle through ItemList

				// For loop to prevent spawning of items on other objects
				for (int i = 0; i < RectList.Count; i++)
				{
					if (NewItemRect.Intersects(RectList[i]))
					{
						useCoordinates = false;
					}
				}


				// For loop to prevent spawning of items on top of other items
				for (int i = 0; i < ItemRects.Count; i++)
				{
					if (NewItemRect.Intersects(ItemRects[i]))
					{
						useCoordinates = false;
					}
				}

				if (useCoordinates == true)
				{
					FinalCoordinates = PossibleCoordinates;
					_ItemCoordinates = FinalCoordinates;
					break;
				}
			}


			ItemRects.Add(new Rectangle((int)FinalCoordinates.X, (int)FinalCoordinates.Y, _Width, _Height)); // Add item rectangle to list
			return FinalCoordinates;
		}

	}
}