using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ChevronShards
{
	public class Sword : Weapon
	{
		public Sword()
		{
			// Set default weapon values
			_WeaponPower = 10;
			_WeaponWidth = 14;
			_WeaponHeight = 32;
		}

		public override string GetWeaponTextureName(char Direction)
		{ // return weapon texture name as string depending on direction
			if (Direction == 'U')
			{
				_WeaponWidth = 14; // width and height will change depending on direction the weapon is facing
				_WeaponHeight = 32;

				return "Sword_Up";
			}
			if (Direction == 'D')
			{
				_WeaponWidth = 14;
				_WeaponHeight = 32;

				return "Sword_Down";
			}
			if (Direction == 'L')
			{
				_WeaponWidth = 32;
				_WeaponHeight = 14;

				return "Sword_Left";
			}
			if (Direction == 'R')
			{
				_WeaponWidth = 32;
				_WeaponHeight = 14;

				return "Sword_Right";
			}
			else {
				return "error"; // erronious input into function
			}
		}
	}
}