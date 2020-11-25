using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 4;
    public Vector3 target;

    public void SetTarget(Vector2Int t){ //SET TARGET HERE
        target = Grid.ActiveGrid.GridToWorld(t);
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, step);

        if(gameObject.transform.position == target){
            Destroy(gameObject);
            }
    
    }
}
