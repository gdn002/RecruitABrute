using UnityEngine;
using System.Collections;

public class DEBUG_MovementCalculatorTester : MonoBehaviour
{
    public Grid grid;
    public Vector2Int position = Vector2Int.zero;
    public int range = 2;

    public LineRenderer linePrefab;

    private MovementCalculator calculator;
    private LineRenderer line;

    // Use this for initialization
    void Start()
    {
        GameObject newObj = Instantiate(linePrefab.gameObject);
        newObj.transform.SetParent(transform, true);
        line = newObj.GetComponent<LineRenderer>();
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;
        line.useWorldSpace = true;
        line.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (calculator == null)
        {
            calculator = new MovementCalculator(grid);
            calculator.CalculateMovement(position, range);
            var tiles = calculator.GetReachableTiles();

            grid.HighlightTile(position, GridTile.TileHighlights.Friend);
            foreach (var tile in tiles)
            {
                grid.HighlightTile(tile, GridTile.TileHighlights.Movement);
            }
        }

        if (GridTile.CurrentlySelected != null)
        {
            var path = calculator.GetPath(GridTile.CurrentlySelected.Coordinates);

            if (path != null)
            {
                line.gameObject.SetActive(true);
                line.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    line.SetPosition(i, grid.GridToWorld(path[i], 0.1f));
                }
            }
            else
            {
                line.gameObject.SetActive(false);
            }
        }
        else
        {
            line.gameObject.SetActive(false);
        }
    }
}
