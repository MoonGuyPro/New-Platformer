using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldControler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    
    public void UpdateGoldUi(int goldNumber)
    {
        goldText.text = goldNumber.ToString();
    }
}
