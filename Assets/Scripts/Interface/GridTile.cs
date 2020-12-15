using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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

    public Vector2Int Coordinates { get; private set; }

    private new Renderer renderer;
    private TileHighlights currentHighlight = TileHighlights.None;

    public void Initialize(Transform parent, Vector2Int coordinates)
    {
        transform.SetParent(parent);

        Vector3 position = Grid.GridToLocal(coordinates);
        transform.localPosition = position;

        Coordinates = coordinates;
    }

    public void SetHighlight(TileHighlights type)
    {
        currentHighlight = type;
        UpdateHighlight();
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
        if (CurrentlySelected == this) return Color.cyan;

        switch (currentHighlight)
        {
            case TileHighlights.Movement:
                return new Color(0.5f, 0.5f, 0.5f);//Gray
            case TileHighlights.Friend:
                return new Color(0f, 0.5f, 0f);//Dark green
            case TileHighlights.AoE:
                return new Color(1f, 0.5f, 0f);//Orange
            case TileHighlights.Foe:
                return Color.red;
            case TileHighlights.ActiveUnit:
                return new Color(0f, 1f, 0f);//Light green
        }

        return Color.white;
    }


    // *** MOUSE EVENTS ***
    public void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        CurrentlySelected = this;
        TurnTracker.ActiveTracker.UpdateHighlight();    
    }

    public void OnMouseExit()
    {
        CurrentlySelected = null;
        TurnTracker.ActiveTracker.UpdateHighlight();
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        switch (TurnTracker.ActiveTracker.CurrentPhase)
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
                    TurnTracker.ActiveTracker.UpdateHighlight();
                }
                break;

            case TurnTracker.GamePhase.Combat:
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
        transform.localScale = new Vector3(Grid.CELL_SIZE - gapBetweenCells, 1, Grid.CELL_SIZE - gapBetweenCells);
        renderer = GetComponentInChildren<Renderer>();
        UpdateHighlight();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentlySelected == this)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                OnMouseExit();
        }
    }
}