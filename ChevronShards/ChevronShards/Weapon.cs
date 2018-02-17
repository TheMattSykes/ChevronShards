using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace ChevronShards
{
	public interface WeaponInterface 
	{
		// Interface to set out structure, methods and parameters for Weapon abstract class
		Texture2D WeaponTexture { get; set; }

		int WeaponFireTime { get; set; }
		int WeaponFireTimeMax { get; set; }

		void SetRectangle();
		string GetWeaponTextureName(char Direction);
		int GetWeaponPower();
		void SetWeaponPower(int setAmount);
		int GetWeaponSpeed();
		void SetWeaponSpeed(int setAmount);

		int GetWeaponWidth();
		int GetWeaponHeight();

		Rectangle GetWeaponRect();

		Vector2 GetWeaponCoordinates();

		void SetWeaponCoordinates(Vector2 wCoeordinates);

		char GetWeaponOrientation();

		void SetWeaponOrientation(char orientation);

		void FireWeapon();
	}


	public abstract class Weapon : WeaponInterface
	{
		protected Texture2D _WeaponTexture; // Texture of the weapon
		public Texture2D WeaponTexture { get { return _WeaponTexture; } set { _WeaponTexture = value; } }

		protected int _WeaponFireTime; // How long the weapon has been fired for
		public int WeaponFireTime { get { return _WeaponFireTime; } set { _WeaponFireTime = value; } }

		protected int _WeaponFireTimeMax; // Maximum time the weapon can be fired
		public int WeaponFireTimeMax { get { return _WeaponFireTimeMax; } set { _WeaponFireTimeMax = value; } }

		protected string _WeaponTextureName;

		protected int _WeaponPower;
		protected int _WeaponSpeed;
		protected int _WeaponWidth;
		protected int _WeaponHeight;

		protected Vector2 _WeaponCoordinates; // Vector XY values of the weapon from 0,0 top left
		protected char _WeaponOrientation; // Direction weapon will be fired in

		protected Rectangle _WeaponRectangle;

		public Weapon()
		{
			SetRectangle();
		}



		public virtual string GetWeaponTextureName(char Direction) { // return the string name of the Weapons texture
			return _WeaponTextureName;
		}


		public void SetRectangle() {

			_WeaponRectangle = new Rectangle((int)_WeaponCoordinates.X, (int)_WeaponCoordinates.Y, _WeaponWidth, _WeaponHeight); // draw rectangle around weapon
		}


		// Return variables to other parts of the program which instantiated a weapon object via a subclass
		public int GetWeaponPower() { 
			return _WeaponPower;
		}

		public void SetWeaponPower(int setAmount)
		{
			_WeaponPower = setAmount;
		}


		public int GetWeaponSpeed()
		{
			return _WeaponSpeed;
		}

		public void SetWeaponSpeed(int setAmount)
		{
			_WeaponSpeed = setAmount;
		}


		public int GetWeaponWidth()
		{
			return _WeaponWidth;
		}

		public int GetWeaponHeight()
		{
			return _WeaponWidth;
		}



		public Rectangle GetWeaponRect()
		{
			return _WeaponRectangle;
		}


		public Vector2 GetWeaponCoordinates()
		{
			return _WeaponCoordinates;
		}

		public void SetWeaponCoordinates(Vector2 wCoordinates)
		{
			_WeaponCoordinates = wCoordinates;
		}

		public char GetWeaponOrientation() { // direction the weapon is facing
			return _WeaponOrientation;
		}

		public void SetWeaponOrientation(char orientation)
		{
			_WeaponOrientation = orientation;
		}




		public virtual void FireWeapon() 
		{ // function increments the weapon coordinates by the speed depending on direction
			if (_WeaponOrientation == 'U')
			{
				_WeaponCoordinates = new Vector2(_WeaponCoordinates.X, _WeaponCoordinates.Y - _WeaponSpeed);
			}

			if (_WeaponOrientation == 'D')
			{
				_WeaponCoordinates = new Vector2(_WeaponCoordinates.X, _WeaponCoordinates.Y + _WeaponSpeed);
			}

			if (_WeaponOrientation == 'L')
			{
				_WeaponCoordinates = new Vector2(_WeaponCoordinates.X - _WeaponSpeed, _WeaponCoordinates.Y);
			}

			if (_WeaponOrientation == 'R')
			{
				_WeaponCoordinates = new Vector2(_WeaponCoordinates.X + _WeaponSpeed, _WeaponCoordinates.Y);
			}
		}
	}
}
