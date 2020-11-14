using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GridArea
{
    public Vector2Int pivot;
    public byte[,] kernel;

    public GridArea()
    {
        kernel = null;
        pivot = Vector2Int.zero;
    }

    public GridArea(byte[,] kernel)
    {
        this.kernel = kernel;
        SetPivotAtCenter();
    }

    public GridArea(byte[,] kernel, Vector2Int pivot)
    {
        this.kernel = kernel;
        this.pivot = pivot;
    }

    public void SetPivotAtCenter()
    {
        pivot = new Vector2Int(kernel.GetLength(0) / 2, kernel.GetLength(1) / 2);
    }

    public List<Vector2Int> GetPlacement(Grid grid, Vector2Int center)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        Vector2Int origin = center - pivot;
        for (int x = 0; x < kernel.GetLength(0); x++)
        {
            for (int y = 0; y < kernel.GetLength(1); y++)
            {
                if (kernel[x, y] == 0) continue;

                Vector2Int tile = origin + new Vector2Int(x, y);
                if (grid.IsInBounds(tile))
                    tiles.Add(tile);
            }
        }

        return tiles;
    }
}