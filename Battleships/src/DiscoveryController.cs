﻿using System;
using SwinGameSDK;

/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
internal static class DiscoveryController
{

	/// <summary>
	/// Handles input during the discovery phase of the game.
	/// </summary>
	/// <remarks>
	/// Escape opens the game menu. Clicking the mouse will
	/// attack a location.
	/// </remarks>
	public static void HandleDiscoveryInput()
	{
		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			GameController.AddNewState(GameState.ViewingGameMenu);
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton))
		{
			DoAttack();
		}

		//added by jesse -- coordinate for home button to back homepage
		if (SwinGame.MouseClicked(MouseButton.LeftButton) & UtilityFunctions.IsMouseInRectangle
			(685, 70, 51, 46))
		{
			GameController.AddNewState(GameState.ViewingGameMenu);
		}
	}

	/// <summary>
	/// Attack the location that the mouse if over.
	/// </summary>
	private static void DoAttack()
	{
		//set point2d to default
		Point2D mouse = default(Point2D);
		mouse = SwinGame.MousePosition();
		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
		col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

		if (row >= 0 && row < GameController.HumanPlayer.EnemyGrid.Height)
		{
			if (col >= 0 && col < GameController.HumanPlayer.EnemyGrid.Width)
			{
				GameController.Attack(row, col);
			}
		}
	}

	/// <summary>
	/// Draws the game during the attack phase.
	/// </summary>s
	public static void DrawDiscovery()
	{
		const int SCORES_LEFT = 172;
		const int SHOTS_TOP = 157;
		const int HITS_TOP = 206;
		const int SPLASH_TOP = 256;

		//Added By Jesse
		const int SCORE_TOP = 306;

		if (((SwinGame.KeyDown(KeyCode.vk_LSHIFT) | SwinGame.KeyDown(KeyCode.vk_RSHIFT)) & SwinGame.KeyDown(KeyCode.vk_c)))
		{
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, true);
		}
		else
		{
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, false);
		}

		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);
		UtilityFunctions.DrawMessage();

		//Added by Voon.
		//Draw Destroyed Ships.
		UtilityFunctions.DrawDestroyField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);

		SwinGame.DrawText(GameController.HumanPlayer.Shots.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Hits.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Missed.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);

		//Added by Jesse
		SwinGame.DrawText(GameController.HumanPlayer.Score.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SCORE_TOP);


		//Added by Voon.
		//Draw the number of ships left that need to destroy.
		SwinGame.DrawText("Number of Ships left: ", Color.White, GameResources.GameFont("Menu"), 350, 90);
		SwinGame.DrawText((5 - GameController.ComputerPlayer.PlayerGrid.ShipsKilled).ToString(), Color.Yellow, GameResources.GameFont("Menu"), 515, 90);

		//added by jesse -- gaming page with home button to return home page
		SwinGame.DrawBitmap(GameResources.GameImage("HomeButton"), 685, 70);
	}

}
