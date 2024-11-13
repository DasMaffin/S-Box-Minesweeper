using Sandbox;
using System;

public enum GameState
{
	PreGame,
	Gaming,
	Win,
	Loss
}

public sealed class GameManager : Component
{
	private static GameManager instance;
	public static GameManager Instance 
	{
		get
		{
			return instance;
		}
		set
		{
			if(instance != null)
			{
				value.Destroy();
			}
			instance = value;
		}
	}
	[Property]
	public CameraComponent MainCamera;
	private GameState gameState;
	public GameState GameState {  get => gameState;
		set
		{ 
			gameState = value;
			switch ( gameState )
			{
				case GameState.PreGame:
				case GameState.Gaming:
					break;
				case GameState.Win:
					Log.Error("Show a win screen you dulli.");
					break;
				case GameState.Loss:
					Log.Error("Show a loss screen you dulli.");
					break;
			}
		}
	}
	public bool GeneratedField = false;

	protected override void OnAwake()
	{
		Instance = this;

		Mouse.Visible = true;
		GameState = GameState.PreGame;

		_ = new WinPanel();

		base.OnAwake();
	}

	protected override void OnUpdate()
	{
		if ( Input.Pressed("F12") )
			Game.TakeScreenshot();

		base.OnUpdate();
	}

	public bool GenerateMinefield()
	{
		foreach (GridCell cell in Grid.GlobalGrid.GridCells)
		{
			int random = Random.Shared.Int(0, 100);
			cell.HasBomb = random < 10 ? true : false;
		}
		GameState = GameState.Gaming;
		return true;
	}
}
