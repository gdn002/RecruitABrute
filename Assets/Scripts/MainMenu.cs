using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private void Awake()
    {
        transform.Find("startGameBtn").GetComponent<Button>().onClick.AddListener(() => {
            //Change this to the respected name of the initial scene
            SceneLoadController.Load(SceneLoadController.Scene.MainScene);
        
        });

        transform.Find("quiteGameBtn").GetComponent<Button>().onClick.AddListener(() => {
            Application.Quit();

        });
    }

}
