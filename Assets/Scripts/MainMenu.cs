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


    public void StartTheGameYo()
    {
        SceneManager.LoadScene(1);
    }



    public void QuitTheGameYo()
    {
    transform.Find("quiteGameBtn").GetComponent<Button>().onClick.AddListener(() => {
        Application.Quit();
    });

    }


}
