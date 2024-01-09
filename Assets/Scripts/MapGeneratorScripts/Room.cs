using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int type;
    public NewLevelGeneratorTerrain.RoomType roomType;
    public int roomNumber;

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }

}
