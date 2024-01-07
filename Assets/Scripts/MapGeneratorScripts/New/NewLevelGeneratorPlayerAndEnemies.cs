using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 To jest 3, ostatnia część generowania poziomu!
 W tej klasie generujemy gracza i przeciwników po wygenerowaniu poprzednich rzeczy.
*/

public class NewLevelGeneratorPlayerAndEnemies : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    /*private void PlayerSpawn()
    {
        Vector3 spawnPosition = spawnPoint.transform.position;

        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        loadingCanvas.enabled = false;
    }*/
}
