using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public GameObject newSpellPrefab;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            if (playerCombat != null)
            {
                // Zmiana zmiennej w skrypcie gracza
                playerCombat.spellPrefab = newSpellPrefab;
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // Dodaj odpowiedni¹ logikê, jeœli gracz opuœci obszar o³tarza
    }
}
