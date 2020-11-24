using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int movementRange;
    public int initiative;
    public string unitName;
    public Entity UnitEntity;
    public bool enemy;

    public HealthBar healthBar;

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


    public void ReceiveDamage(int damage)
    {
        currentHealth += damage;
        
        if (currentHealth <= 0)
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
        currentHealth = maxHealth;

        if (baseAttack == null)
        {
            Debug.LogWarning("Unit " + unitName + " has no set base attack skill. Loading default skill...");
            baseAttack = ScriptableObject.CreateInstance<Skill>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Just for test, for now.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReceiveDamage(20);
        }
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