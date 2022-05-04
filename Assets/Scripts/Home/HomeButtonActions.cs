using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonActions : MonoBehaviour
{
    public void StartProEditor()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
