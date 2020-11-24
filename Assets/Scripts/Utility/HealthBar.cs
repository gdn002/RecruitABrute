using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image filling;

    public void SetMaximumHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        filling.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        filling.color = gradient.Evaluate(slider.normalizedValue);

    }


void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}