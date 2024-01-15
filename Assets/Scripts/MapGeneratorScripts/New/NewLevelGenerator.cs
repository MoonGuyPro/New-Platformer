using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLevelGenerator : MonoBehaviour
{
    public static NewLevelGenerator Instance;
    [Header("Terrain (Area control) settings")]
    [Range(4,16)] public int numberOfRoomsInPath;

    [Header("Balance settings")] 
    [Range(0,1)] public float safeTilesRatio;

    [Header("Exploration settings")] 
    [Range(1, 3)] public int fireAltarAmount;
    [Range(1, 3)] public int iceAltarAmount;
    [Range(1, 5)] public int healingPointsAmount;
    [Range(1, 30)] public int coinsAmount;

}
