using Sandbox;
using Sandbox.Diagnostics;
using System.Collections;
using static Sandbox.PhysicsGroupDescription.BodyPart;
public sealed class GridManager : Component
{
	private static GridManager instance;
	public static GridManager Instance
	{
		get
		{
			return instance;
		}
		set
		{
			if ( instance != null )
			{
				value.Destroy();
			}
			instance = value;
		}
	}

	public int GridSizeX = 5;
	public int GridSizeY = 5;

	protected override void OnAwake()
	{
		Assert.NotNull(GameManager.Instance.CubePrefab);

		GenerateGrid();
		Instance = this;

		base.OnAwake();
	}

	public void GenerateGrid()
	{
		if ( Grid.GlobalGrid?.Count() > 0 )
		{
			foreach ( GridCell cell in Grid.GlobalGrid )
			{
				cell.GameObject?.Destroy();
			}
		}

		Grid.GlobalGrid = new Grid(50f, GridSizeX, GridSizeY);


		for ( int i = 0; i < Grid.GlobalGrid.GridSizeX; i++ )
		{
			for ( int j = 0; j < Grid.GlobalGrid.GridSizeY; j++ )
			{
				GridCell cell = new GridCell(i, j);
				GameObject cube = GameManager.Instance.CubePrefab.Clone(this.GameObject, cell.Position, Rotation.Identity, new Vector3(1, 1, 1));
				GridCell c = cube.GetComponent<GridCell>();
				c.XIndex = i;
				c.YIndex = j;
				c.Position = cell.Position;
				c.HasBomb = false;
				c.Shown = false;
				Grid.GlobalGrid.AddCell(cube.GetComponent<GridCell>());
			}
		}
	}
}

public class GridCell : Component
{
	[Property]
	public Vector3 Position { get; set; }
	[Property] public int XIndex { get; set; }
	[Property] public int YIndex { get; set; }

	public bool Shown = false;

	private bool hasBomb;
	[Property] public bool HasBomb { get => hasBomb;
		set
		{
			//GetComponent<ModelRenderer>().Tint = value ? Color.White : Color.Black;

			hasBomb = value;
		} 
	}

	public GridCell() { }

	public GridCell(int _x, int _y)
	{
		Position = new Vector3(_x * Grid.GlobalGrid.CellSize, _y * Grid.GlobalGrid.CellSize, 22.5f);
		XIndex = _x;
		YIndex = _y;
	}

	public GridCell(GridCell _cell)
	{
		Position = _cell.Position;
		XIndex = _cell.XIndex;
		YIndex = _cell.YIndex;
	}
	private int surroundingBombs = 0;
	private bool foundSurroundings = false;
	public int SurroundingBombs(int x, int y)
	{
		if ( foundSurroundings ) return surroundingBombs;
		for ( int i = -1; i <= 1; i++ )
		{
			for ( int j = -1; j <= 1; j++ )
			{
				int xi = x+i; 
				int yj = y+j;
				if(xi < 0 || yj < 0 || xi > Grid.GlobalGrid.GridSizeX - 1 || yj > Grid.GlobalGrid.GridSizeY - 1) continue;
				if ( Grid.GlobalGrid.GetCell(xi, yj).HasBomb ) surroundingBombs++;
			}
		}
		foundSurroundings = true;
		return surroundingBombs;
	}

	public override string ToString()
	{
		return $"(X, Y): ({XIndex}, {YIndex})";
	}
}

public class Grid : IEnumerable<GridCell>
{
	public static Grid GlobalGrid { get; set; }
	// The size of an individual grid cell.
	public float CellSize { get; set; }

	// The size of the grid as GridSizeX x GridSizeY
	public int GridSizeX { get; set; }
	public int GridSizeY { get; set; }

	public List<GridCell> GridCells = new List<GridCell>();

	public Grid(float _cellSize, int _gridSizeX, int _gridSizeY)
	{
		CellSize = _cellSize;
		GridSizeX = _gridSizeX;
		GridSizeY = _gridSizeY;
		GridCells = new List<GridCell>();
	}

	public bool GameFinished()
	{
		return GlobalGrid.All(c => c.HasBomb || c.Shown);
	}

	public GridCell GetCell(int xIndex, int yIndex)
	{
		// Loop through all grid cells and return the one that matches the x and y indexes
		return GridCells.FirstOrDefault(cell => cell.XIndex == xIndex && cell.YIndex == yIndex);
	}

	public void AddCell(GridCell cell)
	{
		GridCells.Add(cell);
	}

	public IEnumerator<GridCell> GetEnumerator()
	{
		return GridCells.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
