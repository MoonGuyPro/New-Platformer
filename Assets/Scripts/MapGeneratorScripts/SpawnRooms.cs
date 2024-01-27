using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawnuje pozostałe pokoje w wolnych miejscach po wygenerowaniu głównej ścieżki

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public NewLevelGeneratorTerrain newLevelGenerator;

    [SerializeField] private int firstRoomWeight;
    [SerializeField] private int secondRoomWeight;
    [SerializeField] private int thirdRoomWeight;
    [SerializeField] private int fourRoomWeight;
    [SerializeField] private int fiveRoomWeight;
    
    private GameObject newRoom;
    

    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
        if (roomDetection == null && newLevelGenerator.stopGeneration)
        {
            int rand = GetWeightedRandomRoomIndex();
            newRoom = Instantiate(newLevelGenerator.rooms[rand], transform.position, Quaternion.identity);
            newLevelGenerator.generatedRandomRooms.Add(newRoom);
            Destroy(gameObject);
        }
    }
    
    int GetWeightedRandomRoomIndex()        //losowanie z wagą by zapewnic mniejsze prawdopodibienstwo dla wylosowania pokoju z wyjsciem na dole
    {
        int[] weights = {firstRoomWeight, secondRoomWeight, thirdRoomWeight, fourRoomWeight, fiveRoomWeight}; // wagi dla indeksów 0, 1, 2, 3, 4
        int totalWeight = 0;

        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulatedWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulatedWeight += weights[i];
            if (randomValue < cumulatedWeight)
            {
                return i;
            }
        }

        return 0; // Domyślnie zwraca 0 na wypadek nieoczekiwanego błędu
    }
}
