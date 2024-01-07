using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 To jest 2 część generowania poziomu!
 W tej klasie generujemy gamepleyowe rzeczy takie jak spawn point gracza, przeszkody, znajdźki, ołtarze.
 W następnej klasie będziemy generowac kolejne rzeczy.
*/

public class NewLevelGeneratorInteractiveObjects : MonoBehaviour
{
    
    private GameObject spawnPoint;

    private void Start()
    {
        spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint");    //szukanie spawnPointu gracza 
    }
}
