using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 To jest 1 część generowania poziomu!
 W tej klasie generujemy sam teren poprzez losowanie pokoi w odpowiednich miejscach!
 Po wygenerowaniu terenu w następnej klasie będziemy generować kolejne elementy.
*/

public class NewLevelGeneratorTerrain : MonoBehaviour
{
    [Header("MAIN SETTING")] 
    [SerializeField] private bool randomNumberOfRooms;
    [SerializeField] private int numberOfRoomsInPath;
    
    [Header("Assignments")]
    public  GameObject[] rooms;
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private GameObject[] startingRooms;
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private LayerMask roomLayer;


    [Header("Level size variables")] 
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float moveAmount;
    [SerializeField] private float startTimeBtwRoom;
    
    public bool stopGeneration;
    public List<GameObject> generatedRoomsOnPath;
    public List<GameObject> generatedRandomRooms;

    private float timeBtwRoom;
    private bool playerSpawned;
    private Direction direction;
    private int downCounter;
    
    private int counter;
    
    private GameObject choosedRoom;
    private GameObject newRoom;
    
    
    public enum RoomType        //określa wyjścia z danego pokoju
    {
        LR,
        LRB,
        LRT,
        LRBT
    }

    /*[Serializable]
    public class DirectionProbability
    {
        public Direction direction;
        public float probability;
    }*/

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
        int rand = Random.Range(0, startingRooms.Length);
        newRoom = Instantiate(rooms[rand], transform.position, Quaternion.identity);
        generatedRoomsOnPath.Add(newRoom);
        direction = RandomEnumValue<Direction>(0);
        
    }

    private void Update()
    {
        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            if (!randomNumberOfRooms)
            {
                ProceduralRoomsGenerator();
            }
            else
            {
                RoomsGenerator();
            }
            
            timeBtwRoom = startTimeBtwRoom;
        } else
        {
            timeBtwRoom -= Time.deltaTime;
        }
    }

    private void ProceduralRoomsGenerator()
    {
        
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
                newRoom = Instantiate(rooms[rand], transform.position, Quaternion.identity);  //losujemy dowolny pokój z dostępnych
                generatedRoomsOnPath.Add(newRoom);

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
                newRoom = Instantiate(rooms[rand], transform.position, Quaternion.identity);  //losujemy dowolny pokój z dostępnych
                generatedRoomsOnPath.Add(newRoom);
                
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
                        RemoveRoomFromList(roomDetection.gameObject);
                        roomDetection.GetComponent<Room>().RoomDestruction();       //niszczymu pokój który nie pasuje

                        choosedRoom = GetRandomRoomWithType(RoomType.LRBT);
                        newRoom = Instantiate(choosedRoom, transform.position, Quaternion.identity);     //inicjalizujemy pokój typu LRTB
                        generatedRoomsOnPath.Add(newRoom);
                    }
                    else
                    {
                        RemoveRoomFromList(roomDetection.gameObject);
                        roomDetection.GetComponent<Room>().RoomDestruction();

                        choosedRoom = GetRandomRoomWithType(RoomType.LRB, RoomType.LRBT);
                        newRoom = Instantiate(choosedRoom, transform.position, Quaternion.identity);   //musimy wylosować typ pokoju z wyjściem Bottom
                        generatedRoomsOnPath.Add(newRoom);
                    }

                }

                Vector2 newPos = new Vector2(transform.position.x, transform.position.y - moveAmount);
                transform.position = newPos;

                choosedRoom = GetRandomRoomWithType(RoomType.LRT, RoomType.LRBT);
                newRoom = Instantiate(choosedRoom, transform.position, Quaternion.identity);   //musimy wylosować typ pokoju z wyjściem Top
                generatedRoomsOnPath.Add(newRoom);

                direction = RandomEnumValue<Direction>(0);
            }
            else
            {
                //STOP level generator
                stopGeneration = true;
                Debug.Log(generatedRoomsOnPath[0].transform.position);
                Debug.Log(generatedRoomsOnPath[^1].transform.position);
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

    private void RemoveRoomFromList(GameObject gameObjectToRemove)
    {
        if (generatedRoomsOnPath.Contains(gameObjectToRemove))
        {
            generatedRoomsOnPath.Remove(gameObjectToRemove);
        }
        else
        {
            Debug.Log("Room not found");
        }
    }
    
    

}
