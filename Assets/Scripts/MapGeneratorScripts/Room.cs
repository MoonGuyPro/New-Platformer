using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //public int type;
    public NewLevelGeneratorTerrain.RoomType roomType;
    //public int roomNumber;
    public LayerMask layerToCheck;

    public List<GameObject> FindGroundInRoom()       //Znajduje obiekty terenu w pokoju i dodaje do listy
    {
        Vector2 center = transform.position;

        List<GameObject> groundTilesInRoom = new List<GameObject>();

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(9,9), 0, layerToCheck);
        
        foreach (Collider2D hit in hits)
        {
            // Sprawdź, czy trafienie to nie ten sam obiekt
            if (hit.gameObject != gameObject)
            {
                groundTilesInRoom.Add(hit.gameObject);
            }
        }

        if (groundTilesInRoom.Count == 0)
        {
            Debug.Log("pusty pokoj" + transform.position);
        }
            
        // Znaleźliśmy obiekty ziemi, teraz szukamy wolnych miejsc
        return groundTilesInRoom;
    }

/*    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = transform.position;
        Gizmos.DrawWireCube(center, new Vector2(9,9));
    }*/

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }

}
