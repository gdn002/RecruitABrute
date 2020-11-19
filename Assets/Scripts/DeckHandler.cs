using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckHandler : MonoBehaviour
{
    public Unit unit;
    public int maxdecksize;
    public List<Unit> PlayerDeck = new List<Unit>();
    public GameObject unitbutton;
    public RectTransform placementpanel;
    public Unit selectedUnit = null;
    public GameObject Grid;
    

    public void PrintDeck(){
        Debug.Log(PlayerDeck.Count);
        foreach(Unit u in PlayerDeck){
            Debug.Log(u.unitName);
        }
    }

    public void AddCard(Unit u){
        if (PlayerDeck.Count < maxdecksize)
        {
            Instantiate(u);
            Debug.Log(u.unitName + " added");
            PlayerDeck.Add(u);
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
        Debug.Log(selectedUnit);
        Destroy(b.gameObject);

    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("LMB pressed");
            if(selectedUnit != null){

                //selectedUnit.transform.SetParent(Grid); //WHAT DO HERE
                //selectedUnit.transform.position = GridTile.CurrentlySelected.coordinates;

                //Instantiate(selectedUnit, GridTile.CurrentlySelected.coordinates); //Using instantiate here will most likely create a new unit every time. We don't want this
                Debug.Log("Place " + selectedUnit.unitName);
                selectedUnit = null;
            }
        }

    }

}
