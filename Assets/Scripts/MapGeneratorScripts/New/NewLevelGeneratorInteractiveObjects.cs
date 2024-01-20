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
    
    public Texture2D heatMapTexture;
    private bool roomsAddedToHeatMap;
    public static int width = 1000; // Szerokosc mapy cieplnej
    public static int height = 1000; // Wysokosc mapy cieplnej

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
        shorterList = new List<GameObject>();
        
        generator = FindObjectOfType<NewLevelGeneratorTerrain>();
        roomSize = new Vector2(9, 9);
        stopGeneration = false;
        roomsAddedToHeatMap = false;
        heatMapTexture = generator.heatMapTexture;
    }

    private void Update()
    {
        if (MapGenerated() && !stopGeneration)
        {
            SetAllInteractiveObjects();
            if (!roomsAddedToHeatMap)
            {
                AddRoomsToHeatMap();
                roomsAddedToHeatMap = true;
            }
        }
        
    }

    private void AddRoomsToHeatMap()
    {
        foreach (GameObject randomRoom in generator.generatedRandomRooms)
        {
            Vector2 roomPos = randomRoom.transform.position;
            Vector2 boxSize = new Vector2(10, 10); // Rozmiar kwadratu 10x10
            Collider2D[] colliders = Physics2D.OverlapBoxAll(roomPos, boxSize, 0, layerToCheck);

            foreach (Collider2D collider in colliders)
            {
                Vector2 colliderPosition = collider.transform.position;
                int groundX = (int)(((colliderPosition.x + 10) / 40.0f) * width);
                int groundY = (int)(((colliderPosition.y + 10) / 40.0f) * height);
                groundX = Mathf.Clamp(groundX, 0, width - 1);
                groundY = Mathf.Clamp(groundY, 0, height - 1);

                // Rysowanie pikseli na mapie cieplnej
                UpdateHeatMapPixel(groundX, groundY, Color.blue);
            }
        }

        heatMapTexture.Apply();
    }

    private void UpdateHeatMapPixel(int x, int y, Color color)
    {
        int dotSize = 8; // Rozmiar kropki

        for (int dx = -dotSize; dx <= dotSize; dx++)
        {
            for (int dy = -dotSize; dy <= dotSize; dy++)
            {
                int drawX = Mathf.Clamp(x + dx, 0, width - 1);
                int drawY = Mathf.Clamp(y + dy, 0, height - 1);
                heatMapTexture.SetPixel(drawX, drawY, color);
            }
        }
    }
    
    public bool MapGenerated()
    {
        int roomsGeneratedNumber = generator.generatedRandomRooms.Count + generator.generatedRoomsOnPath.Count;
        if (roomsGeneratedNumber >= 16)
        {
            return true;
        }
        return false;
    }

    private void SetAllInteractiveObjects()
    {
        //Najpierw spawn gracza i koniec poziomu
        firstRoom = generator.generatedRoomsOnPath[0];
        SpawnObjectInRoom(firstRoom, spawnPoint, 1.48f, SpawnPointType.Altar);
        lastRoom = generator.generatedRoomsOnPath[^1];
        SpawnObjectInRoom(lastRoom, finishPoint, 1, SpawnPointType.Normal);
        
        //Teraz ołtarze
        List<GameObject> roomsList = generator.generatedRandomRooms;    //Bierzemy pokoje poza główną ścieżką gracza
        if (roomsList.Count < fireAltarAmount + iceAltarAmount)         //Na wypadek jakby było za mało pokoi dostępnych poza główną ścieżką
        {
            roomsList.AddRange(generator.generatedRoomsOnPath);
        }

        shorterList.AddRange(roomsList);
        SpawnAltars(fireAltar, fireAltarAmount, shorterList);
        
        SpawnAltars(iceAltar, iceAltarAmount, shorterList);     //tutaj z inną ograniczoną listą o te pokoje w ktorych jest juz ołtarz
        
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
            DrawCircleOnHeatMap(spawnPosition, Color.yellow);
        }

        for (int i = 0; i < healingPointsAmount; i++)
        {
            GameObject spawnPoint = GetRandomPositionToSpawn(positionsToSpawn);
            Vector2 spawnPosition = new(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 1);
            positionsToSpawn.Remove(spawnPoint);
            Instantiate(healingPoint, spawnPosition, Quaternion.identity);
            DrawPlusOnHeatMap(spawnPosition, Color.green);
        }
    }
    
    private void DrawCircleOnHeatMap(Vector2 position, Color color)
    {
        int radius = 4; // Możesz dostosować rozmiar koła
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    SetHeatMapPixel(position, x, y, color);
                }
            }
        }
        heatMapTexture.Apply();
    }

    private void DrawPlusOnHeatMap(Vector2 position, Color color)
    {
        int size = 5; // Rozmiar plusa
        for (int i = -size; i <= size; i++)
        {
            // Pionowa część plusa
            SetHeatMapPixel(position, i, 0, color);
            // Pozioma część plusa
            SetHeatMapPixel(position, 0, i, color);
        }
        heatMapTexture.Apply();
    }
    
    private void SetHeatMapPixel(Vector2 position, int dx, int dy, Color color)
    {
        int pixelX = Mathf.Clamp((int)((position.x + 10) / 40.0f * width) + dx, 0, width - 1);
        int pixelY = Mathf.Clamp((int)((position.y + 10) / 40.0f * height) + dy, 0, height - 1);
        heatMapTexture.SetPixel(pixelX, pixelY, color);
    }

    private void SpawnAltars(GameObject gameObject, int amount, List<GameObject> roomsList)
    {
        for (int i = 0; i < amount; i++)       //Generujemy taką ilość jaką wybraliśmy w inspektorze
        {
            int index = Random.Range(0, roomsList.Count);       //Losujemy pokój w którym powstanie obiekt
            GameObject randomRoom = roomsList[index];
            SpawnObjectInRoom(randomRoom, gameObject, (float)0.5, SpawnPointType.Altar);
            shorterList.Remove(shorterList[index]);
        }


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
            case SpawnPointType.Enemies:
                GameObject check7 = CheckNeighbourAtPosition(new Vector2(position.x - 1, position.y + 1));
                GameObject check8 = CheckNeighbourAtPosition(new Vector2(position.x + 1, position.y + 1));
                GameObject check9 = CheckNeighbourAtPosition(new Vector2(position.x, position.y + 1));
                GameObject check10 = CheckNeighbourAtPosition(new Vector2(position.x - 1, position.y));
                GameObject check11 = CheckNeighbourAtPosition(new Vector2(position.x + 1, position.y));
                if (check7 == null && check8 == null && check9 == null && check10 != null && check11 != null)
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
        if (gameObjects.Count == 0)
        {
            Debug.Log("Lista gameObjects jest pusta.");
            return null;
        }

        int index = Random.Range(0, gameObjects.Count);
        GameObject choosed = gameObjects[index];

        return choosed;
    }

}
