using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private int fireAltarAmount;
    [SerializeField] private GameObject iceAltar;
    private int iceAltarAmount;
    [SerializeField] private GameObject healingPoint;
    private int healingPointsAmount;
    [SerializeField] private GameObject coin;
    private int coinsAmount;

    [HideInInspector] public bool stopGeneration;
    
    [Header("Ground Variables")]    //Do sprawdzenia sąsiedztwa danego tilea
    [SerializeField] private LayerMask layerToCheck;
    
    private NewLevelGeneratorTerrain generator;
    private SpawnRooms[] objectsWithSpawnRooms;
    private GameObject firstRoom;
    private GameObject lastRoom;

    private Vector2 roomSize;   //Do określenia rozmiaru pokoju

    private List<GameObject> shorterList;       //lista w której nie am pokoi w ktorych zostały zespawnowane obiekty, potrzebna przy ołtarzach by nie powtarzac pokoju
    private NewLevelGenerator _newLevelGenerator;


    private void Start()
    {
        _newLevelGenerator = FindObjectOfType<NewLevelGenerator>();
        fireAltarAmount = _newLevelGenerator.fireAltarAmount;
        iceAltarAmount = _newLevelGenerator.iceAltarAmount;
        healingPointsAmount = _newLevelGenerator.healingPointsAmount;
        coinsAmount = _newLevelGenerator.coinsAmount;
        
        generator = FindObjectOfType<NewLevelGeneratorTerrain>();
        roomSize = new Vector2(9, 9);
        stopGeneration = false;
    }

    private void Update()
    {
        if (MapGenerated() && !stopGeneration)
        {
            SetAllInteractiveObjects();
        }
    }

    private bool MapGenerated()
    {
        int roomsGeneratedNumber = generator.generatedRandomRooms.Count + generator.generatedRoomsOnPath.Count;
        if (roomsGeneratedNumber == 16)
        {
            return true;
        }
        return false;
    }

    private void SetAllInteractiveObjects()
    {
        //Najpierw spawn gracza i koniec poziomu
        firstRoom = generator.generatedRoomsOnPath[0];
        SpawnObjectInRoom(firstRoom, spawnPoint, 1);
        lastRoom = generator.generatedRoomsOnPath[^1];
        SpawnObjectInRoom(lastRoom, finishPoint, 1);
        
        //Teraz ołtarze
        List<GameObject> roomsList = generator.generatedRandomRooms;    //Bierzemy pokoje poza główną ścieżką gracza
        if (roomsList.Count < fireAltarAmount + iceAltarAmount)         //Na wypadek jakby było za mało pokoi dostępnych poza główną ścieżką
        {
            roomsList.AddRange(generator.generatedRoomsOnPath);
        }
        
        SpawnAltars(fireAltar, fireAltarAmount, roomsList, true);
        
        SpawnAltars(iceAltar, iceAltarAmount, shorterList, false);     //tutaj z inną ograniczoną listą o te pokoje w ktorych jest juz ołtarz
        
        //Teraz pozostale obiekty takie jak coinsy i życia
        //Najpierw zbieramy całą liste wolnych miejsc do spawnowania, a potem w nich losowo spawnujemy
        SpawnCoinsAndHealing(roomsList);
        
        stopGeneration = true;
    }

    private void SpawnCoinsAndHealing(List<GameObject> roomsList)
    {
        List<GameObject> positionsToSpawn = new List<GameObject>();
        foreach (GameObject room in roomsList)
        {
            List<GameObject> groundTilesInRoom = room.gameObject.GetComponent<Room>().FindGroundInRoom();
            List<GameObject> placesToSpawnList = FindPlacesToSpawn(groundTilesInRoom);
            positionsToSpawn.AddRange(placesToSpawnList);
        }

        for (int i = 0; i < coinsAmount; i++)
        {
            Vector2 spawnPosition = GetRandomPositionToSpawn(positionsToSpawn, 1);
            Instantiate(coin, spawnPosition, Quaternion.identity);
        }

        for (int i = 0; i < healingPointsAmount; i++)
        {
            Vector2 spawnPosition = GetRandomPositionToSpawn(positionsToSpawn, 1);
            Instantiate(healingPoint, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnAltars(GameObject gameObject, int amount, List<GameObject> roomsList, bool shouldReturnList)
    {
        for (int i = 0; i <= amount; i++)       //Generujemy taką ilość jaką wybraliśmy w inspektorze
        {
            int index = Random.Range(0, roomsList.Count);       //Losujemy pokój w którym powstanie obiekt
            GameObject randomRoom = roomsList[index];
            SpawnObjectInRoom(randomRoom, gameObject, (float)0.5);
            roomsList.Remove(roomsList[index]);
        }

        if (shouldReturnList)
            shorterList = roomsList;
    }
    

    private void SpawnObjectInRoom(GameObject room, GameObject objectPrefab, float pivotAdjustment)
    {
        List<GameObject> groundTilesInRoom = room.gameObject.GetComponent<Room>().FindGroundInRoom();
        if (groundTilesInRoom.Count != 0)
        {
            List<GameObject> placesToSpawnList = FindPlacesToSpawn(groundTilesInRoom);
            Vector2 spawnPosition = GetRandomPositionToSpawn(placesToSpawnList, pivotAdjustment);
            Instantiate(objectPrefab, spawnPosition, quaternion.identity);
        }
    }

    public List<GameObject> FindPlacesToSpawn(List<GameObject> groundTilesList)    //Mając liste terenu znajduje teren nad którym jest wolne miejsce i zwracam liste z tymi miejscami
    {
        List<GameObject> placesToSpawnList = new List<GameObject>();
        
        foreach (GameObject obj in groundTilesList)
        {
            var position = obj.transform.position;
            GameObject check = CheckNeighbourAtPosition(new Vector2(position.x, position.y + 1));
            if (check == null)
            {
                placesToSpawnList.Add(obj);
            }
            
        }

        if (placesToSpawnList.Count == 0)
        {
            Debug.Log("Brak miejsca");
        }

        return placesToSpawnList;

    }
    
    GameObject CheckNeighbourAtPosition(Vector2 position)
    {
        Collider2D collider = Physics2D.OverlapPoint(position);
        if (collider != null)
        {
            return collider.gameObject;
        }
        return null;
    }

    public Vector2 GetRandomPositionToSpawn(List<GameObject> gameObjects, float pivotAdjustment)
    {
        Vector2 position = new Vector2();

        int index = Random.Range(0, gameObjects.Count);
        GameObject choosed = gameObjects[index];
        position = new Vector2(choosed.transform.position.x, choosed.transform.position.y + pivotAdjustment); //dodaje ponieważ gameobject ma pozycje ziemi nad którą jest miejsce

        return position;
    }

}
