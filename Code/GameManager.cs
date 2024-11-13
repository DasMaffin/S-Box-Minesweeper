using Sandbox;
using System;
using System.Text.RegularExpressions;

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
			GameManagerRazor.Instance = instance;
		}
	}
	[Property] public CameraComponent MainCamera;
	[Property] public GameObject CubePrefab;

	[Property] private GameState gameState;
	public GameState GameState {  
		get => gameState;
		set
		{ 
			gameState = value;
			Log.Info($"Gamestate changed to {value}");
			switch ( gameState )
			{
				case GameState.PreGame:
				case GameState.Gaming:
					break;
				case GameState.Win:
					EnableWinPanel();
					break;
				case GameState.Loss:
					EnableLossPanel();
					break;
			}
		}
	}

	protected override void OnAwake()
	{
		Instance = this;

		Mouse.Visible = true;
		GameState = GameState.PreGame;

		ToggleLossPanel();
		ToggleWinPanel();
		DisableSettingsHUD();

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

	#region UIHandling

	[Property] public PanelComponent WinPanel;
	[Property] public PanelComponent LossPanel;
	[Property] public PanelComponent SettingsPanel;
	public void ToggleWinPanel()
	{
		WinPanel.Enabled = !WinPanel.Active;
	}

	public void ToggleLossPanel()
	{
		LossPanel.Enabled = !LossPanel.Active;
	}
	public void EnableWinPanel()
	{
		WinPanel.Enabled = true;
	}

	public void EnableLossPanel()
	{
		LossPanel.Enabled = true;
	}

	public void EnableSettingsHUD()
	{
		SettingsPanel.Enabled = true;
	}

	public void DisableSettingsHUD()
	{
		SettingsPanel.Enabled = false;
	}

	#endregion
}

public class GameManagerRazor
{
	private static GameManager instance;
	public static GameManager Instance 
	{
		get => instance;
		set
		{
			instance = value;
		}
	}
}
