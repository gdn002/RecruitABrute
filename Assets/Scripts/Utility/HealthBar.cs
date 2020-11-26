using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public SpriteRenderer fill;
    public Gradient gradient;
    public Unit attachedUnit;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float iLerp = Mathf.InverseLerp(0, attachedUnit.maxHealth, attachedUnit.currentHealth);
        fill.transform.localScale = new Vector3(iLerp, 1, 1);
        fill.color = gradient.Evaluate(iLerp);

        transform.forward = Camera.main.transform.forward;
    }

}