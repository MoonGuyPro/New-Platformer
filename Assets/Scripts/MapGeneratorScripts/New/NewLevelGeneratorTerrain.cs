using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 To jest 1 część generowania poziomu!
 W tej klasie generujemy sam teren!
 Po wygenerowaniu terenu w następnej klasie będziemy generować kolejne elementy.
*/

public class NewLevelGeneratorTerrain : MonoBehaviour
{
    [Header("Assignments")]
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private LayerMask roomLayer;

    [Header("Level size variables")] 
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float moveAmount;
    [SerializeField] private float startTimeBtwRoom;
    
    private float timeBtwRoom;
    private bool stopGeneration;
    private bool playerSpawned;
    private Direction direction;
    private int downCounter;
    
    public enum RoomType        //określa wyjścia z danego pokoju
    {
        LR,
        LRB,
        LRT,
        LRBT
    }

    public enum Direction
    {
        Right1,
        Right2,
        Left1,
        Left2,
        Down
    }

    private void Start()
    {
        loadingCanvas.enabled = true;   //loading screen
        playerSpawned = false;
        
        //Losowanie pozycji startowej z tablicy i inicjalizacja 1 pokoju
        int randStartPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = RandomEnumValue<Direction>(0);
    }

    private void Update()
    {
        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            RoomsGenerator();
            timeBtwRoom = startTimeBtwRoom;
        } else
        {
            timeBtwRoom -= Time.deltaTime;
        }
    }

    private void RoomsGenerator()
    {
        if (direction == Direction.Right1 || direction == Direction.Right2)     //IDZIEMY W PRAWO
        {
            if (transform.position.x < maxX)
            {
                downCounter = 0;    //nie zmieniamy poziomu

                Vector2 newPos = new Vector2(transform.position.x + moveAmount, transform.position.y);  //określamy pozycje dla wygenerowania pokoju
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);  //losujemy dowolny pokój z dostępnych

                direction = RandomEnumValue<Direction>(0);
                
                if ( direction == Direction.Left1)      //koryguje prawdopodobieństwo żeby pozycja generowania nie wróciła do miejsca gdzie już wygenerowano pokój
                {
                    direction = Direction.Right2;
                } else if ( direction == Direction.Left2)
                {
                    direction = Direction.Down;
                }
            }
            else
            {
                direction = Direction.Down;
            }
        } else if (direction == Direction.Left1 || direction == Direction.Left2)    //IDZIEMY W LEWO
        {
            if (transform.position.x > minX)
            {
                downCounter = 0;    //nie zmieniamy poziomu

                Vector2 newPos = new Vector2(transform.position.x - moveAmount, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);  //losujemy dowolny pokój z dostępnych

                direction = RandomEnumValue<Direction>(3);
            }
            else
            {
                direction = Direction.Down;
            }
        } else if(direction == Direction.Down)   //IDZIEMY W DÓŁ
        {
            downCounter++;      //zwiększamy ponieważ schodzimy w dół

            if (transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, roomLayer);    //sprawdzamy jakie pokoje są w pobliżu
                
                if (roomDetection.GetComponent<Room>().roomType != RoomType.LRB && roomDetection.GetComponent<Room>().roomType != RoomType.LRBT)
                {
                    if(downCounter >= 2)
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();       //niszczymu pokój który nie pasuje
                        Instantiate(GetRandomRoomWithType(RoomType.LRBT), transform.position, Quaternion.identity);     //inicjalizujemy pokój typu LRTB
                    }
                    else
                    {
                        roomDetection.GetComponent<Room>().RoomDestruction();
                        Instantiate(GetRandomRoomWithType(RoomType.LRB, RoomType.LRBT), transform.position, Quaternion.identity);   //musimy wylosować typ pokoju z wyjściem Bottom
                    }


                }

                Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmount);
                transform.position = newPos;
                
                Instantiate(GetRandomRoomWithType(RoomType.LRT, RoomType.LRBT), transform.position, Quaternion.identity);   //musimy wylosować typ pokoju z wyjściem Top

                direction = RandomEnumValue<Direction>(0);
            }
            else
            {
                //STOP level generator
                stopGeneration = true;
            }

        }
    }

    private T RandomEnumValue<T>(int startNumber) where T : Enum
    {
        T[] enumValues = (T[])Enum.GetValues(typeof(T));
        T randomValue = enumValues[Random.Range(startNumber, enumValues.Length)];
        return randomValue;
    }

    private GameObject GetRandomRoomWithType(params RoomType[] roomTypes)     //losujemy pokój danego typu/typów
    {
        List<GameObject> matchingRooms = new List<GameObject>();
        
        foreach (GameObject room in rooms)
        {
            Room roomScript = room.GetComponent<Room>();
            if (roomScript != null && Array.Exists(roomTypes, type => type == roomScript.roomType))
            {
                matchingRooms.Add(room);
            }
        }

        if (matchingRooms.Count == 0)
        {
            return null; // Nie znaleziono pokoi pasujących do kryterium
        }

        int randomIndex = Random.Range(0, matchingRooms.Count);
        return matchingRooms[randomIndex];
    }
    
    
    /*private RoomType RandomRoomType()
    {
        RoomType[] roomTypes = (RoomType[])Enum.GetValues(typeof(RoomType));
        RoomType randomRoom = roomTypes[Random.Range(0, roomTypes.Length)];
        return randomRoom;
    }*/
    

}
