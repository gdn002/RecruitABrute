using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsText : MonoBehaviour
{
    public Unit unit;
    public Text unitText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (unit != null)
        {
            unitText.text = unit.unitName + "\n" +
                            "Movement range: " + unit.movementRange + "\n" +
                            "Health: " + unit.health + " / " + unit.maxHealth + "\n" +
                            "Damage: " + unit.damage + "\n" +
                            "Initiative: " + unit.initiative + "\n";
        }
        else
        {
            unitText.text = "";
        }
    }
}