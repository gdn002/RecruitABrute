using UnityEngine;
using System.Collections;

public class DEBUG_AITester : MonoBehaviour
{
    public AI attacker;

    private bool testRun = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!testRun)
        {
            attacker.FindTarget();
            testRun = true;
        }
    }
}
