using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private int coinAmount;

    private void Start()
    {
        coinAmount = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coinAmount += 1;
            Debug.Log(coinAmount);
            Destroy(other.gameObject);
        }
    }
}
