using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementCalculator
{
    private Grid grid;

    private Vector2Int GridSize { get { return grid.gridSize; } }
    private bool[,] CollisionArray { get { return grid.collisionArray; } }

    private int[,] distanceArray;
    private Vector2Int[,] pathArray;
    private Queue<Vector2Int> pathQueue;

    public MovementCalculator(Grid grid)
    {
        this.grid = grid;
        distanceArray = new int[GridSize.x, GridSize.y];
        pathArray = new Vector2Int[GridSize.x, GridSize.y];
        pathQueue = new Queue<Vector2Int>();
    }

    public void CalculateMovement(Vector2Int from, int range)
    {
        Reset();

        // Get started with source node
        pathQueue.Enqueue(from);
        distanceArray[from.x, from.y] = 0;

        // Run BFS
        BreadthFirstSearch(range);
    }

    public List<Vector2Int> GetReachableTiles(Vector2Int from)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                if (distanceArray[x, y] > 0)
                    tiles.Add(new Vector2Int(x, y));
            }
        }

        return tiles;
    }

    public List<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
    {
        // Check if destination was reached
        if (distanceArray[to.x, to.y] < 0) return null;

        // Retrace steps
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = to;
        while (current != from)
        {
            path.Add(current);
            current = pathArray[current.x, current.y];
        }

        // Add the start to close the path off
        path.Add(from);
        // Reverse so the path goes start to finish 
        // NOTE: We might be able to optimize this out in the future
        path.Reverse();
        return path;
    }

    private bool BreadthFirstSearch(int range)
    {
        Vector2Int current;
        while (pathQueue.Count > 0)
        {
            current = pathQueue.Dequeue();

            // Check range
            if (distanceArray[current.x, current.y] >= range)
                continue;

            // Advance on all four directions
            Step(current, Vector2Int.up);
            Step(current, Vector2Int.down);
            Step(current, Vector2Int.left);
            Step(current, Vector2Int.right);
        }

        // Path not found
        return false;
    }

    private void Step(Vector2Int current, Vector2Int step)
    {
        Vector2Int next = current + step;

        // Boundary check
        if (!grid.IsInBounds(next)) return;

        // Collision check
        if (CollisionArray[next.x, next.y]) return;

        // Visited tiles check
        if (distanceArray[next.x, next.y] >= 0) return;

        int distance = distanceArray[current.x, current.y] + 1;

        // Keep track of the path
        pathArray[next.x, next.y] = current;
        distanceArray[next.x, next.y] = distance;

        pathQueue.Enqueue(next);
    }

    private void Reset()
    {
        pathQueue.Clear();

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                distanceArray[x, y] = -1;
                pathArray[x, y] = Vector2Int.zero;
            }
        }
    }
}
