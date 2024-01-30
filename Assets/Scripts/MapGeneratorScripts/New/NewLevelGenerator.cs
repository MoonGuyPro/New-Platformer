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
    [Range(0, 3)] public int fireAltarAmount;
    [Range(0, 3)] public int iceAltarAmount;
    [Range(0, 5)] public int healingPointsAmount;
    [Range(0, 30)] public int coinsAmount;

}
