using UnityEngine;
using System.Collections;

public class DEBUG_MovementCalculatorTester : MonoBehaviour
{
    public Grid grid;
    public Vector2Int position = Vector2Int.zero;
    public int range = 2;

    private bool testRun = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!testRun)
        {
            MovementCalculator calculator = new MovementCalculator(grid);
            calculator.CalculateMovement(position, range);
            var tiles = calculator.GetReachableTiles(position);

            grid.HighlightTile(position, GridCell.TileHighlights.Friend);
            foreach (var tile in tiles)
            {
                grid.HighlightTile(tile, GridCell.TileHighlights.Movement);
            }

            testRun = true;
        }
    }
}
