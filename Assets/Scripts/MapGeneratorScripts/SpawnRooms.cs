using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawnuje pozostałe pokoje w wolnych miejscach po wygenerowaniu głównej ścieżki

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public NewLevelGeneratorTerrain newLevelGenerator;
    
    private GameObject newRoom;
    

    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
        if (roomDetection == null && newLevelGenerator.stopGeneration)
        {
            int rand = Random.Range(0, newLevelGenerator.rooms.Length);
            newRoom = Instantiate(newLevelGenerator.rooms[rand], transform.position, Quaternion.identity);
            newLevelGenerator.generatedRandomRooms.Add(newRoom);
            Destroy(gameObject);
        }
    }
}
