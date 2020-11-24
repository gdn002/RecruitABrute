using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsText : MonoBehaviour
{
    public static UnitStatsText ActiveUnitStatsText { get; private set; }
    public Text unitText;

    public void UpdateText(Unit unit)
    {
        unitText.text = unit.unitName + "\n" +
                        "Movement range: " + unit.movementRange + "\n" +
                        "Attack range: " + unit.AttackRange + "\n" +
                        "Health: " + unit.currentHealth + " / " + unit.maxHealth + "\n" +
                        "Damage: " + unit.AttackDamage + "\n" +
                        "Initiative: " + unit.initiative + "\n";
    }

    public void ClearText()
    {
        unitText.text = "";
    }

    private void Awake()
    {
        if (ActiveUnitStatsText == null)
        {
            ActiveUnitStatsText = this;
        }
        else
        {
            Debug.LogError("A UnitStatsText component was initialized while another one is already running: " +
                           gameObject.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}