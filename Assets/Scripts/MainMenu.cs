using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private void Awake()
    {


       

    }


    public void StartTheGame()
    {
        SceneManager.LoadScene(1);
    }



    public void QuitTheGame()
    {
    transform.Find("quiteGameBtn").GetComponent<Button>().onClick.AddListener(() => {
        Application.Quit();
    });

    }


}
