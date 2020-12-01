using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    private struct AITarget
    {
        public Vector2Int coordinates;
        public int value;
        public float distance;
    }

    public Unit AttachedUnit { get; private set; }
    public Skill AttachedSkill { get { return AttachedUnit.baseAttack; } }


    private MovementCalculator movementCalculator;


    // Parses all possible targets and tracks the most optimal target
    public void GetDefiniteTarget()
    {
        movementCalculator.CalculateMovement(AttachedUnit.GetCoordinates(), AttachedUnit.movementRange);
        List<AITarget> targets = GetPossibleTargets();

        foreach (var target in targets)
        {
            Vector2Int? movePos = null;
            if (IsTargetReachable(target, ref movePos))
            {
                string msg = "DIRECTIVE FOUND \r\n";
                if (movePos != null)
                    msg += "Move to position: " + movePos + "\r\n";
                msg += "Target position: " + target.coordinates +"\r\n";
                msg += "Target value: " + target.value + "\r\n";
                Debug.Log(msg);
                return;
            }
        }
    }

    // Returns true if this target is within attack range
    // If this target requires movement before it can be in range, movePos will assume the destination coordinates.
    // If no movement is required, movePos will not be altered.
    private bool IsTargetReachable(AITarget target, ref Vector2Int? movePos)
    {
        float distToTarget = Vector2Int.Distance(target.coordinates, AttachedUnit.GetCoordinates());

        // Check if target is in range without moving
        if (distToTarget <= AttachedUnit.AttackRange)
            return true;

        // Check if target can be in range after moving
        if (distToTarget > AttachedUnit.AttackRange + AttachedUnit.movementRange)
            return false;

        // Find tile within movement range that is also in attack range of the desired target
        // Prioritize the least amount of movement
        int distance = int.MaxValue;
        Vector2Int? coordinates = null;

        List<Vector2Int> reachableTiles = movementCalculator.GetReachableTiles();
        foreach (var tile in reachableTiles)
        {
            if (Vector2Int.Distance(target.coordinates, tile) <= AttachedUnit.AttackRange)
            {
                int thisDist = movementCalculator.GetDistance(tile);
                if (thisDist < distance)
                {
                    distance = thisDist;
                    coordinates = tile;
                }
            }
        }

        movePos = coordinates;
        return coordinates != null;
    }

    // Returns a list of possible target points for the Unit, sorted by value (descending order) and distance (ascending order)
    private List<AITarget> GetPossibleTargets()
    {
        List<AITarget> targets = new List<AITarget>();

        switch (AttachedSkill.targetType)
        {
            // Only units can be targeted, thus check the value of targeting all valid Units
            case Skill.TargetType.Unit: 
                {
                    List<Unit> allUnits = Grid.ActiveGrid.GetAllUnits();
                    foreach (var unit in allUnits)
                    {
                        // Filter out only valid targets
                        if (AttachedSkill.IsAffected(AttachedUnit, unit))
                        {
                            // Check all units that will be affected from targeting this unit
                            AITarget target;
                            target.coordinates = unit.GetCoordinates();
                            target.value = GetValueFor(AttachedSkill.GetAffectedUnits(AttachedUnit, target.coordinates));
                            target.distance = Vector2Int.Distance(AttachedUnit.GetCoordinates(), target.coordinates);

                            // If there is value to this target, add it to the list
                            if (target.value > 0) targets.Add(target);
                        }
                    }
                }
                break;

            // Any tile can be targeted, thus check the values of all tiles
            case Skill.TargetType.Tile:
                {
                    for (int x = 0; x < Grid.ActiveGrid.gridSize.x; x++)
                    {
                        for (int y = 0; y < Grid.ActiveGrid.gridSize.y; y++)
                        {
                            // Check all units that will be affected from targeting this tile
                            AITarget target;
                            target.coordinates = new Vector2Int(x, y);
                            target.value = GetValueFor(AttachedSkill.GetAffectedUnits(AttachedUnit, target.coordinates));
                            target.distance = Vector2Int.Distance(AttachedUnit.GetCoordinates(), target.coordinates);

                            // If there is value to this target, add it to the list
                            if (target.value > 0) targets.Add(target);
                        }
                    }
                }
                break;
        }

        targets.Sort((x, y) =>
        {
            int i = y.value.CompareTo(x.value);
            if (i == 0)
                return x.distance.CompareTo(y.distance);
            return i;
        });
        return targets;
    }

    // Get the total "value" sum for a list of affected units
    private int GetValueFor(List<Unit> affectedUnits)
    {
        int value = 0;
        foreach (var unit in affectedUnits)
        {
            value += Mathf.Abs(AttachedSkill.PreviewEffectOnUnit(unit));
        }

        return value;
    }


    // Use this for initialization
    void Start()
    {
        movementCalculator = new MovementCalculator();

        AttachedUnit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
