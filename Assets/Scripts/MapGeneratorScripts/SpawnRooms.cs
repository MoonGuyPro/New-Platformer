using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRooms : MonoBehaviour
{
    public LayerMask whatIsRoom;
    public LevelGenerator levelGenerator;

    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, whatIsRoom);
        if (roomDetection == null && levelGenerator.stopGeneration == true)
        {
            int rand = Random.Range(0, levelGenerator.rooms.Length);
            Instantiate(levelGenerator.rooms[rand], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
