using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChevronShards
{
	public class LoadGame
	{
		private bool _hasLoadedGame;
		public bool HasLoadedGame { get{ return _hasLoadedGame; } set {_hasLoadedGame = value;} }

		/// LoadGameFile
		/// A method used to load the game save file information from a text file.
		/// This method will check to see if the text file is valid in certain points and will set variables according to denoted structure.
		public void LoadGameFile(InformationDisplay mainID, Player mainPlayer, HUD mainHUD, int SaveFileNumber) 
		{
			try
			{ // If an error occurs when loading the game information then go to catch.
				string line;
				string FileName = "GameSave" + SaveFileNumber + ".txt";

				int Count = 0;

				int Row = 0;
				int Column = 0;

				StreamReader Reader = new StreamReader(FileName); // Setup a file reader

				while ((line = Reader.ReadLine()) != null)
				{
					// Count represents current line of text file
					if (Count == 1)
					{
						if (line != "True" && line != "False") // Check to see if valid
						{
							throw new Exception(); // an error is in the text file so go to catch
						}

						if (line == "True") // Has the player completed the first dungeon
						{
							mainPlayer.CompletedDungeons[0] = true;
						}
					}

					if (Count == 2)
					{
						if (line != "True" && line != "False") // Check to see if valid
						{
							throw new Exception();
						}

						if (line == "True") // Has the player completed the second dungeon
						{
							mainPlayer.CompletedDungeons[1] = true;
						}
					}

					if (Count == 3)
					{
						if (line != "True" && line != "False") // Check to see if valid
						{
							throw new Exception();
						}

						if (line == "True") // Has the player completed the third dungeon
						{
							mainPlayer.CompletedDungeons[2] = true;
						}
					}

					if (Count == 4)
					{
						// Fourth line contains where the player last saved in the overworld, even if in a dungeon.
						mainPlayer.CurrentOWSec = new Vector2(Convert.ToInt32(line[0]) - 48, Convert.ToInt32(line[1]) - 48);
					}

					if (Count == 5)
					{
						// Fith line contains how many coins the player has.
						mainPlayer.CoinCount = Convert.ToInt32(line);
					}


					if (Count == 6)
					{
						// Sixth line for how many hours the player has remaining.
						mainHUD.TotalTime = Convert.ToInt32(line);
					}

					// Seventh and Eigth lines denote information for the HUD clock.
					if (Count == 7)
					{
						mainHUD.CurrentHour = Convert.ToInt32(line);
					}
					if (Count == 8)
					{
						mainHUD.CurrentMin = Convert.ToInt32(line);
					}

					// Nested for loops used to go through text data and use boolean information to construct a 2D array.
					// File lines 9-14 contain binary information for an array structure denoting where in the overworld the player has already been.
					if (Count >= 9 && Count < 14)
					{
						for (int i = 0; i < line.Length; i++)
						{
							if (line[i] != '1' && line[i] != '0') 
							{
								throw new Exception(); // Must be binary data otherwise erronious.
							}

							if (line[i] == '1')
							{
								mainPlayer.VisitedOWSections[Row, Column] = true;
							}
							if (line[i] == '0')
							{
								mainPlayer.VisitedOWSections[Row, Column] = false;
							}
							Row++;
						}
						Row = 0;
						Column++;
					}

					if (Count == 14) { Column = 0; } // Reset Column, Row has already been reset.

					// File lines 14-18 contain binary information for an array structure denoting where in Dungeon1 the player has already been.
					if (Count >= 14 && Count < 18)
					{
						for (int i = 0; i < line.Length; i++)
						{
							if (line[i] != '1' && line[i] != '0')
							{
								throw new Exception();
							}

							if (line[i] == '1')
							{
								mainPlayer.VisitedDUN1Sections[Row, Column] = true;
							}
							if (line[i] == '0')
							{
								mainPlayer.VisitedDUN1Sections[Row, Column] = false;
							}
							Row++;
						}

						Row = 0;
						Column++;
					}

					if (Count == 18) { Column = 0; } // Reset Column, Row has already been reset.

					// File lines 18-22 contain binary information for an array structure denoting where in Dungeon2 the player has already been.
					if (Count >= 18 && Count < 22)
					{
						for (int i = 0; i < line.Length; i++)
						{
							if (line[i] != '1' && line[i] != '0')
							{
								throw new Exception();
							}

							if (line[i] == '1')
							{
								mainPlayer.VisitedDUN2Sections[Row, Column] = true;
							}
							if (line[i] == '0')
							{
								mainPlayer.VisitedDUN2Sections[Row, Column] = false;
							}
							Row++;
						}

						Row = 0;
						Column++;
					}

					if (Count == 22) { Column = 0; } // Reset Column, Row has already been reset.

					// File lines 22-ENDOFFILE contain binary information for an array structure denoting where in Dungeon3 the player has already been.
					if (Count >= 22)
					{
						for (int i = 0; i < line.Length; i++)
						{
							if (line[i] != '1' && line[i] != '0')
							{
								throw new Exception();
							}

							if (line[i] == '1')
							{
								mainPlayer.VisitedDUN3Sections[Row, Column] = true;
							}
							if (line[i] == '0')
							{
								mainPlayer.VisitedDUN3Sections[Row, Column] = false;
							}
							Row++;
						}

						Row = 0;
						Column++;
					}


					Count++;
				}

				_hasLoadedGame = true;
				Reader.Close(); // Close the reader
			}
			catch
			{
				mainID.GameError = true;
				mainID.GameErrorCode = "GE0008LGF"; // failed to load game file, streamreader error would cause this
			}
		}


		/// INIT
		/// The INIT function is used everytime the LoadScreen is show, this uses the information from all the GameSave files in a priliminary manner.
		/// This information will be used to display information on the LoadScreen rather than actually loading the GameSave file.
		public void INIT(ref bool[] ActiveSaves, ref bool[,] CompletedDungeons) 
		{ 
			string line;
			int Count = 0;

			for (int i = 0; i < 3; i++) // There are 3 save files to scan for priliminary information
			{
				int GameSaveNumber = i + 1;
				StreamReader Reader = new StreamReader("GameSave"+GameSaveNumber+".txt"); // Set up reader for each game file
				while ((line = Reader.ReadLine()) != null)
				{
					if (Count == 0)
					{
						if (line == "Active") // check to see if the game file is active
						{
							ActiveSaves[i] = true;
						}
						else
						{
							ActiveSaves[i] = false;
							break;
						}
					}
					if (Count == 1)
					{
						if (line == "True") // has first shard been collected?
						{
							CompletedDungeons[i, 0] = true;
						}
						else
						{
							CompletedDungeons[i, 0] = false;
						}
					}
					if (Count == 2)
					{
						if (line == "True") // has second shard been collected?
						{
							CompletedDungeons[i, 1] = true;
						}
						else
						{
							CompletedDungeons[i, 1] = false;
						}
					}
					if (Count == 3)
					{
						if (line == "True") // has third shard been collected?
						{
							CompletedDungeons[i, 2] = true;
						}
						else
						{
							CompletedDungeons[i, 2] = false;
						}
					}

					Count++;
				}
				Reader.Close(); // close reader
				line = null; // reset values
				Count = 0;
			}
		}



		/// SaveGameFile
		/// Writes current information into the text file of the GameSave selected on the LoadScreen.
		public void SaveGameFile(Player mainPlayer, HUD mainHUD, InformationDisplay mainID)
		{
			// New arrays to store information from text file
			bool[,] InternalVisitedOWSecs = mainPlayer.VisitedOWSections;
			bool[,] InternalVisitedDUN1Secs = mainPlayer.VisitedDUN1Sections;
			bool[,] InternalVisitedDUN2Secs = mainPlayer.VisitedDUN2Sections;
			bool[,] InternalVisitedDUN3Secs = mainPlayer.VisitedDUN3Sections;

			InternalVisitedOWSecs[(int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y] = true; // must have visited the section they last saved in.

			// Sets the dungeon section the player saved in to visited status i.e. true.
			if (mainPlayer.CurrentDungeonNumber == 1 && mainPlayer.InDungeon == true)
			{
				InternalVisitedDUN1Secs[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] = true;
			}

			if (mainPlayer.CurrentDungeonNumber == 2 && mainPlayer.InDungeon == true)
			{
				InternalVisitedDUN2Secs[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] = true;
			}

			if (mainPlayer.CurrentDungeonNumber == 3 && mainPlayer.InDungeon == true)
			{
				InternalVisitedDUN3Secs[(int)mainPlayer.CurrentDUNSec.X, (int)mainPlayer.CurrentDUNSec.Y] = true;
			}

			int SaveFileNumber = mainID.SaveFileNumber;

			string FileName = "GameSave" + SaveFileNumber + ".txt";

			using (var Writer = new StreamWriter(FileName)){

				// Begin writing data to relevent text file in same layout as the reading version
				Writer.WriteLine("Active"); // save file is active
				Writer.WriteLine(mainPlayer.CompletedDungeons[0]);
				Writer.WriteLine(mainPlayer.CompletedDungeons[1]);
				Writer.WriteLine(mainPlayer.CompletedDungeons[2]);
				Writer.WriteLine("{0}{1}",(int)mainPlayer.CurrentOWSec.X, (int)mainPlayer.CurrentOWSec.Y);
				Writer.WriteLine(mainPlayer.CoinCount);
				Writer.WriteLine(mainHUD.TotalTime);
				Writer.WriteLine(mainHUD.CurrentHour);
				Writer.WriteLine(mainHUD.CurrentMin);


				// Reversed version of reading visited sections, writes a 1 or 0 depending on array value; through a nested for loop.
				for (int Column = 0; Column < 5; Column++)
				{
					for (int Row = 0; Row < 5; Row++)
					{
						if (InternalVisitedOWSecs[Row, Column] == true)
						{
							Writer.Write("1");
						}
						else
						{
							Writer.Write("0");
						}
					}

					Writer.WriteLine();
				}


				for (int Column = 0; Column < 4; Column++)
				{
					for (int Row = 0; Row < 4; Row++)
					{

						if (InternalVisitedDUN1Secs[Row, Column] == true)
						{
							Writer.Write("1");
						}
						else
						{
							Writer.Write("0");
						}
					}
					Writer.WriteLine();
				}

				for (int Column = 0; Column < 4; Column++)
				{
					for (int Row = 0; Row < 4; Row++)
					{
						if (InternalVisitedDUN2Secs[Row, Column] == true)
						{
							Writer.Write("1");
						}
						else
						{
							Writer.Write("0");
						}
					}
					Writer.WriteLine();
				}

				for (int Column = 0; Column < 4; Column++)
				{
					for (int Row = 0; Row < 4; Row++)
					{
						if (InternalVisitedDUN3Secs[Row, Column] == true)
						{
							Writer.Write("1");
						}
						else
						{
							Writer.Write("0");
						}
					}
					Writer.WriteLine();
				}

				Writer.Close();
			}
		}


		/// DeleteSaveFile
		/// Method for deleting save file, where game save file is set to inactive.
		public void DeleteGameFile(int deleteNum)
		{
			string FileName = "GameSave" + deleteNum + ".txt";

			using (var Writer = new StreamWriter(FileName))
			{
				Writer.WriteLine("InActive"); // Set to inactive, in effect deleting values until overritten with a new save.
			}
		}
	}
}