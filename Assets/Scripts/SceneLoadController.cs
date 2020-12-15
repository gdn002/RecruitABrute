using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadController
{

    public enum Scene
    {
        //Here put the First Level Scene name,
        MainScene,
        MainMenuScene,
        MapScene,
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

}