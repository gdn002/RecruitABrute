using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : MonoBehaviour
{
    // Global access to the currently active tracker; only one TurnTracker should be active at once.
    public static TurnTracker ActiveTracker { get; private set; }

    public enum GamePhase
    {
        Setup = 0,
        Combat = 1,
        Reward = 2,
    }

    public enum InputMode
    {
        Movement = 0,
        Target,
    }

    // Keep track of turns and rounds
    public int TurnCounter { get; private set; }
    public int RoundCounter { get; private set; }
    public GamePhase CurrentPhase {get; private set;}

    // Keep track of initiative
    public List<Unit> InitiativeOrder { get; private set; }

    // Keep track of input
    private InputMode inputMode = InputMode.Movement;

    public Unit ActiveUnit
    {
        get { return InitiativeOrder[TurnCounter]; }
    }

    public Vector2Int ActiveUnitStartCoordinates;
    private GridTile lastSelected;

    public void AddToInitiative(Unit unit)
    {
        // Find the Unit's place in initiative
        for (int i = 0; i < InitiativeOrder.Count; i++)
        {
            if (InitiativeOrder[i].initiative < unit.initiative)
            {
                InitiativeOrder.Insert(i, unit);
                return;
            }
        }

        InitiativeOrder.Add(unit);
    }

    public void RemoveFromInitiative(Unit unit)
    {
        for (int i = 0; i < InitiativeOrder.Count; i++)
        {
            if (InitiativeOrder[i] == unit)
            {
                InitiativeOrder.RemoveAt(i);
                if (i < TurnCounter)
                    TurnCounter--;
                // This still doesn't handle the possibility that the currently active unit dies in its own turn
            }
        }
    }

    public void NextPhase()
    {
        if (CurrentPhase != GamePhase.Reward)
        {
            CurrentPhase++;

            // Phase change callbacks
            switch(CurrentPhase)
            {
                case GamePhase.Combat:
                    OnEnterCombatPhase();
                    break;
                case GamePhase.Reward:
                    //OnEnterRewardPhase();
                    break;
            }
        }
    }

    public void NextTurn()
    {
        if (CurrentPhase != GamePhase.Combat)
            return;

        OnTurnEnd();

        TurnCounter++;
        if (TurnCounter >= InitiativeOrder.Count)
        {
            TurnCounter = 0;
            RoundCounter++;
        }

        OnTurnStart();
    }

    

    private void OnTurnEnd()
    {
        ActiveUnit.Deactivate();

    }

    private void OnTurnStart()
    {
        ActiveUnit.Activate();
        ActiveUnitStartCoordinates = ActiveUnit.GetCoordinates();

        SetInputMode(InputMode.Movement);

        Debug.Log("Turn " + TurnCounter + ", Round " + RoundCounter + ", Active Unit: " + ActiveUnit.gameObject.name);
    }

    private void OnEnterCombatPhase()
    {
        // First Turn

        OnTurnStart();
    }


    // *** HIGHLIGHT MODES ***

    // Highlights the tiles the currently active unit can move to
    private void SetMovementHighlight()
    {
        Grid.ActiveGrid.ClearAllHighlights();
        Grid.ActiveGrid.CalculateMovement(ActiveUnitStartCoordinates, ActiveUnit.movementRange);
        Grid.ActiveGrid.HighlightTiles(Grid.ActiveGrid.GetReachableTiles(), GridTile.TileHighlights.Movement);
        Grid.ActiveGrid.HighlightTile(ActiveUnitStartCoordinates, GridTile.TileHighlights.Friend);
    }

    // Highlights the tiles the currently active unit can target (currently only works for the base attack)
    private void SetTargetHighlight()
    {
        Grid.ActiveGrid.ClearAllHighlights();
        Grid.ActiveGrid.HighlightTiles(ActiveUnit.baseAttack.GetValidTargets(ActiveUnit), GridTile.TileHighlights.AoE);
    }


    // *** INPUT MODES ***

    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;

        switch (inputMode)
        {
            case InputMode.Movement:
                SetMovementHighlight();
                break;

            case InputMode.Target:
                SetTargetHighlight();
                break;
        }
    }

    private void HandleMovementInput()
    {
        GridTile selection = GridTile.CurrentlySelected;
        if (lastSelected != selection)
        {
            // Update pathline
            if (selection == null)
            {
                Grid.ActiveGrid.HidePathLine();
            }
            else
            {
                Grid.ActiveGrid.RenderPathLine(selection.Coordinates);
            }

            lastSelected = selection;
        }
        else if (selection == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Move unit to selected tile if it is reachable
            if (selection.GetHighlight() == GridTile.TileHighlights.Movement ||
                selection.GetHighlight() == GridTile.TileHighlights.Friend)
            {
                ActiveUnit.UnitEntity.Move(selection.Coordinates);
            }
        }
      
    }

    private void HandleTargetInput()
    {
        GridTile selection = GridTile.CurrentlySelected;
        if (selection == null) return;


        if (Input.GetMouseButtonDown(0))
        {
            // Use skill on selected target if it is valid
            if (selection.GetHighlight() == GridTile.TileHighlights.AoE)
            {
                ActiveUnit.baseAttack.ActivateSkill(ActiveUnit, selection.Coordinates);
                NextTurn();
            }
        }
    }


    void Awake()
    {
        InitiativeOrder = new List<Unit>();

        if (ActiveTracker == null)
        {
            ActiveTracker = this;
        }
        else
        {
            Debug.LogError("A TurnTracker component was initialized while another one is already running: " +
                           gameObject.name);
        }

        TurnCounter = 0;
        RoundCounter = 0;
        CurrentPhase = GamePhase.Setup;
    }

    void Start()
    {
    
    }

    void Update()
    {
        // This is only for testing while we don't have the scripts/interface to call NextTurn externally
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextTurn();
        }

        // This is only for testing while we don't have the scripts/interface to select/deselect skills
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inputMode == InputMode.Movement)
                SetInputMode(InputMode.Target);
            else
                SetInputMode(InputMode.Movement);
        }

        if (CurrentPhase == GamePhase.Combat)
        {
            switch (inputMode)
            {
                case InputMode.Movement:
                    HandleMovementInput();
                    break;

                case InputMode.Target:
                    HandleTargetInput();
                    break;
            }
        }
    }

    void OnDestroy()
    {
        if (ActiveTracker == this)
        {
            ActiveTracker = null;
        }
    }
}