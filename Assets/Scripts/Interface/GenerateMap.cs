using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenerateMap : MonoBehaviour
{
    public Canvas myCanvas;
    private GameObject map;
    public GameObject button;
    public GameObject title;
    public GameObject mapBackground;
    public GameObject panel;
    public GameObject startButton;
    public int sceneToLoad; //0 will be main menu, 1 will be map scene 2-x will be the levels
    public int yRandomness;
    public int spacing;
    public int amountOfButtons;

    private string[] easyLevels = {"2Rogues", "1Archer"};
    
    void Start()
    {
        CreateNewMap();
    }

    void AddPanel(){
        GameObject p = Instantiate(mapBackground);
        p.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width,Screen.height);
        p.transform.SetParent(map.transform);
        p.transform.position = new Vector2(Screen.width/2,Screen.height/2);

    }

    void AddTitle(){
        GameObject t = Instantiate(title);
        t.transform.SetParent(map.transform);
        t.transform.position = new Vector2(Screen.width/3,Screen.height-Screen.height/5);
    }

    void AddButtons(){
        spacing = Screen.width/(amountOfButtons)-50;
        GameObject sB = Instantiate(startButton); //Create the first button
        sB.transform.SetParent(map.transform);
        sB.transform.position = new Vector3(25*amountOfButtons+spacing/2,Screen.height/2,0);
        Button startBut = sB.GetComponent<Button>();
        startBut.onClick.AddListener(() => LoadLevel(0, startBut));

        GameObject lastButton = sB;

        int num = 1;
        for (int i = 1; i<amountOfButtons;i++){
            GameObject b = (GameObject)Instantiate(button); //Make button Gameobject
            b.transform.SetParent(map.transform, false); //Set parent to canvas
            Vector3 pos = lastButton.transform.position + new Vector3((spacing),Random.Range(-yRandomness,yRandomness),0); //Give position which is last position + another vector
            //b.GetComponent<RectTransform>().localPosition = pos; //Set position
            b.transform.position = pos;
            DrawLineBetweenTwo(lastButton, b); //draw line between old and new position
            
            lastButton = b; //update lastpost
            Button tempBut = b.GetComponent<Button>(); //
            int tempInt = num;
            tempBut.onClick.AddListener(() => LoadLevel(tempInt, tempBut));
            num++;
        }

    }

    void DrawLineBetweenTwo(GameObject a, GameObject b){
        GameObject line = Instantiate(panel);
        line.transform.SetParent(map.transform);
        float lineLength = Vector3.Distance(a.transform.position,b.transform.position);
        RectTransform rect = line.GetComponent<RectTransform>();

        Image img = line.GetComponent<Image>(); //set color of line
        img.color = Color.black;

        rect.sizeDelta = new Vector2(5, lineLength);
        rect.position = (a.transform.position+b.transform.position)/2;
        rect.transform.LookAt(b.transform, Vector3.down);
        rect.transform.rotation *= Quaternion.Euler(0,90,90);
        rect.transform.SetSiblingIndex(2);
    }

    void CreateCanvas(){
        map = new GameObject();
        map.name = "Map";
        map.AddComponent<Canvas>();

        myCanvas = map.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        map.AddComponent<CanvasScaler>();
        map.AddComponent<GraphicRaycaster>();
    }


    void CreateNewMap(){
        CreateCanvas();
        AddPanel();
        AddTitle();
        AddButtons();
        DontDestroyOnLoad(map);
        Destroy(gameObject);
    }

    public void LoadLevel(int levelIndex, Button b){

        ColorBlock c = b.colors;

        if (levelIndex+2 == sceneToLoad){
            b.onClick.RemoveAllListeners();
            c.normalColor = Color.red;
            b.colors = c;
            if (levelIndex == 0)
            {
                SceneManager.LoadScene("UIScene");
            }
            sceneToLoad++;
            map.GetComponent<Canvas>().enabled = false;
            string scene = easyLevels[Random.Range(0, easyLevels.Length)];
            SceneManager.LoadScene(scene);
        }
    }
}