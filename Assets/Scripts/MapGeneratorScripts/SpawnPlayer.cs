using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    //private Canvas loadingCanvas;
    
    public void PlayerSpawn()
    {
        Vector3 spawnPosition = transform.position;

        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        //loadingCanvas.enabled = false;
    }
}
