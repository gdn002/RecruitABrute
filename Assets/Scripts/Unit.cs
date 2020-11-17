using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int movementRange;
    public int attackRange;
    public int damage;
    public int initiative;
    public string unitName;

    public UnitStatsText unitStatsText;
//Add way to handle spells

    void Awake()
    {
        Debug.Log("awake");
        unitStatsText = GameObject.FindWithTag("UnitStatsText").GetComponent<UnitStatsText>();
    }

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
        unitStatsText.unit = this;
    }

    void OnMouseExit()
    {
        unitStatsText.unit = null;
    }
}