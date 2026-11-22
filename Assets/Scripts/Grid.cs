using UnityEngine;

public class Grid
{
    private Vector2 cellSize;
    private Vector2 originPosition;
    
    public Grid(Vector2Int gridSize, Vector2 cellSize, Vector2 originPosition, bool debug = false)
    {
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        if(debug)
        {
            for (int xIndex = 0; xIndex <= gridSize.x; ++xIndex)
            {
                for (int yIndex = 0; yIndex <= gridSize.y; ++yIndex)
                {
                    Debug.DrawLine(GridToWorldPosition(xIndex, 0), GridToWorldPosition(xIndex, yIndex), Color.white, 100);
                    Debug.DrawLine(GridToWorldPosition(0, yIndex), GridToWorldPosition(xIndex, yIndex), Color.white, 100);
                }
            }
        }
    }
    
    public Vector2 GridToWorldPosition(float x, float y)
    {
        return new Vector2(x, y) * cellSize + originPosition;
    }
    
    public Vector2 GridToWorldPosition(Vector2 worldPosition)
    {
        return worldPosition * cellSize + originPosition;
    }
    
    public Vector2Int WorldToGridPosition(float x, float y)
    {
        Vector2Int position = new()
        {
            x = Mathf.FloorToInt(x / cellSize.x - originPosition.x),
            y = Mathf.FloorToInt(y / cellSize.y - originPosition.y)
        };
        
        return position;
    }
    
    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        Vector2Int position = new()
        {
            x = Mathf.FloorToInt(worldPosition.x / cellSize.x - originPosition.x),
            y = Mathf.FloorToInt(worldPosition.y / cellSize.y - originPosition.y)
        };

        return position;
    }
}