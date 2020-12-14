using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TemporaryStartGameScript : MonoBehaviour
{
    public void StartTheGameYo(){
        SceneManager.LoadScene(1);
    }
}
