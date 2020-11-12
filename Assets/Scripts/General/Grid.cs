﻿using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public const float CELL_SIZE = 1;

    public Vector2Int gridSize = new Vector2Int(5, 5);

    public GridCell gridCellPrefab;


    // *** UTILITY VARIABLES ***

    // Stores all objects attached to this grid
    private List<Entity> entityList;

    // Stores all GridCells
    private GridCell[,] cellArray;

    // Stores grid cell collision data
    public bool[,] collisionArray;


    // *** UTILITY FUNCTIONS ***

    // ** Coordinate Functions **

    // Convert grid coordinates to local position. (Y axis is set to verticalPosition)
    public static Vector3 GridToLocal(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * CELL_SIZE, 0, gridPosition.y * CELL_SIZE);
    }

    // Convert local position to the nearest grid coordinates. (Y axis is ignored)
    public static Vector2Int LocalToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x + (CELL_SIZE / 2)) / CELL_SIZE), Mathf.FloorToInt((position.z + (CELL_SIZE / 2)) / CELL_SIZE));
    }

    // Convert grid coordinates to world position. (Y axis is set to verticalPosition)
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return transform.TransformPoint(GridToLocal(gridPosition));
    }

    // Convert world position to the nearest grid coordinates. (Y axis is ignored)
    public Vector2Int WorldToGrid(Vector3 position)
    {
        return LocalToGrid(transform.InverseTransformPoint(position));
    }


    // ** Collision Functions **

    // Set collision flags in a grid cell. No effect if the coordinates are out of bounds of the collider array.
    public void SetCollision(Vector2Int index, bool collision)
    {
        if (IsInCollisionArray(index))
        {
            collisionArray[index.x, index.y] = collision;
        }
    }

    // Get all collision flags from a grid cell. Out of bounds positions will always return central collision.
    public bool GetCollision(Vector2Int index)
    {
        if (IsInCollisionArray(index))
        {
            return collisionArray[index.x, index.y];
        }

        return true;
    }

    private bool IsInCollisionArray(Vector2Int index)
    {
        return (index.x >= 0 && index.y >= 0 && index.x < collisionArray.GetLength(0) && index.y < collisionArray.GetLength(1));
    }

    // *** INITIALIZATION FUNCTIONS ***
    private void InitializeEntities()
    {
        entityList = new List<Entity>();
        collisionArray = new bool[gridSize.x, gridSize.y];

        // Locate all existing entities on this grid
        entityList.AddRange(GetComponentsInChildren<Entity>());
        Debug.Log(entityList.Count + " entities found in grid: " + name);

        // Initialize and update all entities on the grid
        foreach (var item in entityList)
        {
            item.Initialize(this);
            item.UpdateEntity();

            SetCollision(item.coordinates, item.hasCollision);
        }
    }

    private void InitializeCells()
    {
        if (gridCellPrefab != null)
        {
            cellArray = new GridCell[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    GameObject cell = Instantiate(gridCellPrefab.gameObject);
                    cellArray[x, y] = cell.GetComponent<GridCell>();
                    cellArray[x, y].Initialize(transform, new Vector2Int(x, y));
                }
            }
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        InitializeEntities();
        InitializeCells();
    }

    // Update is called once per frame
    void Update()
    {
    }


#if UNITY_EDITOR
    // *** DEBUG FUNCTIONS ***

    public bool showGrid = false;
    public bool showCollisions = false;

    private void OnDrawGizmos()
    {
        if (showGrid) DEBUG_DrawGrid();
        if (showCollisions) DEBUG_DrawCollisions();
    }

    private void DEBUG_DrawGrid()
    {
        Gizmos.color = Color.white;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Gizmos.DrawWireCube(GridToWorld(new Vector2Int(x, y)), new Vector3(CELL_SIZE, 0, CELL_SIZE));
            }
        }
    }

    private void DEBUG_DrawCollisions()
    {
        if (collisionArray == null) return;

        Gizmos.color = Color.red;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (collisionArray[x,y])
                {
                    Gizmos.DrawWireCube(GridToWorld(new Vector2Int(x, y)), new Vector3(CELL_SIZE, 0, CELL_SIZE));
                }
            }
        }
    }
#endif
}
