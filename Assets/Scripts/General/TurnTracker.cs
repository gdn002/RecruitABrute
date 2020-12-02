using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : MonoBehaviour
{
    private const int UNIT_CAP = 5;
    
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
    public int UnitsPlaced { get; private set; }

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

    public void PlaceUnit()
    {
        if (CurrentPhase != GamePhase.Setup)
            return;

        UnitsPlaced++;

        if (UnitsPlaced >= UNIT_CAP)
        {
            NextPhase();
        }
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

        if (ActiveUnit.HasAI)
        {
            ActiveUnit.AttachedAI.FindTarget();
            StartCoroutine(ActiveUnit.AttachedAI.CommitActions());
        }
    }

    private void OnEnterCombatPhase()
    {
        // First Turn

        OnTurnStart();
    }


    // *** HIGHLIGHT MODES ***

    // Updates all highlights
    public void UpdateHighlight()
    {
        UpdatePathLine();
        Grid.ActiveGrid.ClearAllHighlights();

        if (CurrentPhase == GamePhase.Combat)
        {
            switch (inputMode)
            {
                case InputMode.Movement:
                    SetMovementHighlight();
                    SetUnitHighlight();
                    break;

                case InputMode.Target:
                    SetUnitHighlight();
                    SetTargetHighlight();
                    break;
            }
        }
        else
        {
            SetUnitHighlight();
        }
    }

    public void UpdatePathLine()
    {
        if (GridTile.CurrentlySelected == null || inputMode != InputMode.Movement)
        {
            Grid.ActiveGrid.HidePathLine();
        }
        else
        {
            Grid.ActiveGrid.RenderPathLine(GridTile.CurrentlySelected.Coordinates);
        }
    }
    
    // Highlights all units according to their allegiance to the currently active unit
    private void SetUnitHighlight()
    {
        List<Unit> units = Grid.ActiveGrid.GetAllUnits();

        foreach (var unit in units)
        {
            if (unit == ActiveUnit && CurrentPhase == GamePhase.Combat) continue;
            Grid.ActiveGrid.HighlightTile(unit.GetCoordinates(), unit.enemy ? GridTile.TileHighlights.Foe : GridTile.TileHighlights.Friend);
        }

        if (CurrentPhase == GamePhase.Combat)
        {
            Grid.ActiveGrid.HighlightTile(ActiveUnit.GetCoordinates(), GridTile.TileHighlights.ActiveUnit);
        }
    }

    // Highlights the tiles the currently active unit can move to
    private void SetMovementHighlight()
    {
        Grid.ActiveGrid.CalculateMovement(ActiveUnitStartCoordinates, ActiveUnit.movementRange);
        Grid.ActiveGrid.HighlightTiles(Grid.ActiveGrid.GetReachableTiles(), GridTile.TileHighlights.Movement);
    }

    // Highlights the tiles the currently active unit can target (currently only works for the base attack)
    private void SetTargetHighlight()
    {
        Grid.ActiveGrid.HighlightTiles(ActiveUnit.baseAttack.GetValidTargets(ActiveUnit), GridTile.TileHighlights.AoE);
    }

    // *** INPUT MODES ***

    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
        UpdateHighlight();
    }

    private void HandleMovementInput()
    {
        GridTile selection = GridTile.CurrentlySelected;
        if (lastSelected != selection)
        {
            // Update pathline
            if (!IsMoving())
            {
                UpdatePathLine();
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
                selection.GetHighlight() == GridTile.TileHighlights.ActiveUnit)
            {
                // Check if mid-animation
                if (!ActiveUnit.UnitEntity.IsMoving)
                {
                    ActiveUnit.UnitEntity.StartMoveAnimation(selection.Coordinates);
                }
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
    
    // *** UTILITY FUNCTIONS ***
    
    public void ResetLastSelected()
    {
        lastSelected = null;
    }

    public bool IsMoving()
    {
        return ActiveUnit.UnitEntity.IsMoving;
    }
    
    // *** MONOBEHAVIOUR FUNCTIONS ***

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
        UnitsPlaced = 0;
    }

    void Start()
    {
        UpdateHighlight();
    }

    void Update()
    {
        // DEBUG PURPOSES
        if (CurrentPhase == GamePhase.Setup)
            NextPhase();

        // If the active unit is an AI unit, disable all direct input
        if (ActiveUnit != null && ActiveUnit.HasAI)
        {
            // If the AI is no longer busy, it means it is done and the next turn can start
            if (!ActiveUnit.AttachedAI.IsBusy)
                NextTurn();

            return;
        }

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