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

	// TODO this is the least performant code I think I have ever written. But it works!
	private void ShowCell(int xIndex, int yIndex)
	{
		if ( MyCell.Shown ) return;
		MyCell.Shown = true;

		int bombs = MyCell.SurroundingBombs(xIndex, yIndex);

		if ( MyCell.HasBomb )
		{
			MyRenderer.Tint = Color.Red;

			GameManager.Instance.GameState = GameState.Loss;
		}
		else if ( bombs == 0 )
		{
			MyRenderer.Tint = Color.White;
			
			for ( int i = -1; i <= 1; i++ )
			{
				for ( int j = -1; j <= 1; j++ )
				{
					int xi = xIndex + i;
					int yj = yIndex + j;
					if ( xi < 0 || yj < 0 || xi > Grid.GlobalGrid.GridSizeX - 1 || yj > Grid.GlobalGrid.GridSizeY - 1 ) continue;
					Grid.GlobalGrid.GetCell(xi, yj).GetComponent<MineController>().ShowCell(xi, yj);
				}
			}
		}
		else
		{
			MyRenderer.Tint = Color.Gray;
			MyTextRenderer.Text = bombs.ToString();
		}
		if ( Grid.GlobalGrid.GameFinished() )
			GameManager.Instance.GameState = GameState.Win;
	}
}

