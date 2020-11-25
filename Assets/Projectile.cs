using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 4;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        SetTarget(target); 
    }

    public void SetTarget(Transform t){ //SET TARGET HERE
        target = t;
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.position, step);

        if(gameObject.transform.position == target.position){
            Destroy(gameObject);
            }
    
    }
}
