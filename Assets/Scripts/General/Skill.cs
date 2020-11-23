using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Custom/Skill")]
public class Skill : ScriptableObject
{
    public enum TargetType
    {
        Self = 0,
        Unit,
        Tile,
    }

    public enum ZoneType
    {
        Point = 0,
        Square,
        Row,
        Column,
        Shape,
    }

    [Flags]
    public enum AffectedUnitType
    {
        Self        = 0b0001,
        Friendly    = 0b0010,
        Enemy       = 0b0100,
        Prop        = 0b1000,
        All         = 0b1111,
    }

    public enum EffectType
    {
        Damage = 0,
        Heal,
    }

    public string skillName = "Default Skill";

    public TargetType targetType            = TargetType.Unit;
    public ZoneType effectZone              = ZoneType.Point;
    public AffectedUnitType affectedUnits   = AffectedUnitType.Enemy;
    public EffectType effectOnTargets       = EffectType.Damage;

    public int power = 10;
    public int range = 1;
    public int radius = 1;


    // Returns all tiles that can be targeted by this skill while the casting unit is located at the given coordinates
    public List<Vector2Int> GetValidTargets(Unit caster)
    {
        Vector2Int coordinates = caster.GetCoordinates();

        List<Vector2Int> validTargets = new List<Vector2Int>();
        switch (targetType)
        {
            case TargetType.Self: // Only the self can be targeted
                validTargets.Add(coordinates);
                break;

            case TargetType.Unit: // All valid units in range can be targeted
                foreach (var unit in GetAllUnitsInRange(coordinates))
                {
                    if (IsAffected(caster, unit))
                        validTargets.Add(unit.GetCoordinates());
                }
                break;

            case TargetType.Tile: // All tiles in range can be targeted
                validTargets = GetAllTilesInRange(coordinates);
                break;
        }

        return validTargets;
    }

    // Returns all tiles that will be affected by this skill when targeting the given coordinates
    public List<Vector2Int> GetAffectedTiles(Vector2Int target)
    {
        List<Vector2Int> affectedTiles = new List<Vector2Int>();
        switch (effectZone)
        {
            case ZoneType.Point: // Only the target tile will be affected
                affectedTiles.Add(target);
                break;

            case ZoneType.Square: // All tiles in a square centered around the target point will be affected
                affectedTiles = GetAllTilesInRadius(target, radius, radius);
                break;

            case ZoneType.Row: // All tiles in radius in the same row as the target point will be affected
                affectedTiles = GetAllTilesInRadius(target, radius, 0);
                break;

            case ZoneType.Column: // All tiles in radius in the same column as the target point will be affected
                affectedTiles = GetAllTilesInRadius(target, 0, radius);
                break;

            case ZoneType.Shape:
                // TODO: Handle special shapes
                break;
        }

        return affectedTiles;
    }

    // Returns all units that will be affected by this skill when targeting the given coordinates
    public List<Unit> GetAffectedUnits(Unit caster, Vector2Int target)
    {
        List<Vector2Int> affectedTiles = GetAffectedTiles(target);
        List<Unit> affectedUnits = new List<Unit>();

        foreach (var tile in affectedTiles)
        {
            Unit unit = Grid.ActiveGrid.GetUnit(tile);
            if (IsAffected(caster, unit))
                affectedUnits.Add(unit);
        }

        return affectedUnits;
    }

    // Executes the effects of this skill, targeting the given coordinates
    public void ActivateSkill(Unit caster, Vector2Int target)
    {
        List<Unit> affectedUnits = GetAffectedUnits(caster, target);
        foreach (var unit in affectedUnits)
        {
            EffectOnUnit(unit);
        }
    }

    // Check if this type of unit is affected by this skill
    public bool IsAffected(Unit caster, Unit unit)
    {
        if (unit == null)
            return false;

        if (caster == unit)
            return IsAffected(AffectedUnitType.Self);

        if (caster.enemy == unit.enemy)
            return IsAffected(AffectedUnitType.Friendly);

        return IsAffected(AffectedUnitType.Enemy);

        // TODO: Check for targetable props in the future
    }

    // Commits the effects of this skill on an unit. (Does not verify if the Unit is a valid target, use GetAffectedUnits for that)
    public void EffectOnUnit(Unit unit)
    {
        switch (effectOnTargets)
        {
            case EffectType.Damage:
                unit.ModifyHealth(-power);
                break;

            case EffectType.Heal:
                unit.ModifyHealth(power);
                break;
        }
    }


    private List<Unit> GetAllUnitsInRange(Vector2Int coordinates)
    {
        List<Unit> allUnits = Grid.ActiveGrid.GetAllUnits();
        List<Unit> unitsInRange = new List<Unit>();

        foreach (var unit in allUnits)
        {
            if (Vector2Int.Distance(coordinates, unit.GetCoordinates()) <= (float)range)
            {
                unitsInRange.Add(unit);
            }
        }

        return unitsInRange;
    }

    private List<Vector2Int> GetAllTilesInRange(Vector2Int coordinates)
    {
        List<Vector2Int> tilesInRange = new List<Vector2Int>();
        Vector2Int gridSize = Grid.ActiveGrid.gridSize;

        int minX = coordinates.x - range; if (minX < 0) minX = 0;
        int minY = coordinates.y - range; if (minY < 0) minY = 0;
        int maxX = coordinates.x + range + 1; if (maxX > gridSize.x) maxX = gridSize.x;
        int maxY = coordinates.y + range + 1; if (maxY > gridSize.y) maxY = gridSize.y;

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                if (Vector2Int.Distance(coordinates, tile) <= (float)range)
                    tilesInRange.Add(tile);
            }
        }

        return tilesInRange;
    }

    private List<Vector2Int> GetAllTilesInRadius(Vector2Int target, int radX, int radY)
    {
        List<Vector2Int> tilesInRadius = new List<Vector2Int>();
        Vector2Int gridSize = Grid.ActiveGrid.gridSize;

        int minX = target.x - radX; if (minX < 0) minX = 0;
        int minY = target.y - radY; if (minY < 0) minY = 0;
        int maxX = target.x + radX + 1; if (maxX > gridSize.x) maxX = gridSize.x;
        int maxY = target.y + radY + 1; if (maxY > gridSize.y) maxY = gridSize.y;

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                tilesInRadius.Add(new Vector2Int(x, y));
            }
        }

        return tilesInRadius;
    }

    private bool IsAffected(AffectedUnitType type)
    {
        return (affectedUnits & type) != 0;
    }
}