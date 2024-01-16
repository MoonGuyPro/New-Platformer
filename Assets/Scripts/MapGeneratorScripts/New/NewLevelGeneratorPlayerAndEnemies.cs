using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<GameObject> freeSpaces;        //Dla przeciwników i kręcącego koła

    [Serializable]
    public class Enemies
    {
        public GameObject typeOfEnemy;
        public double probability;
        public float pivotAdjustment;

        public Enemies(GameObject _typeOfEnemy, double _probability, float _pivotAdjustment)
        {
            typeOfEnemy = _typeOfEnemy;
            probability = _probability;
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
        meleeEnemy = new Enemies(meleeEnemyPrefab, 0.25, (float)1.2);
        rangedEnemy = new Enemies(rangeEnemyPrefab, 0.25, (float)1.2);
        sawTrap = new Enemies(movingSawPrefab, 0.25, (float)0.6);
        spikesTrap = new Enemies(spikesPrefab, 0.25,(float) 1.2);
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
        roomsList.AddRange(terrainGenerator.generatedRoomsOnPath.Skip(1));


        foreach (GameObject room in roomsList)      //dodajemy do listy wszystkie znalezione wolne tile
        {
            List<GameObject> groundPositions = room.gameObject.GetComponent<Room>().FindGroundInRoom();
            freeSpaces.AddRange(objectsGenerator.FindPlacesToSpawn(groundPositions, NewLevelGeneratorInteractiveObjects.SpawnPointType.Normal));
        }

        int safeTiles;          //liczba tileów bezpiecznych i niebezpiecznych
        int dangerousTiles;

        safeTiles = Mathf.RoundToInt(safeTilesRatio * freeSpaces.Count);
        dangerousTiles = freeSpaces.Count - safeTiles;

        int dangerousTilesFilled = 0;
        while (dangerousTiles > dangerousTilesFilled)
        {
            Enemies enemy = ChooseRandomEnemyOrTrap();
            GameObject spawnPoint = objectsGenerator.GetRandomPositionToSpawn(freeSpaces);
            Vector2 position = new(spawnPoint.transform.position.x, spawnPoint.transform.position.y + enemy.pivotAdjustment);
            freeSpaces.Remove(spawnPoint);
            GameObject enemyPrefab = enemy.typeOfEnemy;

            dangerousTilesFilled += NumberOfDangerousTiles(enemy, position, enemy.pivotAdjustment);

            Instantiate(enemyPrefab, position, Quaternion.identity);
        }

        SpawnPlayer player = FindObjectOfType<SpawnPlayer>();
        player.PlayerSpawn();
        stopGenerator = true;

    }

    private int NumberOfDangerousTiles(Enemies enemy, Vector2 position, float pivot)
    {
        if(enemy == spikesTrap)
        {
            return 1;
        }
        else
        {
            int dangerousTilesNumber = 1;
            Vector2 newPosLeft = new Vector2(position.x - 1, position.y - pivot + 1);
            Vector2 newPosRight = new Vector2(position.x + 1, position.y - pivot + 1);
            Vector2 newPosDownLeft = new Vector2(position.x - 1, position.y - pivot);
            Vector2 newPosDownRight = new Vector2(position.x + 1, position.y - pivot);
            bool checkLeft = false;
            bool checkRight = false;


            while (!checkLeft)
            {
                if (objectsGenerator.CheckNeighbourAtPosition(newPosLeft) != null || objectsGenerator.CheckNeighbourAtPosition(newPosDownLeft) == null)
                {
                    checkLeft = true;
                }
                else
                {
                    dangerousTilesNumber++;
                    newPosLeft.x -= 1;
                    newPosDownLeft.x -= 1;
                }
            }

            while (!checkRight)
            {
                if (objectsGenerator.CheckNeighbourAtPosition(newPosRight) != null || objectsGenerator.CheckNeighbourAtPosition(newPosDownRight) == null)
                {
                    checkRight = true;
                }
                else
                {
                    dangerousTilesNumber++;
                    newPosRight.x += 1;
                    newPosDownRight.x += 1;
                }
            }
            Debug.Log("Pos " + position);
            Debug.Log(enemy.typeOfEnemy + " " + dangerousTilesNumber);

            if (dangerousTilesNumber > 6)
                dangerousTilesNumber = 6;

            return dangerousTilesNumber;
            
        }
        
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
