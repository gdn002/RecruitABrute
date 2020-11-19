using UnityEngine;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    
    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        Grid.ActiveGrid.SetCollision(coordinates, false);
        Grid.ActiveGrid.SetCollision(newCoordinates, hasCollision);
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

    public void SetCollision(bool collision)
    {
        hasCollision = collision;
        Grid.ActiveGrid.SetCollision(coordinates, hasCollision);
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
    void OnMouseOver()
    {
    }

    void OnMouseExit()
    {
    }
}
