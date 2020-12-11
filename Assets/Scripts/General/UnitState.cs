using UnityEngine;

public class UnitState : ScriptableObject
{
    public Unit unitPrefab;

    public int health;
    public int maxHealth;
    public int movementRange;
    public int initiative;
    public string unitName;
    public bool enemy;
    
    
    public void Set(Unit unit)
    {
        health = unit.health;
        maxHealth = unit.maxHealth;
        movementRange = unit.movementRange;
        initiative = unit.initiative;
        unitName = unit.unitName;
        enemy = unit.enemy;
    }
}