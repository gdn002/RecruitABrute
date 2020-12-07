using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckHandler : MonoBehaviour
{
    public static DeckHandler MainDeckHandler { get; private set; }

    public int maxdecksize;
    public GameObject unitbutton;
    public RectTransform placementpanel;
    public Unit selectedUnit = null;
    public List<UnitState> Units { get; private set; } = new List<UnitState>();

    public Unit BaseUnitPrefab;
    
    void Awake()
    {
        BaseUnitPrefab.enemy = false;
        
        if (MainDeckHandler == null)
        {
            MainDeckHandler = this;
            DontDestroyOnLoad(transform.root.gameObject);
            UnitState initUnit = ScriptableObject.CreateInstance<UnitState>();
            initUnit.health = 100;
            initUnit.maxHealth = 100;
            initUnit.movementRange = 3;
            initUnit.initiative = 3;
            initUnit.unitName = "Test Brute";
            initUnit.enemy = false;
            AddCard(initUnit);
            AddCard(initUnit);
            AddCard(initUnit);
        }
    }


    public void PrintDeck()
    {
        Debug.Log(Units.Count);
        foreach (UnitState u in Units)
        {
            Debug.Log(u.unitName);
        }
    }

    public void AddCard(UnitState u)
    {
        if (Units.Count < maxdecksize)
        {
            Units.Add(u);
        }
    }

    public void DrawDeckToUI()
    {
        int num = 0;

        foreach (UnitState u in Units)
        {
            GameObject btn = Instantiate(unitbutton);
            btn.transform.SetParent(placementpanel, false);
            btn.transform.localScale = new Vector3(1, 1, 1);
            btn.GetComponentInChildren<Text>().text = u.unitName;
            Button tempButton = btn.GetComponent<Button>();

            tempButton.onClick.AddListener(() => PlacementButtonOnClick(u, tempButton));
            num++;
        }
    }

    public void PlacementButtonOnClick(UnitState u, Button b)
    {
        GameObject unitGameObject = Instantiate(BaseUnitPrefab.gameObject);
        Unit unit = unitGameObject.AddComponent<Unit>();
        unit.Init(u);
        unitGameObject.SetActive(false);
        selectedUnit = unit;
        Grid.ActiveGrid.AddEntity(selectedUnit.UnitEntity);
        Destroy(b.gameObject);
    }

    void Update()
    {
    }
}