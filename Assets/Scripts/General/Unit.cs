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
    public Entity UnitEntity;

    public UnitStatsText unitStatsText;
    //Add way to handle spells

    private List<Renderer> localRenderers;

    // *** UTILITY FUNCTIONS ***
    public Vector2Int GetCoordinates()
    {
        return UnitEntity.coordinates;
    }


    // *** UNIT STATE ***

    public void Activate()
    {
        UnitEntity.SetCollision(false);
    }

    public void Deactivate()
    {
        UnitEntity.SetCollision(true);
    }

    private void Highlight(bool highlight)
    {
        Color color = highlight ? Color.cyan : Color.white;
        foreach (var renderer in localRenderers)
        {
            renderer.material.color = color;
        }
    }


    // *** MONOBEHAVIOUR FUNCTIONS ***

    private void Awake()
    {
        UnitEntity = gameObject.GetComponent<Entity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        localRenderers = new List<Renderer>();
        localRenderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // *** MOUSE EVENTS ***
    private void OnMouseOver()
    {
        UnitStatsText.ActiveUnitStatsText.UpdateText(this);
        Highlight(true);
    }

    private void OnMouseExit()
    {
        UnitStatsText.ActiveUnitStatsText.ClearText();
        Highlight(false);
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