﻿using System;
using SwinGameSDK;

/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

internal static class EndingGameController
{

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame()
	{
		UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		//added mouse 1 to quit by jesse
		if (GameController.HumanPlayer.IsDestroyed)
		{
			SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			SwinGame.DrawTextLines("Left Click To Quit", Color.Yellow, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 350, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
		else
		{
			GameController.TheGame.StopTimer();
			SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			SwinGame.DrawTextLines("Left Click To Quit", Color.Yellow, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 350, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
			GameController.EndCurrentState();
		}
	}

}
