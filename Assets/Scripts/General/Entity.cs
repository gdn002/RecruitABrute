using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A GameObject whose position is regulated by a Grid
public class Entity : MonoBehaviour
{
    public const double BASE_MOVE_ANIMATION_SECONDS = 0.3f;
    
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    public bool IsMoving { get; private set; }

    private Vector2Int newCoordinates;
    private List<Vector2Int> movementPath;
    
    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        Grid.ActiveGrid.SetCollision(coordinates, false);
        Grid.ActiveGrid.SetCollision(newCoordinates, hasCollision);
        coordinates = newCoordinates;
        UpdatePosition();
        Grid.ActiveGrid.UpdateHighlighting();
    }
    
    public void StartMoveAnimation(Vector2Int newCoordinates)
    {
        this.newCoordinates = newCoordinates;
        movementPath = Grid.ActiveGrid.GetPath(coordinates, newCoordinates);
        StartCoroutine(nameof(AnimateMove));
    }

    IEnumerator AnimateMove()
    {
        IsMoving = true;
        double moveAnimationSeconds = BASE_MOVE_ANIMATION_SECONDS * Math.Log(movementPath.Count);
        double elapsedTime = 0.0f;
        while (elapsedTime < moveAnimationSeconds)
        {
            double interpolationRatio = elapsedTime / moveAnimationSeconds;
            int interpolationIndex = (int) (interpolationRatio * movementPath.Count);
            if (interpolationIndex >= movementPath.Count - 1)
            {
                interpolationIndex = movementPath.Count - 2;
            }
            double interpolationSubRatio = (interpolationRatio - ((double) interpolationIndex / movementPath.Count)) * movementPath.Count;
            Vector2 interpolatedCoordinates = Vector2.Lerp(movementPath[interpolationIndex], movementPath[interpolationIndex + 1], (float) interpolationSubRatio);
            UpdatePosition(interpolatedCoordinates);
            yield return null;
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
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
