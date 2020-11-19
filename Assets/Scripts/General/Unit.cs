using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int movementRange;
    public int attackRange;
    public int damage;
    public int initiative;
    public string unitName;

    public UnitStatsText unitStatsText;
    //Add way to handle spells

    // *** UTILITY FUNCTIONS ***
    public Entity GetEntity()
    {
        return gameObject.GetComponent<Entity>();
    }

    public Vector2Int GetCoordinates()
    {
        return GetEntity().coordinates;
    }

    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // *** MOUSE EVENTS ***
    private void OnMouseOver()
    {
        UnitStatsText.ActiveUnitStatsText.UpdateText(this);
    }

    private void OnMouseExit()
    {
        UnitStatsText.ActiveUnitStatsText.ClearText();
    }

    private void OnMouseDown()
    {
        Unit attacker = TurnTracker.ActiveTracker.ActiveUnit;
        if (Grid.ActiveGrid.AttackDistance(attacker, this) <= attacker.attackRange && attacker != this)
        {
            health -= attacker.damage;
            TurnTracker.ActiveTracker.NextTurn();
        }
    }
}