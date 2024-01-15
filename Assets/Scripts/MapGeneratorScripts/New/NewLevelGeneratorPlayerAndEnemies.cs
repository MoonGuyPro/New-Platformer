using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 To jest 3, ostatnia część generowania poziomu!
 W tej klasie generujemy gracza i przeciwników po wygenerowaniu poprzednich rzeczy.
*/

public class NewLevelGeneratorPlayerAndEnemies : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangeEnemyPrefab;
    [Header("Traps")] 
    [SerializeField] private GameObject movingSawPrefab;
    [SerializeField] private GameObject spikesPrefab;

    private float safeTilesRatio;

    private NewLevelGeneratorTerrain terrainGenerator;
    private NewLevelGeneratorInteractiveObjects objectsGenerator;
    private bool stopGenerator;

    private List<GameObject> freeSpaces;

    [Serializable]
    public class Enemies
    {
        public GameObject typeOfEnemy;
        public double probability;
        public int numberOfTiles;
        public float pivotAdjustment;

        public Enemies(GameObject _typeOfEnemy, double _probability, int _numberOfTiles, float _pivotAdjustment)
        {
            typeOfEnemy = _typeOfEnemy;
            probability = _probability;
            numberOfTiles = _numberOfTiles;
            pivotAdjustment = _pivotAdjustment;
        }
    }
    
    private Enemies meleeEnemy;
    private Enemies rangedEnemy;
    private Enemies sawTrap;
    private Enemies spikesTrap;
    private List<Enemies> enemiesList;
    
    private NewLevelGenerator _newLevelGenerator;

    private void Start()
    {
        _newLevelGenerator = FindObjectOfType<NewLevelGenerator>();
        safeTilesRatio = _newLevelGenerator.safeTilesRatio;
        
        terrainGenerator = FindObjectOfType<NewLevelGeneratorTerrain>();
        objectsGenerator = FindObjectOfType<NewLevelGeneratorInteractiveObjects>();
        freeSpaces = new List<GameObject>();
        stopGenerator = false;
        SetStartingProbabilities();
    }

    private void SetStartingProbabilities()
    {
        meleeEnemy = new Enemies(meleeEnemyPrefab, 0.25, 3, (float)1.2);
        rangedEnemy = new Enemies(rangeEnemyPrefab, 0.25, 3, (float)1.2);
        sawTrap = new Enemies(movingSawPrefab, 0.25, 7, (float)0.6);
        spikesTrap = new Enemies(spikesPrefab, 0.25, 1,(float) 1.2);
        enemiesList = new List<Enemies>
        {
            meleeEnemy,
            rangedEnemy,
            sawTrap,
            spikesTrap
        };
    }

    private void Update()
    {
        if (objectsGenerator.stopGeneration && !stopGenerator)
        {
            SpawnEnemiesAndTraps();
        }
    }


    private void SpawnEnemiesAndTraps()
    {
        List<GameObject> roomsList = new List<GameObject>();    //Dodaje wszystkie pokoje do listy
        roomsList.AddRange(terrainGenerator.generatedRandomRooms);
        roomsList.AddRange(terrainGenerator.generatedRoomsOnPath);
        

        foreach (GameObject room in roomsList)      //dodajemy do listy wszystkie znalezione wolne tile
        {
            List<GameObject> groundPositions = room.gameObject.GetComponent<Room>().FindGroundInRoom();
            freeSpaces.AddRange(objectsGenerator.FindPlacesToSpawn(groundPositions));
        }

        int safeTiles;          //liczba tileów bezpiecznych i niebezpiecznych
        int dangerousTiles;

        safeTiles = Mathf.RoundToInt(safeTilesRatio * freeSpaces.Count);
        dangerousTiles = freeSpaces.Count - safeTiles;

        int dangerousTilesFilled = 0;
        while (dangerousTiles > dangerousTilesFilled)
        {
            Enemies enemy = ChooseRandomEnemyOrTrap();
            Vector2 position = objectsGenerator.GetRandomPositionToSpawn(freeSpaces, enemy.pivotAdjustment);
            GameObject enemyPrefab = enemy.typeOfEnemy;
            Instantiate(enemyPrefab, position, Quaternion.identity);
            
            dangerousTilesFilled += enemy.numberOfTiles;
        }

        SpawnPlayer player = FindObjectOfType<SpawnPlayer>();
        player.PlayerSpawn();
        stopGenerator = true;

    }
    
    private Enemies ChooseRandomEnemyOrTrap()
    {
        double totalProbability = 0;
        foreach (var enemy in enemiesList)
        {
            totalProbability += enemy.probability;
        }

        double randomPoint = Random.value * totalProbability;

        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (randomPoint < enemiesList[i].probability)
            {
                AdjustProbabilities(i);
                return enemiesList[i];
            }
            randomPoint -= enemiesList[i].probability;
        }

        return null; // Powinno wystąpić tylko w przypadku pustej listy lub błędu
    }
    
    private void AdjustProbabilities(int selectedIndex)
    {
        double decreaseAmount = enemiesList[selectedIndex].probability * 0.1; // Na przykład 10% obniżenia
        double increaseAmount = decreaseAmount / (enemiesList.Count - 1);

        enemiesList[selectedIndex].probability -= decreaseAmount;

        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (i != selectedIndex)
            {
                enemiesList[i].probability += increaseAmount;
            }
        }

        // Normalizuj, jeśli to konieczne, aby upewnić się, że suma wynosi 1
        NormalizeProbabilities();
    }

    private void NormalizeProbabilities()
    {
        double total = 0;
        foreach (var enemy in enemiesList)
        {
            total += enemy.probability;
        }

        if (Mathf.Abs((float)(total - 1.0)) > double.Epsilon)
        {
            foreach (var enemy in enemiesList)
            {
                enemy.probability /= total;
            }
        }
    }

}
