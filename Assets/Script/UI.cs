using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Play");
    }
    public void Exit()
    {
        Application.Quit();
    }
    
}
