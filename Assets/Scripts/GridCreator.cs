using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridCreator : MonoBehaviour
{
    public static GridCreator Instance { get; private set; }
    
    private Grid grid;
    
    [SerializeField, Min(4)] private Vector2Int gridSize;
    [SerializeField, Min(0)] private Vector2 cellSize;
    [SerializeField, Min(1), Space(5)] private int bombAmount;
    
    [SerializeField, Space(5)] private GameObject cellPrefab;
    
    [SerializeField, Space(15)] private bool debug;

    private Cell startCell;
    
    private Cell[,] spawnedCells;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (cellPrefab == null)
        {
            Debug.LogWarning("CellPrefab is null!");
            return;
        }
        
        Vector2 originPosition = (Vector2)gridSize / 2 * cellSize * -1;
        grid = new Grid(gridSize, cellSize, originPosition, debug);
        spawnedCells = new Cell[gridSize.x, gridSize.y];
        
        CreateCells();
    }
    
    private void CreateCells()
    {
        for (int i = 0; i < spawnedCells.GetLength(0); ++i)
        {
            for (int j = 0; j < spawnedCells.GetLength(1); ++j)
            {
                var cell = Instantiate(cellPrefab,
                    grid.GridToWorldPosition(i + cellSize.x / 2, j + cellSize.y / 2),
                    Quaternion.identity).GetComponent<Cell>();
                
                cell.transform.localScale = cellSize;
                
                spawnedCells[i, j] = cell;
            }
        }
    }

    public void StartGrid(Cell cell)
    {
        if (!startCell)
        {
            startCell = cell;
            
            AssignBombs();
            CountBombs();
        }
    }
    
    private void AssignBombs()
    {
        int bombs = 0;
        int x, y;
        
        while (bombs < bombAmount)
        {
            x = Random.Range(0, spawnedCells.GetLength(0));
            y = Random.Range(0, spawnedCells.GetLength(1));

            var checkingCell = spawnedCells[x, y];
            
            if(!checkingCell.IsBomb && checkingCell != startCell && !GetAroundCells(checkingCell).Contains(startCell))
            {
                spawnedCells[x, y].SetAsBomb();
                ++bombs;
            }
        }
    }
    
    private void CountBombs()
    {
        for (int x = 0; x < spawnedCells.GetLength(0); ++x)
        {
            for (int y = 0; y < spawnedCells.GetLength(1); ++y)
            {
                if (spawnedCells[x, y].IsBomb)
                {
                    Cell[] cellsAround = GetAroundCells(spawnedCells[x, y]);
                    
                    foreach (var cell in cellsAround)
                    {
                        if (!cell.IsBomb)
                        {
                            cell.AddBombCount();
                        }
                    }
                }
            }
        }
    }
    
    public Cell[] GetAroundCells(Cell cell)
    {
        List<Cell> cells = new();
        Cell aroundCell;
        Vector2Int cellCoord = grid.WorldToGridPosition(cell.gameObject.transform.position);
        Vector2Int coordToCheck = new();
        
        int yIndex = -1;

        for (int i = 0; i < 9; ++i)
        {
            if (i % 3 == 0 && i != 0)
            {
                ++yIndex;
            }
            
            coordToCheck.x = cellCoord.x + (i % 3 - 1);
            coordToCheck.y = cellCoord.y + yIndex;
            
            if(coordToCheck != cellCoord)
            {
                if (coordToCheck.x >= 0 && coordToCheck.x < spawnedCells.GetLength(0) &&
                    coordToCheck.y >= 0 && coordToCheck.y < spawnedCells.GetLength(1))
                {
                    aroundCell = spawnedCells[coordToCheck.x, coordToCheck.y];
                    cells.Add(aroundCell);
                }
            }
        }
        
        return cells.ToArray();
    }

    public void Restart()
    {
        foreach (var cell in spawnedCells)
        {
            cell.Restart();
        }
        
        startCell = null;
        
        GameManager.Instance.ResetValues();
    }
    
    public int BombAmount => bombAmount;
    public int CellsAmount => spawnedCells.GetLength(0) *  spawnedCells.GetLength(1);
    
    private void OnValidate()
    {
        if (cellPrefab != null && !cellPrefab.TryGetComponent<Cell>(out _))
        {
            cellPrefab = null;
        }

        if (bombAmount >= gridSize.x * gridSize.y - 9)
        {
            bombAmount = gridSize.x * gridSize.y - 9;
        }
    }
}
