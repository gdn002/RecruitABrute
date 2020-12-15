using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanelScript : MonoBehaviour
{
    public GameObject AddPanel;
    public GameObject UpgradePanel;
    private Object[] brutes;
    public GameObject unitbutton;
    public GameObject UpgradeUnitPanel;
    public static DeckHandler MainDeckHandler { get; private set; } //needed?
    
    public void DisplayRewardsOnAddPanel(){
        Debug.Log("DISPLAY REWARTDS ON ADD PANEL");
        brutes = Resources.LoadAll("Prefabs/Brutes", typeof(GameObject));
        brutes = brutes.OrderBy(brute => Random.value).ToArray();
        
        for(int i = 0;i<3;i++){
            var b = brutes[i];
            GameObject btn = Instantiate(unitbutton);
            btn.transform.SetParent(AddPanel.transform, false);
            //btn.transform.localScale = new Vector3(1, 1, 1);
            btn.transform.localPosition = new Vector3(-100+i*100,0,0);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(100,200);
            btn.GetComponentInChildren<Text>().text = b.name;
            Button tempButton = btn.GetComponent<Button>();

            tempButton.onClick.AddListener(() => AddButtonOnclick(b));
        }  
    }

    private void AddButtonOnclick(Object b){


        Debug.Log("This GameObject was selected");
        Debug.Log(b);
        GameObject newUnit = (GameObject) b;

        DeckHandler.MainDeckHandler.AddCard(newUnit.GetComponent<Unit>());//CAST INTO GAME OBJECT
        //ADD b TO DECK HERE
        ResetUI();

        
        
    }

    private void ResetUI(){
        GameObject.Find("Map").GetComponent<Canvas>().enabled = true;//Instead of loading scene 1

        DestroyButtons(UpgradePanel); //not sure if these are needed
        DestroyButtons(AddPanel); //not sure if these are needed


        UpgradeUnitPanel.SetActive(false);
        UpgradePanel.SetActive(false);
        AddPanel.SetActive(false);
        gameObject.SetActive(false); //these 3+2+2 lines can prob be moved outside if later

    }

    private void UpgradeButtonOnClick(UnitState u){
        Debug.Log(u.unitName);
        UpgradePanel.SetActive(false);
        UpgradeUnitPanel.SetActive(true);
        UpgradeUnitPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = u.unitName;
        
        Button healButton = UpgradeUnitPanel.transform.GetChild(1).GetComponent<Button>();
        UpgradeUnitPanel.transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text = "Heal fully";
        healButton.onClick.AddListener(() =>
        {
            healMethod(u);
            healButton.onClick.RemoveAllListeners();
        });
        //change onclick here
        Button maxHealthButton = UpgradeUnitPanel.transform.GetChild(2).GetComponent<Button>();
        UpgradeUnitPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>().text = "+10 Max HP";
        maxHealthButton.onClick.AddListener(() =>
        {
            maxHealthMethod(u);
            maxHealthButton.onClick.RemoveAllListeners();

        });
        //change onclick here
        
        // Skill button
        Object[] skills = Resources.LoadAll("Prefabs/Skills/Rewards", typeof(ScriptableObject));
        Skill skill = (Skill) skills[Random.Range(0, skills.Length)];
        
        Button addSkillButton = UpgradeUnitPanel.transform.GetChild(3).GetComponent<Button>();
        UpgradeUnitPanel.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>().text = "Add skill: " + skill.skillName;
        addSkillButton.onClick.AddListener(() =>
        {
            addSkillMethod(u, skill);
            addSkillButton.onClick.RemoveAllListeners();
        });
    }
    
    private void addSkillMethod(UnitState u, Skill skill){
        u.abilities = u.abilities.Append(skill).ToArray();
        ResetUI();
    }
    private void healMethod(UnitState u)
    {
        u.health = u.maxHealth;
        ResetUI();
    }
    
    private void maxHealthMethod(UnitState u){
        u.maxHealth+=10;
        u.health+=10;
        ResetUI();
    }

    private void DestroyButtons(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /*private void CreateButton(GameObject panelToDrawOn, Object valueToSendToButton){
            GameObject btn = Instantiate(unitbutton);
            btn.transform.SetParent(panelToDrawOn.transform, false);
            //btn.transform.localScale = new Vector3(1, 1, 1);
            btn.transform.localPosition = new Vector3(-100+spacenum*100,0,0);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(100,200);
            btn.GetComponentInChildren<Text>().text = valueToSendToButton.name;
            Button tempButton = btn.GetComponent<Button>();

            tempButton.onClick.AddListener(() => AddButtonOnclick(valueToSendToButton));
    }*/


    public void DisplayDeckForUpgrade(){
        int x = 0;
        foreach (UnitState u in DeckHandler.MainDeckHandler.Units){
            GameObject btn = Instantiate(unitbutton);
            btn.transform.SetParent(UpgradePanel.transform, false);
            //btn.transform.localScale = new Vector3(1, 1, 1);
            btn.transform.localPosition = new Vector3(-100+x*100,0,0);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(100,200);
            btn.GetComponentInChildren<Text>().text = u.unitName;
            Button tempButton = btn.GetComponent<Button>();

            tempButton.onClick.AddListener(() => UpgradeButtonOnClick(u));
            x++;
        }
    }


}
