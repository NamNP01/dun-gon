using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PauseMenu;
    public bool IsPaused;
    //public Text timerText;
    //public Text ScoreTextPauseMenu;
    //public Text ScoreTextGameOver;
    private float startingTime = 0f;
    //public PlayerData Data;

    // Start is called before the first frame update
    void Start()
    {
        startingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //Data.currentTime = Time.time - startingTime;
        //UpdateTimerDisplay();
        //ScoreTextPauseMenu.text = " " + Data.scoreValue;
        //ScoreTextGameOver.text = " " + Data.scoreValue;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    //void UpdateTimerDisplay()
    //{
    //    int minutes = Mathf.FloorToInt(Data.currentTime / 60);
    //    int seconds = Mathf.FloorToInt(Data.currentTime % 60);
    //    string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
    //    timerText.text = "" + timeString;
    //}
    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }
    public void PlayAgain()
    {
        // Tìm PlayerHP và tắt GameOver nếu tìm thấy
        PlayerHP playerHP = FindFirstObjectByType<PlayerHP>();
        if (playerHP != null && playerHP.GameOver != null)
        {
            playerHP.GameOver.SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        IsPaused = false;
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
}
