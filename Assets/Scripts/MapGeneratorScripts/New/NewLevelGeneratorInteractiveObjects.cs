using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 To jest 2 część generowania poziomu!
 W tej klasie generujemy gamepleyowe rzeczy takie jak spawn point gracza, przeszkody, znajdźki, ołtarze, mete.
 W następnej klasie będziemy generowac kolejne rzeczy.
*/

public class NewLevelGeneratorInteractiveObjects : MonoBehaviour
{
    [Header("Interactive objects to spawn")]
    [SerializeField] private GameObject finishPoint;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject fireAltar;
    [SerializeField] private GameObject iceAltar;
    [SerializeField] private GameObject healingPoint;
    [SerializeField] private GameObject coin;

    [HideInInspector] public bool stopGeneration;

    [Header("Ground Variables")]    //Do sprawdzenia sąsiedztwa danego tilea
    [SerializeField] private LayerMask layerToCheck;
    [SerializeField] private float checkDistance;
    private GameObject leftNeighbour, rightNeighbour, topNeighbour, bottomNeighbour;
    
    private NewLevelGeneratorTerrain generator;
    private GameObject firstRoom;
    private GameObject lastRoom;
    
    // Szukamy wolnych miejsc gdzie jest możliwość spawnu 
    public List<Transform> spawnPlaces;     //miejsca w których możemy zespawnować obiekty


    private void Start()
    {
        generator = FindObjectOfType<NewLevelGeneratorTerrain>();
    }

    private void Update()
    {
        if (generator.stopGeneration)
        {
            SetEntranceAndExit();
        }
    }

    private void SetEntranceAndExit()
    {
        firstRoom = generator.generatedRoomsOnPath[0];
        
        lastRoom = generator.generatedRoomsOnPath[^1];
    }
}
