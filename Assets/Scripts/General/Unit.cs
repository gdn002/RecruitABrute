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
    public bool enemy;


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
        Grid.ActiveGrid.RemoveEntity(UnitEntity);
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
            Debug.LogWarning("Unit " + unitName + " has no set base attack skill. Loading default skill...");
            baseAttack = ScriptableObject.CreateInstance<Skill>();
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
        Grid.ActiveGrid.MouseOverGridTile(GetCoordinates());
    }

    private void OnMouseExit()
    {
        UnitStatsText.ActiveUnitStatsText.ClearText();
        Highlight(false);
        Grid.ActiveGrid.MouseExitGridTile(GetCoordinates());
    }

    private void OnMouseDown()
    {
        Grid.ActiveGrid.MouseDownGridTile(GetCoordinates());
    }
}