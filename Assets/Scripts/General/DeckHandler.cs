using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Unit StartingUnitPrefab;
    
    void Awake()
    {        
        if (MainDeckHandler == null)
        {
            MainDeckHandler = this;
            DontDestroyOnLoad(this);
            
            //Temporary to test adding card
            UnitState initUnit = ScriptableObject.CreateInstance<UnitState>();
            initUnit.Set(StartingUnitPrefab);
            initUnit.unitPrefab = StartingUnitPrefab;
            AddCard(initUnit);
        }
        else
        {
            Destroy(gameObject);
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

        if (placementpanel == null)
        {
            placementpanel = GameObject.FindGameObjectsWithTag("PlacementPanel")[0].GetComponent<RectTransform>();
        }

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
        GameObject unitGameObject = Instantiate(u.unitPrefab.gameObject);
        Unit unit = unitGameObject.GetComponent<Unit>();
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