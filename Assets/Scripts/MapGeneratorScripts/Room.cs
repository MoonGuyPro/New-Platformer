using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int type;
    public NewLevelGeneratorTerrain.RoomType roomType;
    public int numberOfRoomFromLevelGenerator;

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }

}
