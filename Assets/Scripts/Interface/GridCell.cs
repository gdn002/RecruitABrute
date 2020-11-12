using UnityEngine;
using System.Collections;

public class GridCell : MonoBehaviour
{
    public float gapBetweenCells = 0.01f;
    public float height = 0.1f;


    public void Initialize(Transform parent, Vector2Int coordinates)
    {
        transform.SetParent(parent);

        Vector3 position = Grid.GridToLocal(coordinates);
        position.y = -(height / 2);
        transform.localPosition = position;
    }

    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(Grid.CELL_SIZE - gapBetweenCells, height, Grid.CELL_SIZE - gapBetweenCells);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
