/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform[] startingPositions;
    public GameObject[] rooms;  //  index 0 - LR; index 1 - LRB; index 2 - LRT, index 3 - LRBT

    public GameObject playerPrefab;
    public Canvas loadingCanvas;

    private int direction;
    public float moveAmount;

    private float timeBtwRoom;
    public float startTimeBtwRoom = 0.25f;

    public float minX;
    public float maxX;
    public float minY;
    public bool stopGeneration;

    public LayerMask room;

    private int downCounter;
    private bool playerSpawned;
    public GameObject spawnPoint;

    private void Start()
    {
        loadingCanvas.enabled = true;
        playerSpawned = false;
        int randStartPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint");

        direction = Random.Range(1, 6);
    }

    private void Update()
    {
        if (spawnPoint == null)
        {
            spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint");
        } 
        else if (!playerSpawned && stopGeneration)
        {
            PlayerSpawn();
            playerSpawned = true;
        }

        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            Move();
            timeBtwRoom = startTimeBtwRoom;
        } else
        {
            timeBtwRoom -= Time.deltaTime;
        }

    }

    private void Move()
    {
        if (direction == 1 || direction == 2)   //Move Right
        {
            if (transform.position.x < maxX)
            {
                downCounter = 0;

                Vector2 newPos = new Vector2(transform.position.x + moveAmount, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
                if ( direction == 3)
                {
                    direction = 2;
                } else if ( direction == 4)
                {
                    direction = 5;
                }
            }
            else
            {
                direction = 5;
            }

        } else if (direction == 3 || direction == 4)    //Move Left
        {
            if (transform.position.x > minX)
            {
                downCounter = 0;

                Vector2 newPos = new Vector2(transform.position.x - moveAmount, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(3, 6);
            }
            else
            {
                direction = 5;
            }

        } else if(direction == 5)   //Move Down
        {
            downCounter++;

            if (transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
                if (roomDetection.GetComponent<Room>().type != 1 && roomDetection.GetComponent<Room>().type != 3)
                {
                    if(downCounter >= 2)
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();
                        Instantiate(rooms[3], transform.position, Quaternion.identity);
                    }
                    else
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();

                        int randBottomRoom = Random.Range(1, 4);
                        if (randBottomRoom == 2)
                        {
                            randBottomRoom = 1;
                        }
                        Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                    }


                }

                Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmount);
                transform.position = newPos;

                int rand = Random.Range(2, 4);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
            }
            else
            {
                //STOP level generator
                stopGeneration = true;
            }

        }

        
    }

    private void PlayerSpawn()
    {
        Vector3 spawnPosition = spawnPoint.transform.position;

        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        loadingCanvas.enabled = false;

    }

    
}
*/
