using UnityEngine;
using System.Collections;

public class DEBUG_GridAreaTester : MonoBehaviour
{
    public Grid grid;
    public Vector2Int position = Vector2Int.zero;
    public Vector2Int areaPivot = Vector2Int.zero;

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
            // IMPORTANT: As currently is, this kernel will be flipped vertically
            // (due to how this initialization is being done)
            GridArea area = new GridArea(new byte[5, 5] { { 1, 0, 0, 0, 1},
                                                          { 0, 1, 0, 1, 0},
                                                          { 0, 0, 1, 0, 0},
                                                          { 0, 1, 0, 1, 0},
                                                          { 1, 0, 0, 0, 1} });
            area.pivot = areaPivot;

            var tiles = area.GetPlacement(grid, position);
            foreach (var tile in tiles)
            {
                grid.UpdateHighlighting();
            }
            grid.UpdateHighlighting();

            testRun = true;
        }
    }
}
