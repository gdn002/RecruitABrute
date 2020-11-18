﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : MonoBehaviour
{
    // Global access to the currently active tracker; only one TurnTracker should be active at once.
    public static TurnTracker ActiveTracker { get; private set; }

    // Keep track of turns and rounds
    public int TurnCounter { get; private set; }
    public int RoundCounter { get; private set; }

    // Keep track of initiative
    public List<Unit> InitiativeOrder { get; private set; }
    public Unit ActiveUnit { get { return InitiativeOrder[TurnCounter]; } }


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

    public void NextTurn()
    {
        // TODO: "deactivate" the previous unit

        TurnCounter++;
        if (TurnCounter == InitiativeOrder.Count)
        {
            TurnCounter = 0;
            RoundCounter++;
        }

        // TODO: "activate" the next unit
        // TODO: turn change callbacks (if needed)
        Debug.Log("Turn " + TurnCounter + ", Round " + RoundCounter + ", Active Unit: " + ActiveUnit.gameObject.name);
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
            Debug.LogError("A TurnTracker component was initialized while another one is already running: " + gameObject.name);
        }
    }

    void Start()
    {
        TurnCounter = 0;
        RoundCounter = 0;
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