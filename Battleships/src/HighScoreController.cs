using System;
using System.IO;
using SwinGameSDK;
using System.Collections.Generic;

/// <summary>
/// Controls displaying and collecting high score data.
/// </summary>
/// <remarks>
/// Data is saved to a file.
/// </remarks>
internal static class HighScoreController
{
	//Edited by Voon.
	private const int NAME_WIDTH = 10;
	private const int SCORES_LEFT = 490;

	/// <summary>
	/// The score structure is used to keep the name and
	/// score of the top players together.
	/// </summary>
	private struct Score : IComparable
	{
		public string Name;
		public int Value;

		/// <summary>
		/// Allows scores to be compared to facilitate sorting
		/// </summary>
		/// <param name="obj">the object to compare to</param>
		/// <returns>a value that indicates the sort order</returns>
		public int CompareTo(object obj)
		{
			if (obj is Score)
			{
				Score other = (Score)obj;

				return other.Value - this.Value;
			}
			else
			{
				return 0;
			}
		}
	}

	private static List<Score> _Scores = new List<Score>();

	/// <summary>
	/// Loads the scores from the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNNNNNNNNSSS
	/// 
	/// Where NNNNNNNNNN is the name and SSS is the score
	/// </remarks>
	private static void LoadScores()
	{
		string filename = SwinGame.PathToResource("highscores.txt");

		StreamReader input = new StreamReader(filename);

		//Read in the # of scores
		int numScores = Convert.ToInt32(input.ReadLine());

		_Scores.Clear();

		int i = 0;

		for (i = 1; i <= numScores; i++)
		{
			Score s = new Score();
			string line = input.ReadLine();


			s.Name = line.Substring(0, NAME_WIDTH);
			s.Value = Convert.ToInt32(line.Substring(NAME_WIDTH));
			_Scores.Add(s);
		}
		input.Close();
	}

	/// <summary>
	/// Saves the scores back to the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	private static void SaveScores()
	{
		string filename = SwinGame.PathToResource("highscores.txt");

		StreamWriter output = new StreamWriter(filename);

		output.WriteLine(_Scores.Count);

		foreach (Score s in _Scores)
		{
			output.WriteLine(s.Name + s.Value);
		}

		output.Close();
	}

	/// <summary>
	/// Draws the high scores to the screen.
	/// </summary>
	public static void DrawHighScores()
	{
		const int SCORES_HEADING = 40;
		const int SCORES_TOP = 80;
		const int SCORE_GAP = 30;

		if (_Scores.Count == 0)
		{
			LoadScores();
		}

		SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, SCORES_HEADING);

		//For all of the scores
		int i = 0;
		for (i = 0; i < _Scores.Count; i++)
		{
			Score s = default(Score);

			//fixed by Voon.
			s = _Scores[i];

			//for scores 1 - 9 use 01 - 09
			if (i < 9)
			{
				SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
			}
			else
			{
				SwinGame.DrawText((i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
			}
		}

		//fixed by Voon.
		SaveScores();
	}
	/// <summary>
	/// Give instruction about gameplay
	/// </summary>
public static void DrawInstruction()
{
	SwinGame.DrawText("Gameplay Instructions", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 30);
	SwinGame.DrawText("----------------------------------------", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 40);
	SwinGame.DrawText("1. Please choose the difficulties of the game", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 100);
	SwinGame.DrawText("2. Click the Play button to start the battleship game", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 130);
	SwinGame.DrawText("3. Arrange 5 ships where you want to put", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 160);
	SwinGame.DrawText("4. Click the play symbol at the top right to proceed", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 190);
	SwinGame.DrawText("5. Choose the place you want to shoot until you defeat the enemy", Color.AliceBlue, GameResources.GameFont("Courier"), 2, 220);
		}
	/// <summary>
	/// Handles the user input during the top score screen.
	/// </summary>
	/// <remarks></remarks>
	public static void HandleHighScoreInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE) || SwinGame.KeyTyped(KeyCode.vk_RETURN))
		{
			GameController.EndCurrentState();
		}
	}

	/// <summary>
	/// Read the user's name for their highsSwinGame.
	/// </summary>
	/// <param name="value">the player's sSwinGame.</param>
	/// <remarks>
	/// This verifies if the score is a highsSwinGame.
	/// </remarks>
	public static void ReadHighScore(int value)
	{
		const int ENTRY_TOP = 500;

		if (_Scores.Count == 0)
		{
			LoadScores();
		}

		//is it a high score
		if (value > _Scores[_Scores.Count - 1].Value)
		{
			Score s = new Score();
			s.Value = value;

			GameController.AddNewState(GameState.ViewingHighScores);

			int x = SCORES_LEFT + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: ");

			SwinGame.StartReadingText(Color.White, NAME_WIDTH, GameResources.GameFont("Courier"), x, ENTRY_TOP);

			//Read the text from the user
			while (SwinGame.ReadingText())
			{
				SwinGame.ProcessEvents();

				UtilityFunctions.DrawBackground();
				DrawHighScores();
				SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, ENTRY_TOP);
				SwinGame.RefreshScreen();
			}

			s.Name = SwinGame.TextReadAsASCII();

			if (s.Name.Length < 10)
			{
				s.Name = s.Name + new string(' ', 10 - s.Name.Length);
			}

			_Scores.RemoveAt(_Scores.Count - 1);
			_Scores.Add(s);
			_Scores.Sort();

			GameController.EndCurrentState();
		}
	}

}
