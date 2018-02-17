namespace ChevronShards
{
	class FireBall : Weapon
	{
		public FireBall()
		{
			// Set default weapon values
			_WeaponTextureName = "FireBall";
			_WeaponWidth = 24;
			_WeaponHeight = 24;
		}

		public override string GetWeaponTextureName(char Direction)
		{
			// return weapon texture name as string depending on direction
			if (Direction == 'R')
			{
				return "FireBall_Right";
			}

			if (Direction == 'U')
			{
				return "FireBall_Up";
			}

			if (Direction == 'D')
			{
				return "FireBall_Down";
			}

			else {
				return "FireBall"; // default texture name
			}
		}
	}
}