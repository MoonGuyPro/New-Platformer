using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public GameObject newSpellPrefab; // Ustaw w Unity, przypisuj¹c odpowiedni prefab

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
            // Dodaj odpowiedni¹ logikê, jeœli gracz opuœci obszar o³tarza
        }
    }
}
