using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 4;
    public Vector3 target;
    private Func<int> callback;

    public void SetTarget(Vector2Int t, Func<int> callback){ //SET TARGET HERE
        this.callback = callback;
        target = Grid.ActiveGrid.GridToWorld(t);
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, step);

        if (gameObject.transform.position == target)
        {
            callback();
            Destroy(gameObject);
        }
    }
}
