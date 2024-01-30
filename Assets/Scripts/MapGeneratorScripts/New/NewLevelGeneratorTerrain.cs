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
    private int numberOfRoomsInPath;
    
    [Header("Assignments")]
    public  GameObject[] rooms;
    [SerializeField] private Transform[] startingPositions;
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private LayerMask groundLayer;


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
    private Direction direction;
    private int downCounter;
    
    private int counter;
    
    private GameObject choosedRoom;
    private GameObject newRoom;

    private Vector2 currentPosition;
    private int currentRow = 0;
    private int roomsInPathSpawned;
    private NewDirection newDirection;
    private bool firstRoomSpawned;
    
    public Texture2D heatMapTexture;
    public static int width = 1000; // Szerokosc mapy cieplnej
    public static int height = 1000; // Wysokosc mapy cieplnej


    public enum RoomType        //określa wyjścia z danego pokoju
    {
        LR,
        LRB,
    }

    public enum Direction
    {
        Right1,
        Right2,
        Left1,
        Left2,
        Down
    }

    public enum NewDirection
    {
        Right,
        Left,
        Down
    }

    private List<NewDirection> directionsList;
    private int randStartPos;
    private bool skip;
    //private RoomType[] roomTypes = { RoomType.LR, RoomType.LRB, RoomType.LRT, RoomType.LRBT };
    
    private NewLevelGenerator _newLevelGenerator;
    private void Start()
    {
        _newLevelGenerator = FindObjectOfType<NewLevelGenerator>();
        numberOfRoomsInPath = _newLevelGenerator.numberOfRoomsInPath;
        
        loadingCanvas.enabled = true;   //loading screen
        firstRoomSpawned = false;
        directionsList = new List<NewDirection>();
        SetHeatMapBackground();

    }

    private void SetHeatMapBackground()
    {
        // Inicjalizacja tekstury heatMap
        heatMapTexture = new Texture2D(width, height);

        // Wypełnienie tekstury kolorem czarnym
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black; // Ustawienie każdego piksela na czarny
        }
        heatMapTexture.SetPixels(pixels);
        heatMapTexture.Apply();
    }

    private void Update()
    {
        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            ProceduralRoomsGenerator();
            
            timeBtwRoom = startTimeBtwRoom;
        } else
        {
            timeBtwRoom -= Time.deltaTime;
        }
    }

    private void ProceduralRoomsGenerator()
    {
        
        if (numberOfRoomsInPath > 0)
        {
            switch (currentRow)
            {
                case 0:
                    if (numberOfRoomsInPath != 16 && !firstRoomSpawned)
                    {
                        SpawnFirstRoom();
                    }
                    ChooseDirectionBasedOnTheRow(currentRow);
                    break;
                case 1:
                    ChooseDirectionBasedOnTheRow(currentRow);
                    break;
                case 2:
                    ChooseDirectionBasedOnTheRow(currentRow);
                    break;
                case 3:
                    ChooseDirectionBasedOnTheRow(currentRow);
                    break;
                
            }

            if (!skip)
            {
                ApplyMovement();
                // Spawn pokoju na nowej pozycji
                int number = 16 - (currentRow) * 4;
                if (numberOfRoomsInPath == number && newDirection == NewDirection.Down)
                {
                    FillTheRow(currentRow);
                }
                else
                {
                    //int rand1 = Random.Range(0, rooms.Length);
                    newRoom = Instantiate(choosedRoom, currentPosition, Quaternion.identity);
                    generatedRoomsOnPath.Add(newRoom);
                    numberOfRoomsInPath--;
                }

            }
            
        }
        else
        {
            UpdateHeatMapWithPath();
            stopGeneration = true;
        }


    }
    
    private void SpawnFirstRoom()       //Spawnuje 1 pokój, wybiera kierunke w którą idie dalej i odrazu wybiera odpowiedni rodzaj pokoju
    {
        if (numberOfRoomsInPath is 15 or 14)
        {
            randStartPos = Random.Range(0, 2);
            if (randStartPos == 0)
                newDirection = NewDirection.Right;
            else
                newDirection = NewDirection.Left;
        }
        else
        {
            //Losowanie pozycji startowej z tablicy i inicjalizacja 1 pokoju
            randStartPos = Random.Range(0, startingPositions.Length);
            newDirection = (NewDirection)Random.Range(0, 2);
        }
        currentPosition = startingPositions[randStartPos].position;

        if (newDirection == NewDirection.Down)
        {
            choosedRoom = GetRandomRoomWithType(RoomType.LRB);
            newRoom = Instantiate(choosedRoom, currentPosition, Quaternion.identity);
        }
        else
        {
            choosedRoom = GetRandomRoomWithType(RoomType.LR);
            newRoom = Instantiate(choosedRoom, currentPosition, Quaternion.identity);
        }

        generatedRoomsOnPath.Add(newRoom);

        //Kod do nowego generowania
        numberOfRoomsInPath--;
        skip = false;
        firstRoomSpawned = true;

    }

    private void ChooseDirectionBasedOnTheRow(int row)
    {
        int number = 16 - (row) * 4;
        if (numberOfRoomsInPath == number)
        {
            FillTheRow(row);
            //Debug.Log("fillrow");
        }
        else if (numberOfRoomsInPath > number - 4)      //w momencie gdy nie moze zejsc w dół
        {
            //Debug.Log("1" + " "  + numberOfRoomsInPath + " row: " + row);
            if (newDirection == NewDirection.Down)
            {
                if (Math.Round(currentPosition.x) <= 5)
                {
                    newDirection = NewDirection.Right;
                }
                else if (Math.Round(currentPosition.x) >= 15)
                {
                    newDirection = NewDirection.Left;
                }
            }
            else if (newDirection == NewDirection.Right)
            {
                newDirection = NewDirection.Right;
            }
            else
            {
                newDirection = NewDirection.Left;
            }

            skip = false;
        } 
        else if (numberOfRoomsInPath == 3 - row)    //w celu zapewnienia ze algorytm dojdzie do konca mapy
        {
            //Debug.Log("2" + " " + numberOfRoomsInPath+ " row: " + row);
            newDirection = NewDirection.Down;
        } 
        else if (numberOfRoomsInPath == number - 4)     //w celu zapewnienia, że nie bedzie sie tworzył 1 pokoj ktorego mozna nie odwiedzic przez fill row
        {
            if ((Math.Round(currentPosition.x) == -5 || Math.Round(currentPosition.x) == 25) && newDirection == NewDirection.Down)
            {
                newDirection = (NewDirection)Random.Range(0, 3); 
            }
            else if ((Math.Round(currentPosition.x) == -5 || Math.Round(currentPosition.x) == 25) &&
                     newDirection != NewDirection.Down)
            {
                newDirection = NewDirection.Down;
            }
            else if (newDirection == NewDirection.Down)
            {
                newDirection = (NewDirection)Random.Range(0, 2);        //Tylko lewo i prawo
            }
        }   
        else if (numberOfRoomsInPath < number - 4)      //w momencie gdy moze zejsc w dół
        {
            //Debug.Log("3" + " " + numberOfRoomsInPath+ " row: " + row);
            if (newDirection == NewDirection.Left)
            {
                newDirection = (NewDirection)Random.Range(1, 3);    
            }
            else if (newDirection == NewDirection.Right)
            {
                directionsList.Add(NewDirection.Right);
                directionsList.Add(NewDirection.Down);
                int index = Random.Range(0, directionsList.Count);
                newDirection = directionsList[index];
                directionsList.Clear();
            }
            else
            {
                newDirection = (NewDirection)Random.Range(0, 2);        //Tylko lewo i prawo
            }
            skip = false;
        }
        //Debug.Log(newDirection);
        
    }

    private void FillTheRow(int rowNumber) // Wypełnia cały rząd
    {
        int numberOfRoom = 3;
        int startPos;
        if (numberOfRoomsInPath == 16)
        {
            startPos = Random.Range(0, 2);
            currentPosition = new Vector2(startingPositions[startPos].position.x, startingPositions[startPos].position.y - (10 * rowNumber));
        }
        else
        {
            switch (Math.Round(generatedRoomsOnPath[^1].transform.position.x))
            {
                case (-5):
                    startPos = 0;       //zaczuna od lewej
                    break;
                case 5:
                    startPos = 0;
                    break;
                case 15:
                    startPos = 1;       //zaczyna od prawej
                    break;
                case 25:
                    startPos = 1;
                    break;
                default:
                    startPos = 1;
                    break;
            }
            currentPosition = new Vector2(startingPositions[startPos].position.x, startingPositions[startPos].position.y - (10 * rowNumber));
        }

        for (int i = 0; i < 4; i++)
        {
            if (i == numberOfRoom)
            {
                choosedRoom = GetRandomRoomWithType(RoomType.LRB);        
            }
            else
            {
                choosedRoom = GetRandomRoomWithType(RoomType.LR);
                
            }

            if (generatedRoomsOnPath.Count != 0 && generatedRoomsOnPath[^1].gameObject.GetComponent<Room>().roomType is RoomType.LRB )
            {
                choosedRoom = GetRandomRoomWithType(RoomType.LR);
            }
            
            numberOfRoomsInPath--;
            newRoom = Instantiate(choosedRoom, currentPosition, Quaternion.identity);       //Tworzymy pokój
            generatedRoomsOnPath.Add(newRoom);
            if (startPos == 0)
            {
                currentPosition.x += moveAmount;
            }
            else
            {
                currentPosition.x -= moveAmount;
            }
        }
        currentPosition.y -= moveAmount;
        currentRow++;
        skip = true;
    }

    private void ApplyMovement()
    {
        switch (newDirection)
        {
            case NewDirection.Right:
                if (currentPosition.x < maxX)
                {
                    currentPosition.x += moveAmount;
                    choosedRoom = GetRandomRoomWithType(RoomType.LR);
                    //Debug.Log("right");
                }
                else
                {
                    newDirection = NewDirection.Down; // Jeśli jesteśmy na krawędzi, idziemy w dół
                    SwapRoomType(RoomType.LRB);
                    choosedRoom = GetRandomRoomWithType(RoomType.LR);     //wybieramy losowy pokój
                    currentPosition.y -= moveAmount;
                    currentRow++;
                    //Debug.Log("down");
                }
                break;
            case NewDirection.Left:
                if (currentPosition.x > minX)
                {
                    currentPosition.x -= moveAmount;
                    choosedRoom = GetRandomRoomWithType(RoomType.LR);
                    //Debug.Log("Left");
                }
                else
                {
                    newDirection = NewDirection.Down; // Jeśli jesteśmy na krawędzi, idziemy w dół
                    SwapRoomType(RoomType.LRB);
                    choosedRoom = GetRandomRoomWithType(RoomType.LR);
                    currentPosition.y -= moveAmount;
                    currentRow++;
                    //Debug.Log("Down");
                }
                break;

            case NewDirection.Down:
                if (currentPosition.y > minY)
                {
                    SwapRoomType(RoomType.LRB);
                    choosedRoom = GetRandomRoomWithType(RoomType.LR);
                    currentPosition.y -= moveAmount;
                    currentRow++; // Zwiększamy numer rzędu
                }
                break;
        }
        
    }

    private void SwapRoomType(RoomType finalRoom) //niszczy poprzedni pokój który nie pasuje i tworzy na jego miejscu nowy z odpowiednim typem
    {
        Vector2 lastRoomPos = generatedRoomsOnPath[^1].gameObject.transform.position;
        generatedRoomsOnPath[^1].gameObject.GetComponent<Room>().RoomDestruction();
        generatedRoomsOnPath.RemoveAt(generatedRoomsOnPath.Count - 1);
        newRoom = Instantiate(GetRandomRoomWithType(finalRoom), lastRoomPos, Quaternion.identity);
        generatedRoomsOnPath.Add(newRoom);
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

    public void UpdateHeatMapWithPath()
    {
        Vector2 pos = Vector2.zero;
        float radius = 42;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radius, groundLayer);

        int dotSize = 8; // Rozmiar kropki w pikselach

        foreach (Collider2D collider in colliders)
        {
            Vector2 colliderPosition = collider.transform.position;
            int groundX = (int)(((colliderPosition.x + 10) / 40.0f) * width);
            int groundY = (int)(((colliderPosition.y + 10) / 40.0f) * height);
            groundX = Mathf.Clamp(groundX, 0, width - 1);
            groundY = Mathf.Clamp(groundY, 0, height - 1);

            // Rysowanie większych kropek
            for (int x = -dotSize; x <= dotSize; x++)
            {
                for (int y = -dotSize; y <= dotSize; y++)
                {
                    int drawX = Mathf.Clamp(groundX + x, 0, width - 1);
                    int drawY = Mathf.Clamp(groundY + y, 0, height - 1);
                    heatMapTexture.SetPixel(drawX, drawY, Color.green);
                }
            }
        }

        heatMapTexture.Apply();
    }

}
