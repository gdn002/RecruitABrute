﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid ActiveGrid { get; private set; }

    // *** PROPERTY FIELDS ***

    public const float CELL_SIZE = 1;

    public Vector2Int gridSize = new Vector2Int(5, 5);

    public GridTile gridTilePrefab;


    // *** UTILITY VARIABLES ***

    // Stores all objects attached to this grid
    private List<Entity> entityList;

    // Stores all GridTiles
    private GridTile[,] gridTileArray;

    // Stores grid collision data
    public bool[,] collisionArray;

    private MovementCalculator movementCalculator;
    private static LineRenderer movementLine;

    // *** UTILITY FUNCTIONS ***
    
    // ** Coordinate Functions **

    // Convert grid coordinates to local position.
    public static Vector3 GridToLocal(Vector2 gridPosition, float heightOffset = 0)
    {
        return new Vector3(gridPosition.x * CELL_SIZE, heightOffset, gridPosition.y * CELL_SIZE);
    }

    // Convert local position to the nearest grid coordinates. (Y axis is ignored)
    public static Vector2Int LocalToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x + (CELL_SIZE / 2)) / CELL_SIZE), Mathf.FloorToInt((position.z + (CELL_SIZE / 2)) / CELL_SIZE));
    }

    // Convert grid coordinates to world position.
    public Vector3 GridToWorld(Vector2Int gridPosition, float heightOffset = 0)
    {
        return transform.TransformPoint(GridToLocal(gridPosition) + new Vector3(0, heightOffset, 0));
    }

    // Convert world position to the nearest grid coordinates. (Y axis is ignored)
    public Vector2Int WorldToGrid(Vector3 position)
    {
        return LocalToGrid(transform.InverseTransformPoint(position));
    }

    public int AttackDistance(Vector2Int coords1, Vector2Int coords2)
    {
        return Math.Max(Math.Abs(coords1.x - coords2.x), Math.Abs(coords1.y - coords2.y));
    }
    
    public int AttackDistance(Unit unit1, Unit unit2)
    {
        return AttackDistance(unit1.GetCoordinates(), unit2.GetCoordinates());
    }

    // ** Collision Functions **

    // Set collision flags in a grid cell. No effect if the coordinates are out of bounds of the collider array.
    public void SetCollision(Vector2Int index, bool collision)
    {
        if (IsInBounds(index))
        {
            collisionArray[index.x, index.y] = collision;
        }
    }

    // Get all collision flags from a grid cell. Out of bounds positions will always return central collision.
    public bool GetCollision(Vector2Int index)
    {
        if (IsInBounds(index))
        {
            return collisionArray[index.x, index.y];
        }

        return true;
    }

    public bool IsInBounds(Vector2Int index)
    {
        return (index.x >= 0 && index.y >= 0 && index.x < gridSize.x && index.y < gridSize.y);
    }

    // ** Movement Functions **

    public void CalculateMovement(Vector2Int coordinates, int movementRange)
    {
        movementCalculator.CalculateMovement(coordinates, movementRange);
    }
    
    public List<Vector2Int> GetReachableTiles()
    {
        return movementCalculator.GetReachableTiles();
    }

    public List<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
    {
        return movementCalculator.GetPath(from, to);
    }
    
    public void RenderPathLine(Vector2Int destination)
    {
        Vector2Int from = TurnTracker.ActiveTracker.ActiveUnit.GetCoordinates();
        var path = movementCalculator.GetPath(from, destination);

        if (path != null)
        {
            movementLine.gameObject.SetActive(true);
            movementLine.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                movementLine.SetPosition(i, GridToLocal(path[i], 0.1f));
            }
        }
        else
        {
            movementLine.gameObject.SetActive(false);
        }
    }

    public void HidePathLine()
    {
        movementLine.gameObject.SetActive(false);
    }

    // ** Grid Tile Functions **
    
    public void HighlightTile(Vector2Int index, GridTile.TileHighlights type)
    {
        gridTileArray[index.x, index.y].SetHighlight(type);
    }

    public void HighlightTiles(List<Vector2Int> tiles, GridTile.TileHighlights type)
    {
        foreach (Vector2Int tile in tiles)
        {
            HighlightTile(tile, type);
        }
    }
    
    public void ClearAllHighlights()
    {
        foreach (GridTile gridTile in gridTileArray)
        {
            gridTile.SetHighlight(GridTile.TileHighlights.None);
        }
    }

    // ** Entity Functions **

    public Entity GetEntity(Vector2Int coordinates)
    {
        // Limit search to entities with collision only
        // Non-collision entities will be used only as eye-candy, therefore shouldn't be relevant
        if (GetCollision(coordinates))
        {
            foreach (var entity in entityList)
            {
                if (entity.hasCollision && entity.coordinates == coordinates)
                    return entity;
            }
        }

        return null;
    }

    public Unit GetUnit(Vector2Int coordinates)
    {
        foreach (var entity in entityList)
        {
            if (entity.coordinates == coordinates)
                return entity.GetComponent<Unit>();
        }

        return null;
    }

    public List<Unit> GetAllUnits()
    {
        List<Unit> allUnits = new List<Unit>();
        foreach (var entity in entityList)
        {
            Unit unit = entity.GetComponent<Unit>();
            if (unit != null)
                allUnits.Add(unit);
        }

        return allUnits;
    }
    
    public List<Unit> GetAllFriendlyUnits()
    {
        return GetAllUnits().Where(unit => !unit.enemy).ToList();
    }

    public List<Unit> GetAllEnemyUnits()
    {
        return GetAllUnits().Where(unit => unit.enemy).ToList();
    }

    public void AddEntity(Entity entity)
    {
        entityList.Add(entity);
    }

    public bool RemoveEntity(Entity entity)
    {
        return entityList.Remove(entity);
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
            item.UpdateEntity();

            SetCollision(item.coordinates, item.hasCollision);

            // Add Units to TurnTracker (this might be revised once we have a proper setup phase implemented)
            Unit unit = item.GetComponent<Unit>();
            if (unit != null)
            {
                // If you get an error at this line, check if your scene has a TurnTracker
                TurnTracker.ActiveTracker.AddToInitiative(unit);
            }
        }
    }

    private void InitializeTiles()
    {
        if (gridTilePrefab != null)
        {
            gridTileArray = new GridTile[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    GameObject tile = Instantiate(gridTilePrefab.gameObject);
                    gridTileArray[x, y] = tile.GetComponent<GridTile>();
                    gridTileArray[x, y].Initialize(transform, new Vector2Int(x, y));
                }
            }
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    void Awake()
    {
        if (ActiveGrid == null)
        {
            ActiveGrid = this;
        }
        else
        {
            Debug.LogError("A Grid component was initialized while another one is already running: " + gameObject.name);
        }
        
        InitializeEntities();
        InitializeTiles();

        movementCalculator = new MovementCalculator();

        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/Interface/PathLine"));
        newObj.transform.SetParent(transform, true);
        movementLine = newObj.GetComponent<LineRenderer>();
        movementLine.startColor = Color.cyan;
        movementLine.endColor = Color.cyan;
        movementLine.useWorldSpace = false;
        movementLine.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestroy()
    {
        if (ActiveGrid == this)
        {
            ActiveGrid = null;
        }
    }

    public void MouseDownGridTile(Vector2Int coordinates)
    {
        gridTileArray[coordinates.x, coordinates.y].OnMouseDown();
    }
    
    public void MouseOverGridTile(Vector2Int coordinates)
    {
        gridTileArray[coordinates.x, coordinates.y].OnMouseOver();
    }
    
    public void MouseExitGridTile(Vector2Int coordinates)
    {
        gridTileArray[coordinates.x, coordinates.y].OnMouseExit();
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
