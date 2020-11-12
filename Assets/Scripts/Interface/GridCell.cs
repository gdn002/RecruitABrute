using UnityEngine;
using System.Collections;

public class GridCell : MonoBehaviour
{
    public enum TileHighlights
    {
        None = 0,
        Movement,
        AoE,
        Friend,
        Foe,
    }

    public float gapBetweenCells = 0.01f;
    public float height = 0.1f;

    private new Renderer renderer;
    private TileHighlights currentHighlight = TileHighlights.None;

    public void Initialize(Transform parent, Vector2Int coordinates)
    {
        transform.SetParent(parent);

        Vector3 position = Grid.GridToLocal(coordinates);
        position.y = -(height / 2);
        transform.localPosition = position;
    }

    public void SetHighlight(TileHighlights type)
    {
        if (currentHighlight != type)
        {
            currentHighlight = type;
            renderer.material.color = GetHighlightColor();
        }
    }

    private Color GetHighlightColor()
    {
        switch (currentHighlight)
        {
            case TileHighlights.Movement:
                return Color.blue;
            case TileHighlights.AoE:
                return Color.yellow;
            case TileHighlights.Friend:
                return Color.green;
            case TileHighlights.Foe:
                return Color.red;
        }

        return Color.white;
    }

    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(Grid.CELL_SIZE - gapBetweenCells, height, Grid.CELL_SIZE - gapBetweenCells);

        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
