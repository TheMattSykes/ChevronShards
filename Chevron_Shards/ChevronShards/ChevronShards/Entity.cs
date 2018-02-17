using System;

using Microsoft.Xna.Framework;

namespace ChevronShards
{
	public abstract class Entity // class is inherited only and contains fields.
	{

		// Decleration of attributes.

		protected int _Width;
		public int Width { get { return _Width; } set { _Width = value; } }

		protected int _Height;
		public int Height { get { return _Height; } set { _Height = value; }  }


		protected char _orientation;
		public char Orientation { get { return _orientation; } // direction the entity is facing
			set 
			{ 
				if (value == 'L' || value == 'R' || value == 'U' || value == 'D')
				{
					_orientation = value; // set to char value
				}
			} 
		}

		protected int _HealthMax; // Maximum health entity can have
		public int HealthMax { set { _HealthMax = value; } }

		protected int _Health;
		public int Health {
			get 
			{
				return _Health;
			}

			set 
			{ 
				// health constaints
				if (_Health > 0) // health minimum is 0
				{
					_Health = value;
				}

				if (_Health < _HealthMax)
				{
					_Health = value;
				}

				if (_Health > _HealthMax)
				{
					_Health = _HealthMax;
				}

				if (_Health < 0)
				{
					_Health = 0;
				}
			}
		}

		protected Vector2 _entityPos; // possible vector coordinates that are tested, to check that they are within the bounds.
		public Vector2 EntityPos { get { return _entityPos; } set { _entityPos = value; }  }

		protected Vector2 _drawPos; // the vector coordinates where the entity will be drawn on screen.
		public Vector2 DrawPos { get { return _drawPos; } set { _drawPos = value; } }

		protected string _entityDirection; // direction the entity is facing.
		public string EntityDirection { get { return _entityDirection; } set { _entityDirection = value; } }

		protected bool _allowEntityDirChange; // allow entity to change the direction it is facing.
		public bool AllowEntityDirChange { get { return _allowEntityDirChange; } set { _allowEntityDirChange = value; } }

		protected bool _allowEntityMovement;
		public bool AllowEntityMovement { get { return _allowEntityMovement; } set { _allowEntityMovement = value; } }


		/// The entityReaction Class checks which way an entity is facing and moves them backwards by one, this is called if a collision occurs
		public void entityReaction()
		{
			_allowEntityDirChange = false; // don't allow entity to change direction.

			if (_entityDirection == "U") // facing up
			{
				_entityPos = new Vector2(_entityPos.X, _entityPos.Y + 1); // move Y in opposite direction.
			}
			if (_entityDirection == "D")
			{
				_entityPos = new Vector2(_entityPos.X, _entityPos.Y - 1);
			}
			if (_entityDirection == "L")
			{
				_entityPos = new Vector2(_entityPos.X + 1, _entityPos.Y);
			}
			if (_entityDirection == "R")
			{
				_entityPos = new Vector2(_entityPos.X - 1, _entityPos.Y);
			}

			_allowEntityDirChange = true;
		}
	}
}