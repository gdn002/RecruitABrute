using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckHandler : MonoBehaviour
{
    public static DeckHandler MainDeckHandler { get; private set; }

    public Unit unit;
    public int maxdecksize;
    public List<Unit> PlayerDeck = new List<Unit>();
    public GameObject unitbutton;
    public RectTransform placementpanel;
    public Unit selectedUnit = null;
    
    void Awake(){

        if (MainDeckHandler == null)
        {
            MainDeckHandler = this;
        }
    }



    public void PrintDeck(){
        Debug.Log(PlayerDeck.Count);
        foreach(Unit u in PlayerDeck){
            Debug.Log(u.unitName);
        }
    }

    public void AddCard(Unit u)
    {
        if (PlayerDeck.Count < maxdecksize)
        {
            u.enemy = false;
            GameObject unit = Instantiate(u.gameObject);
            Debug.Log(u.unitName + " added");
            PlayerDeck.Add(unit.GetComponent<Unit>());
            unit.SetActive(false);
        }
        else
        {
            Debug.Log("Deck is full :(");
        }

    }

    public void DrawDeckToUI(){
        int num = 0;

        foreach(Unit u in PlayerDeck){
            GameObject btn = (GameObject)Instantiate(unitbutton);
            btn.transform.SetParent(placementpanel, false);
            btn.transform.localScale = new Vector3(1, 1, 1);
 
            btn.GetComponentInChildren<Text>().text = u.unitName;
 
            Button tempButton = btn.GetComponent<Button>();
            int tempInt = num;
 
            tempButton.onClick.AddListener(() => PlacementButtonOnClick(u, tempButton));
            num++;
        }
    }

    public void PlacementButtonOnClick(Unit u, Button b){
        selectedUnit = u;
        Debug.Log("Selected unit: " + selectedUnit);
        Destroy(b.gameObject);
    }

    void Update(){
    }

}
