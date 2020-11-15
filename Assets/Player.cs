using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int maxHealth = 100;
    public int health;


    public HealthBar healthBar;
    private void Start()
    {
        health = maxHealth;
        healthBar.SetMaximumHealth(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamageAmount(20);
        }
    }

    private void DamageAmount(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);

    }
}
