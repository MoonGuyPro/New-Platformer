using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EndGame : MonoBehaviour
{

    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager.PlayerWon();
            string folderPath = Path.Combine(Application.dataPath, "..");
            string filePath = Path.Combine(folderPath, "heatMap_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
            playerMovement.SaveHeatMapToFile(playerMovement.heatMapTexture, filePath);
        }
        Time.timeScale = 0f;
    }
}
