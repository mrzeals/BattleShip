
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SwinGameSDK;

//// <summary>
/// The DeploymentController controls the players actions
/// during the deployment phase.
/// </summary>
static class DeploymentController
{
	private const int SHIPS_TOP = 98;
	private const int SHIPS_LEFT = 20;
	private const int SHIPS_HEIGHT = 90;

	private const int SHIPS_WIDTH = 300;
	private const int TOP_BUTTONS_TOP = 72;

	private const int TOP_BUTTONS_HEIGHT = 46;
	private const int PLAY_BUTTON_LEFT = 693;

	private const int PLAY_BUTTON_WIDTH = 80;
	private const int UP_DOWN_BUTTON_LEFT = 410;

	private const int LEFT_RIGHT_BUTTON_LEFT = 350;
	private const int RANDOM_BUTTON_LEFT = 547;

	private const int RANDOM_BUTTON_WIDTH = 51;

	private const int DIR_BUTTONS_WIDTH = 47;

	private const int TEXT_OFFSET = 5;
	private static Direction _currentDirection = Direction.UpDown;

	private static ShipName _selectedShip = ShipName.Tug;
	/// <summary>
	/// Handles user input for the Deployment phase of the game.
	/// </summary>
	/// <remarks>
	/// Involves selecting the ships, deloying ships, changing the direction
	/// of the ships to add, randomising deployment, end then ending
	/// deployment
	/// </remarks>
	/// Edited by SzeSan added GameController.AddNewState & GameController.HumanPlayer & GameController.EndDeployment() & UtilityFunctions.IsMouseInRectangle
	public static void HandleDeploymentInput()
	{
		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE)) {
			GameController.AddNewState(GameState.ViewingGameMenu);
		}
		//Edited by SzeSan change | to ||
		if (SwinGame.KeyTyped(KeyCode.vk_UP) || SwinGame.KeyTyped(KeyCode.vk_DOWN)) {
			_currentDirection = Direction.UpDown;
		}
		//Edited by SzeSan change | to ||
		if (SwinGame.KeyTyped(KeyCode.vk_LEFT) || SwinGame.KeyTyped(KeyCode.vk_RIGHT)) {
			_currentDirection = Direction.LeftRight;
		}
		if (SwinGame.KeyTyped(KeyCode.vk_r)) {
			GameController.HumanPlayer.RandomizeDeployment();
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton)) {
			ShipName selected = default(ShipName);
			selected = GetShipMouseIsOver();
			if (selected != ShipName.None) {
				_selectedShip = selected;
			} else {
				DoDeployClick();
			}

			if (GameController.HumanPlayer.ReadyToDeploy & UtilityFunctions.IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT)) {
				GameController.EndDeployment();
				//Edited by Sze San change Direction.LeftRight to Direction.UpDown
			} else if (UtilityFunctions.IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT)) {
				_currentDirection = Direction.UpDown;
			} else if (UtilityFunctions.IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT)) {
				_currentDirection = Direction.LeftRight;
			} else if (UtilityFunctions.IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT)) {
				GameController.HumanPlayer.RandomizeDeployment();
			}
		}
	}

	/// <summary>
	/// The user has clicked somewhere on the screen, check if its is a deployment and deploy
	/// the current ship if that is the case.
	/// </summary>
	/// <remarks>
	/// If the click is in the grid it deploys to the selected location
	/// with the indicated direction
	/// </remarks>
	/// Edited by SzeSan added GameController.HumanPlayer & GameResources.GameSound & UtilityFunctions.Message
	/// Edited by SzeSan added UtilityFunctions.CELL_HEIGHT/CELL_GAP/FIELD_LEFT/CELL_WIDTH
	private static void DoDeployClick()
	{
		Point2D mouse = default(Point2D);

		mouse = SwinGame.MousePosition();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
        //row = Convert.ToInt32(Math.Floor((mouse.Y) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
        row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
       
        col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

		if (row >= 0 & row < GameController.HumanPlayer.PlayerGrid.Height) {
			if (col >= 0 & col < GameController.HumanPlayer.PlayerGrid.Width) {
				//if in the area try to deploy
				try {
					GameController.HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection);
                    UtilityFunctions.Message = "Ship deployed";
				} catch (Exception ex) {
					Audio.PlaySoundEffect(GameResources.GameSound("Error"));
					UtilityFunctions.Message = ex.Message;
				}
			}
		}
	}

	/// <summary>
	/// Draws the deployment screen showing the field and the ships
	/// that the player can deploy.
	/// </summary>
	/// Edited by SzeSan added UtilityFunctions.DrawField & GameController.HumanPlayer & GameResources.GameImage  
	public static void DrawDeployment()
	{
		UtilityFunctions.DrawField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer, true);

		//Draw the Left/Right and Up/Down buttons
		if (_currentDirection == Direction.LeftRight) {
            SwinGame.DrawBitmap(GameResources.GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.DrawText("U/D", Color.Gray, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
			//SwinGame.DrawText("L/R", Color.White, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
		} else {
            SwinGame.DrawBitmap(GameResources.GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.DrawText("U/D", Color.White, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
			//SwinGame.DrawText("L/R", Color.Gray, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
		}

		//DrawShips
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
			int i = 0;
			i = ((int) sn) - 1;
			if (i >= 0) {
				if (sn == _selectedShip) {
					SwinGame.DrawBitmap(GameResources.GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT);
					//    SwinGame.FillRectangle(Color.LightBlue, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
					//Else
					//    SwinGame.FillRectangle(Color.Gray, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
				}

				//SwinGame.DrawRectangle(Color.Black, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
				//SwinGame.DrawText(sn.ToString(), Color.Black, GameFont("Courier"), SHIPS_LEFT + TEXT_OFFSET, SHIPS_TOP + i * SHIPS_HEIGHT)

			}
		}
		//Edited by SzeSan
		if (GameController.HumanPlayer.ReadyToDeploy) {
			SwinGame.DrawBitmap(GameResources.GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.FillRectangle(Color.LightBlue, PLAY_BUTTON_LEFT, PLAY_BUTTON_TOP, PLAY_BUTTON_WIDTH, PLAY_BUTTON_HEIGHT)
			//SwinGame.DrawText("PLAY", Color.Black, GameFont("Courier"), PLAY_BUTTON_LEFT + TEXT_OFFSET, PLAY_BUTTON_TOP)
		}

		SwinGame.DrawBitmap(GameResources.GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP);

		UtilityFunctions.DrawMessage();
	}

	/// <summary>
	/// Gets the ship that the mouse is currently over in the selection panel.
	/// </summary>
	/// <returns>The ship selected or none</returns>
	/// Edited by SzeSan added UtilityFunctions.IsMouseInRectangle 
	private static ShipName GetShipMouseIsOver()
	{
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
			int i = 0;
			i = ((int)sn) - 1;

			if (UtilityFunctions.IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)) {
				return sn;
			}
		}

		return ShipName.None;
	}
	//Created a public propetry for  _currentDirection to testing purpose.
	public  static Direction CurrentDirection{
		get{ 
			return _currentDirection;
		}
		set{
			_currentDirection = value;
		}
	}

	//key_Right is typed
	public static void Right_Keytyped(){
		CurrentDirection = Direction.LeftRight;
	}
	//key_Up is typed
	public static void Up_Keytyped(){
		CurrentDirection = Direction.UpDown;
	}

	public  static ShipName SelectedShip{
		get{ 
			return _selectedShip;
		}
		set{
			_selectedShip = value;
		}
	}

	public static void ShipSelection(int row){
		if (row == 0)
		{
			SelectedShip = ShipName.None;
		}
		else if (row == 1)
		{
			SelectedShip = ShipName.Tug;
		}
		else if (row == 2)
		{
			SelectedShip = ShipName.Submarine;
		}
		else if (row == 3)
		{
			SelectedShip = ShipName.Destroyer;
		}
		else if (row == 4)
		{
			SelectedShip = ShipName.Battleship;
		}
		else if (row == 5)
		{
			SelectedShip = ShipName.AircraftCarrier;
		}
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
