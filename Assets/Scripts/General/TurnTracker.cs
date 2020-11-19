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

    // Keep track of turns and rounds
    public int TurnCounter { get; private set; }
    public int RoundCounter { get; private set; }
    public GamePhase CurrentPhase {get; private set;}

    // Keep track of initiative
    public List<Unit> InitiativeOrder { get; private set; }

    public Unit ActiveUnit
    {
        get { return InitiativeOrder[TurnCounter]; }
    }

    public Vector2Int ActiveUnitStartCoordinates;

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
        if (TurnCounter == InitiativeOrder.Count)
        {
            TurnCounter = 0;
            RoundCounter++;
        }

        OnTurnStart();
    }

    private void OnTurnEnd()
    {
        ActiveUnit.Deactivate();

        Grid.ActiveGrid.ClearHighlight();
    }

    private void OnTurnStart()
    {
        ActiveUnit.Activate();
        ActiveUnitStartCoordinates = ActiveUnit.GetCoordinates();

        Grid.ActiveGrid.CalculateMovement(ActiveUnitStartCoordinates, ActiveUnit.movementRange);
        Grid.ActiveGrid.HighlightMovementTiles(ActiveUnit);
        Grid.ActiveGrid.HighlightTile(ActiveUnitStartCoordinates, GridTile.TileHighlights.Friend);

        Debug.Log("Turn " + TurnCounter + ", Round " + RoundCounter + ", Active Unit: " + ActiveUnit.gameObject.name);
    }

    private void OnEnterCombatPhase()
    {
        // First Turn

        OnTurnStart();
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
    }

    void OnDestroy()
    {
        if (ActiveTracker == this)
        {
            ActiveTracker = null;
        }
    }
}