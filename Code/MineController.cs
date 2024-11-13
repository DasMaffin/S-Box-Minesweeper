using Sandbox;
using System.Runtime.CompilerServices;

public sealed class MineController : Component
{
	GridCell MyCell {  get; set; }
	ModelRenderer MyRenderer { get; set; }
	TextRenderer MyTextRenderer { get; set; }

	protected override void OnAwake()
	{
		MyCell = this.GetComponent<GridCell>();
		MyRenderer = this.GetComponent<ModelRenderer>();
		MyTextRenderer = this.GetComponentInChildren<TextRenderer>();

		base.OnAwake();
	}

	protected override void OnUpdate()
	{
		if ( GameManager.Instance.GameState == GameState.Loss || GameManager.Instance.GameState == GameState.Win ) return;
		if ( Input.Pressed("attack1") )
		{
			var ray = GameManager.Instance.MainCamera.ScreenPixelToRay(Mouse.Position);
			var trace = Scene.Trace.Ray(ray, 5000f).Run();
			if ( trace.Hit && trace.GameObject == this.GameObject )
			{
				if ( GameManager.Instance.GameState == GameState.PreGame )
				{
					GameManager.Instance.GenerateMinefield();
					MyCell.HasBomb = false;
				}


				ShowCell(MyCell.XIndex, MyCell.YIndex);
			}
		}
		else if ( Input.Pressed("attack2"))
		{
			var ray = GameManager.Instance.MainCamera.ScreenPixelToRay(Mouse.Position);
			var trace = Scene.Trace.Ray(ray, 5000f).Run();
			if ( trace.Hit && trace.GameObject == this.GameObject )
			{
				if ( !MyCell.Shown )
				{
					if(MyRenderer.Tint == Color.Green )
					{
						MyRenderer.Tint = new Color(1, 0, 1);
					}
					else MyRenderer.Tint = Color.Green;
				}
			}
		}
	}
	private HashSet<(int x, int y)> visitedCells = new HashSet<(int x, int y)>();

	// TODO this is the least performant code I think I have ever written. It does NOT get more spaghetti than this. But it works! (mostly)
	private void ShowCell(int xIndex, int yIndex)
	{
		// Check if the cell is already shown or visited
		var cell = Grid.GlobalGrid.GetCell(xIndex, yIndex);
		if ( cell.Shown || visitedCells.Contains((xIndex, yIndex)) ) return;

		// Mark as shown and add to visited
		cell.Shown = true;
		visitedCells.Add((xIndex, yIndex));

		int bombs = cell.SurroundingBombs(xIndex, yIndex);

		if ( cell.HasBomb )
		{
			// Set cell tint and trigger loss
			MyRenderer.Tint = Color.Red;
			GameManager.Instance.GameState = GameState.Loss;
			return; // Exit recursion immediately on bomb
		}

		if ( bombs == 0 )
		{
			// Set cell tint for empty cells
			MyRenderer.Tint = Color.White;

			// Recursively show surrounding cells
			for ( int i = -1; i <= 1; i++ )
			{
				for ( int j = -1; j <= 1; j++ )
				{
					int xi = xIndex + i;
					int yj = yIndex + j;

					// Skip out-of-bound cells and the current cell
					if ( xi < 0 || yj < 0 || xi >= Grid.GlobalGrid.GridSizeX || yj >= Grid.GlobalGrid.GridSizeY || (i == 0 && j == 0) )
						continue;

					Grid.GlobalGrid.GetCell(xi, yj).GetComponent<MineController>()?.ShowCell(xi, yj);
				}
			}
		}
		else
		{
			// Set cell tint and display number of bombs
			MyRenderer.Tint = Color.Gray;
			MyTextRenderer.Text = bombs.ToString();
		}

		// Check if the game is finished only after all recursion completes
		if (Grid.GlobalGrid.GameFinished() )
		{
			GameManager.Instance.GameState = GameState.Win;
		}
	}
}

