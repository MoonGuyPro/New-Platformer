using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private GameObject goldText;
    public int coinAmount;
    private GoldControler goldControler;

    private void Start()
    {
        coinAmount = 0;
        goldControler = goldText.GetComponent<GoldControler>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coinAmount += 1;
            Debug.Log(coinAmount);
            Destroy(other.gameObject);
            goldControler.UpdateGoldUi(coinAmount);
            
        }
    }
}
