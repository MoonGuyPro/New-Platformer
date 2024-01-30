using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;

    private void Update()
    {
        // Sprawdza czy klawisz R został naciśnięty
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        // Sprawdza czy klawisz Q został naciśnięty
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Debug.Log("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Tylko w Unity Editor
#else
        Application.Quit(); // Dla zbudowanej aplikacji
#endif
        Debug.Log("Quit");
    }

    public void PlayerWon()
    {
        winScreen.SetActive(true);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
