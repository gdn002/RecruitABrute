using UnityEngine;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;

    public Grid ParentGrid { get; private set; }


    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        ParentGrid.SetCollision(coordinates, false);
        ParentGrid.SetCollision(newCoordinates, hasCollision);
        coordinates = newCoordinates;
        UpdatePosition();
    }

    // Updates the entity's local position to match its current grid coordinates.
    public virtual void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates);
    }

    // Moves the entity to the nearest grid coordinates based on its current local position.
    public virtual void MoveToNearest()
    {
        Move(Grid.LocalToGrid(transform.localPosition));
    }

    // Ensures the entity is correctly positioned and oriented within the grid.
    public virtual void UpdateEntity()
    {
        UpdatePosition();
    }

    // *** GENERAL FUNCTIONS ***

    // Initializes this Entity to a Grid. Only call once.
    public void Initialize(Grid parentGrid)
    {
        if (ParentGrid == null)
        {
            ParentGrid = parentGrid;
            transform.SetParent(ParentGrid.transform, true);
        }
        else
        {
            Debug.LogWarning("Entity.Initialize() was called more than once on Entity: " + name);
        }
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
}
