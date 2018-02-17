using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ChevronShards
{
	public class BossLevelManager : Area
	{

		private int _BossLevelNumber;

		private int _EnemyAmount;

		private bool _BossGenerated;

		private Texture2D BossLevelBack;
		private Texture2D Brick;


		/// LoadContent
		/// Load content data into Texture variables.
		public override void LoadContent(ContentManager Content)
		{
			BossLevelBack = Content.Load<Texture2D>("Dungeon1Background");

			Brick = Content.Load<Texture2D>("Gray_Bricks");
		}

		/// GetBossLevelNumber
		/// Returns the current boss level number.
		public override int GetBossLevelNumber() {
			return _BossLevelNumber;
		}

		/// SetBossLevelNumber
		/// Sets the current boss level number.
		public override void SetBossLevelNumber(int num) {
			_BossLevelNumber = num;
			Console.WriteLine("BOSS LEVEL NUMBER: "+_BossLevelNumber);
		}


		/// BossLevel Section Generator
		/// Reads from the relevent BossLevel Section text file, converts each line into usable information as commented below.
		/// The file will denote the structure of the overworld, the file writes the structure in rows and columns under binary form.
		public override void GenerateStructure(params int[] values) // for sliding animation
		{
			string line;
			string FileName = "BossLevel" + _BossLevelNumber + ".txt";

			int Count = 0;

			int Row = 0;
			int Column = 0;

			StreamReader Reader = new StreamReader(FileName);

			while ((line = Reader.ReadLine()) != null)
			{
				_EnemyAmount = 1;

				if (Count >= 2) // The Final Lines denote the structure of the overworld with a 1 indicating a block e.g. a tree is present, or a 0 for a blank space.
				{
					for (int i = 0; i < line.Length; i++)
					{
						if (line[i] == '1')
						{
							_Structure[Row, Column] = true;
						}
						if (line[i] == '0')
						{
							_Structure[Row, Column] = false;
						}
						Row++;
					}

					Row = 0;
					Column++;
				}

				Count++;
			}

			Reader.Close();
		}


		/// GenerateEnemies
		/// Uses values gained previously from the text file and adds them to the enemyList.
		/// Still uses list structure although there is only one boss for continuity with the rest of the program.
		public override void GenerateEnemies(Rectangle playerRect, InformationDisplay mainID)
		{
			if (mainID.EraseList == true)
			{
				_enemyList.Clear();
				mainID.EraseList = false;
			}
			if (_EnemyAmount > 0)
			{
				if (mainID.NewEnemyList == true)
				{
					_enemyList.Clear();

					Random R = new Random();
					int a = R.Next(_EnemyAmount, _EnemyAmount + 1);


					_enemyList.Add(new Boss());
					_enemyList[0].EnemyTextureName = "Boss" + (_BossLevelNumber); // set texture name.

					_enemyList[0].Health = (200);
					_enemyList[0].HealthMax = (200);

					_enemyList[0].Orientation = 'D'; // default facing direction.


					// Set random coordinates within bounds
					Vector2 coordinates = _enemyList[0].EnemyGenCoordinate(_enemyList[0].Width, _enemyList[0].Height, playerRect, _collisionRects, ref _enemyRects);
					_enemyList[0].EnemyCoordinates = (coordinates);


					mainID.NewEnemyList = false;

					_BossGenerated = true;
				}
			}
		}


		/// CheckCompletedDungeon
		/// Returns true if the enemy count in the BossLevel is 0.
		public override bool CheckCompletedDungeon() {
			if (_enemyList.Count < 1 && _BossGenerated == true)
			{
				return true;
			}

			return false;
		}


		/// Draw
		public override void Draw(SpriteBatch spriteBatch, Player mainPlayer, InformationDisplay mainID)
		{

			spriteBatch.Draw(BossLevelBack, new Vector2(0, 96));

			// Draw structure of BossLevel, go through for nested loop in coordinate increments of 48 to set structure.
			int x = 0;
			int y;
			for (int row = 0; row < 16; row++)
			{
				y = 96;
				for (int column = 0; column < 13; column++)
				{
					if (_Structure[row, column] == true) // brick is present at coordinate so draw.
					{
						spriteBatch.Draw(Brick, new Vector2(x, y));
					}
					y += 48;
				}
				x += 48;
			}
		}
	}
}