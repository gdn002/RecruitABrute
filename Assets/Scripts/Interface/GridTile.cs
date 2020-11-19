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
        ActiveUnit,
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
                return Color.Lerp(Color.red, Color.yellow, 0.5f);//Orange
            case TileHighlights.Friend:
                return Color.green;
            case TileHighlights.Foe:
                return Color.red;
            case TileHighlights.ActiveUnit:
                return Color.yellow;
        }

        return Color.white;
    }


    // *** MOUSE EVENTS ***
    void OnMouseOver()
    {
        isSelected = true;
        CurrentlySelected = this;
        UpdateHighlight();

        if (TurnTracker.ActiveTracker.CurrentPhase == TurnTracker.GamePhase.Combat)
            Grid.ActiveGrid.RenderPathLine(Coordinates);
    }

    void OnMouseExit()
    {
        isSelected = false;
        CurrentlySelected = null;
        UpdateHighlight();
        Grid.ActiveGrid.HidePathLine();
    }

    void OnMouseDown()
    {
        switch(TurnTracker.ActiveTracker.CurrentPhase)
        {
            case TurnTracker.GamePhase.Setup:
                if (DeckHandler.MainDeckHandler.selectedUnit != null)
                {
                    Unit u = DeckHandler.MainDeckHandler.selectedUnit;
                    DeckHandler.MainDeckHandler.selectedUnit = null;
                    u.gameObject.SetActive(true);
                    u.transform.SetParent(Grid.ActiveGrid.transform);
                    u.UnitEntity.Move(Coordinates);
                    Grid.ActiveGrid.AddEntity(u.UnitEntity);
                    TurnTracker.ActiveTracker.AddToInitiative(u);
                    TurnTracker.ActiveTracker.PlaceUnit();
                    Grid.ActiveGrid.UpdateHighlighting();
                }
                break;

            case TurnTracker.GamePhase.Combat:
                // Move unit to this tile if it is reachable
                Unit unit = TurnTracker.ActiveTracker.ActiveUnit;
                if (Grid.ActiveGrid.GetReachableTiles().Contains(Coordinates) && !unit.UnitEntity.IsMoving)
                {
                    unit.UnitEntity.StartMoveAnimation(Coordinates);
                }
                break;

            case TurnTracker.GamePhase.Reward:
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