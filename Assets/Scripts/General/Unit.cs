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
    private UnitState unitState;

    public UnitState UnitState
    {
        get
        {
            unitState.Set(this);
            return unitState;
        }
    }

    public void Init(UnitState initUnitState)
    {
        unitState = initUnitState;
        health = initUnitState.health;
        maxHealth = initUnitState.maxHealth;
        movementRange = initUnitState.movementRange;
        initiative = initUnitState.initiative;
        unitName = initUnitState.unitName;
        enemy = initUnitState.enemy;
    }

    // These stats are derived from the unit's base attack Skill
    public int AttackRange { get { return baseAttack.range; } }
    public int AttackDamage { get { return baseAttack.power; } }

    public UnitStatsText unitStatsText;

    // Basic attack
    public Skill baseAttack;

    // Special abilities
    public Skill[] abilities;

    public AI AttachedAI { get; private set; }
    public bool HasAI { get { return AttachedAI != null; } }

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
        else
        {
            unitState.Set(this);
        }
    }

    public void Remove()
    {
        TurnTracker.ActiveTracker.RemoveFromInitiative(this);
        Grid.ActiveGrid.RemoveEntity(UnitEntity);
        UnitEntity.SetCollision(false);
        Grid.ActiveGrid.RemoveEntity(UnitEntity);
        DeckHandler.MainDeckHandler.Units.Remove(unitState);
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
        unitState = ScriptableObject.CreateInstance<UnitState>();

        if (enemy)
        {
            gameObject.AddComponent<AI>();
        }
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

        AttachedAI = GetComponent<AI>();
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
    // *** CAMERA EVENTS ***
    public void RotateForCamera(float degrees)
    {
        gameObject.transform.Find("Mesh").transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), degrees);
    }
    public void AngleForCamera(float degrees)    
    {
        gameObject.transform.Find("Mesh").transform.Find("Cube").transform.localEulerAngles = new Vector3(degrees, 0.0f, 0.0f);       
    }
}