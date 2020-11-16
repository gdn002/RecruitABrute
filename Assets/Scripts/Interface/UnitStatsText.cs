using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsText : MonoBehaviour
{
    public UnitStats unitStats;
    public Text unitStatsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unitStats != null) {
            unitStatsText.text = unitStats.Name + "\n" +
        "Movement range: " + unitStats.MovementRange + "\n" +
        "Health: " + unitStats.Health + " / " + unitStats.MaxHealth + "\n" +
        "Damage: " + unitStats.Damage + "\n" +
        "Initiative: " + unitStats.Initiative + "\n";
        } else {
            unitStatsText.text = "";
        }
    }
}
