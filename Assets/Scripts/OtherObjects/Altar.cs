using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public GameObject newSpellPrefab; // Ustaw w Unity, przypisuj�c odpowiedni prefab

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            if (playerCombat != null)
            {
                playerCombat.spellPrefab = newSpellPrefab;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Dodaj odpowiedni� logik�, je�li gracz opu�ci obszar o�tarza
        }
    }
}
