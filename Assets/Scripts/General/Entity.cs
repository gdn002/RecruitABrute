using UnityEngine;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    public const int MOVE_ANIMATION_FRAMES = 60;
    
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    public bool IsMoving { get; private set; }

    private Vector2Int newCoordinates;
    
    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        Grid.ActiveGrid.SetCollision(coordinates, false);
        Grid.ActiveGrid.SetCollision(newCoordinates, hasCollision);
        coordinates = newCoordinates;
        UpdatePosition();
    }
    
    public void StartMoveAnimation(Vector2Int newCoordinates)
    {
        this.newCoordinates = newCoordinates;
        StartCoroutine(nameof(AnimateMove));
    }

    IEnumerator AnimateMove()
    {
        IsMoving = true;
        for (int moveAnimationFrame = MOVE_ANIMATION_FRAMES; moveAnimationFrame > 0; moveAnimationFrame--)
        {
            float interpolationRatio = (float) moveAnimationFrame / MOVE_ANIMATION_FRAMES;
            Vector2 interpolatedCoordinates = Vector2.Lerp(newCoordinates, coordinates, interpolationRatio);
            UpdatePosition(interpolatedCoordinates);
            yield return null;
        }
        Move(newCoordinates);
        IsMoving = false;
    }

    // Updates the entity's local position to match its current grid coordinates.
    public virtual void UpdatePosition(Vector2 coords)
    {
        transform.localPosition = Grid.GridToLocal(coords);
    }
    
    public virtual void UpdatePosition()
    {
        UpdatePosition(coordinates);
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
