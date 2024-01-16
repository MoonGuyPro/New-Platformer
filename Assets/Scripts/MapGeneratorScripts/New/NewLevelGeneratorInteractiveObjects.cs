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

    public enum SpawnPointType
    {
        Normal,
        Altar,
        Enemies
    }
    
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
        SpawnObjectInRoom(firstRoom, spawnPoint, 1, SpawnPointType.Normal);
        lastRoom = generator.generatedRoomsOnPath[^1];
        SpawnObjectInRoom(lastRoom, finishPoint, 1, SpawnPointType.Normal);
        
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
            List<GameObject> placesToSpawnList = FindPlacesToSpawn(groundTilesInRoom, SpawnPointType.Normal);
            positionsToSpawn.AddRange(placesToSpawnList);
        }

        for (int i = 0; i < coinsAmount; i++)
        {
            GameObject spawnPoint = GetRandomPositionToSpawn(positionsToSpawn);
            Vector2 spawnPosition = new(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 1);
            positionsToSpawn.Remove(spawnPoint);
            Instantiate(coin, spawnPosition, Quaternion.identity);
        }

        for (int i = 0; i < healingPointsAmount; i++)
        {
            GameObject spawnPoint = GetRandomPositionToSpawn(positionsToSpawn);
            Vector2 spawnPosition = new(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 1);
            positionsToSpawn.Remove(spawnPoint);
            Instantiate(healingPoint, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnAltars(GameObject gameObject, int amount, List<GameObject> roomsList, bool shouldReturnList)
    {
        for (int i = 0; i <= amount; i++)       //Generujemy taką ilość jaką wybraliśmy w inspektorze
        {
            int index = Random.Range(0, roomsList.Count);       //Losujemy pokój w którym powstanie obiekt
            GameObject randomRoom = roomsList[index];
            SpawnObjectInRoom(randomRoom, gameObject, (float)0.5, SpawnPointType.Altar);
            roomsList.Remove(roomsList[index]);
        }

        if (shouldReturnList)
            shorterList = roomsList;
    }
    

    private void SpawnObjectInRoom(GameObject room, GameObject objectPrefab, float pivotAdjustment, SpawnPointType type)
    {
        List<GameObject> groundTilesInRoom = room.gameObject.GetComponent<Room>().FindGroundInRoom();
        if (groundTilesInRoom.Count != 0)
        {
            List<GameObject> placesToSpawnList = FindPlacesToSpawn(groundTilesInRoom, type);
            GameObject spawnPoint = GetRandomPositionToSpawn(placesToSpawnList);
            Vector2 spawnPosition = new(spawnPoint.transform.position.x, spawnPoint.transform.position.y + pivotAdjustment);
            placesToSpawnList.Remove(spawnPoint);
            Instantiate(objectPrefab, spawnPosition, quaternion.identity);
        }
    }

    private bool SpawnPointsController(SpawnPointType type, GameObject obj)     //Metoda która zapewnia odpowiedni rodzaj spawnu dla konkretnego obiektu - np. dla Altaru potrzebne są 2 miejsca wolne wzwyż
    {
        Vector2 position = obj.transform.position;
        switch (type)
        {
           case SpawnPointType.Normal:
               GameObject check = CheckNeighbourAtPosition(new Vector2(position.x, position.y + 1));
               if (check == null)
                   return true;
               return false;
           case SpawnPointType.Altar:
               GameObject check1 = CheckNeighbourAtPosition(new Vector2(position.x, position.y + 1));
               GameObject check2 = CheckNeighbourAtPosition(new Vector2(position.x, position.y + 2));
               GameObject check3 = CheckNeighbourAtPosition(new Vector2(position.x - 1, position.y + 1));
               GameObject check4 = CheckNeighbourAtPosition(new Vector2(position.x + 1, position.y + 1));
               GameObject check5 = CheckNeighbourAtPosition(new Vector2(position.x - 1, position.y));
               GameObject check6 = CheckNeighbourAtPosition(new Vector2(position.x + 1, position.y));
                if (check1 == null && check2 == null && check3 == null && check4 == null && check5 != null && check6 != null)
                   return true;
               return false;
            default:
               return false;
        }

    }

    public List<GameObject> FindPlacesToSpawn(List<GameObject> groundTilesList, SpawnPointType type)    //Mając liste terenu znajduje teren nad którym jest wolne miejsce i zwracam liste z tymi miejscami
    {
        List<GameObject> placesToSpawnList = new List<GameObject>();
        
        foreach (GameObject obj in groundTilesList)
        {
            if (SpawnPointsController(type, obj))
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
    
    public GameObject CheckNeighbourAtPosition(Vector2 position)
    {
        Collider2D collider = Physics2D.OverlapPoint(position, layerToCheck);
        if (collider != null)
        {
            //Debug.Log(collider.gameObject.name);
            return collider.gameObject;
        }
        return null;
    }

    public GameObject GetRandomPositionToSpawn(List<GameObject> gameObjects)
    {
        int index = Random.Range(0, gameObjects.Count);
        GameObject choosed = gameObjects[index];

        return choosed;
    }

}
