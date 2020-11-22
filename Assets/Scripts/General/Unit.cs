using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int movementRange;
    public int initiative;
    public string unitName;
    public Entity UnitEntity;


    // These stats are derived from the unit's base attack Skill
    public int AttackRange { get { return baseAttack.range; } }
    public int AttackDamage { get { return baseAttack.power; } }

    public UnitStatsText unitStatsText;

    // Basic attack
    public Skill baseAttack;

    // Special abilities
    public Skill[] abilities;

    private bool isActiveUnit = false;
    private List<Renderer> localRenderers;

    // *** UTILITY FUNCTIONS ***
    public Vector2Int GetCoordinates()
    {
        return UnitEntity.coordinates;
    }


    public void ModifyHealth(int delta)
    {
        health += delta;
        if (health <= 0)
        {
            Remove();
        }
    }

    public void Remove()
    {
        TurnTracker.ActiveTracker.RemoveFromInitiative(this);
        Grid.ActiveGrid.RemoveEntity(UnitEntity);
        UnitEntity.SetCollision(false);
        Destroy(gameObject);
    }

    // *** UNIT STATE ***

    public void Activate()
    {
        UnitEntity.SetCollision(false);
        isActiveUnit = true;
    }

    public void Deactivate()
    {
        UnitEntity.SetCollision(true);
        isActiveUnit = false;
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

        if (baseAttack == null)
        {
            baseAttack = Skill.DEFAULT_SKILL;
            Debug.LogWarning("Unit " + unitName + " has no set base attack skill. Loading default skill...");
        }
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

    }
}