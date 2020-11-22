using System;
using UnityEngine;
using System.Collections;

public class GridTile : MonoBehaviour
{
    // Returns the currently selected tile, if there is any
    public static GridTile CurrentlySelected { get; private set; }


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

    public Vector2Int Coordinates { get; private set; }

    private new Renderer renderer;
    private TileHighlights currentHighlight = TileHighlights.None;
    private bool isSelected = false;

    public void Initialize(Transform parent, Vector2Int coordinates)
    {
        transform.SetParent(parent);

        Vector3 position = Grid.GridToLocal(coordinates);
        position.y = -(height / 2);
        transform.localPosition = position;

        Coordinates = coordinates;
    }

    public void SetHighlight(TileHighlights type)
    {
        if (currentHighlight != type)
        {
            currentHighlight = type;
            UpdateHighlight();
        }
    }

    public TileHighlights GetHighlight()
    {
        return currentHighlight;
    }

    public void UpdateHighlight()
    {
        renderer.material.color = GetHighlightColor();
    }

    private Color GetHighlightColor()
    {
        if (isSelected) return Color.cyan;

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


    // *** MOUSE EVENTS ***
    void OnMouseOver()
    {
        isSelected = true;
        CurrentlySelected = this;
        UpdateHighlight();    
    }

    void OnMouseExit()
    {
        isSelected = false;
        CurrentlySelected = null;
        UpdateHighlight();
    }

    void OnMouseDown()
    {
        switch(TurnTracker.ActiveTracker.CurrentPhase)
        {
            case TurnTracker.GamePhase.Setup:
                if (DeckHandler.MainDeckHandler.selectedUnit != null)
                {
                    Unit u = DeckHandler.MainDeckHandler.selectedUnit;
                    u.gameObject.SetActive(true);
                    u.transform.SetParent(Grid.ActiveGrid.transform);
                    u.UnitEntity.Move(Coordinates);
                    TurnTracker.ActiveTracker.AddToInitiative(u);
                    TurnTracker.ActiveTracker.NextPhase(); // Only allows one unit for now, this should be changed later
                }
                break;
        }
    }

    void OnMouseUp()
    {
    }

    // *** MONOBEHAVIOUR FUNCTIONS ***
    private void Awake()
    {
        transform.localScale = new Vector3(Grid.CELL_SIZE - gapBetweenCells, height, Grid.CELL_SIZE - gapBetweenCells);

        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}