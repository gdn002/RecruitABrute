using UnityEngine;
using UnityEngine.SceneManagement;

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
        Application.Quit();
    }
}