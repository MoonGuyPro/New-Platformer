using System;
using Cinemachine;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    void Start() // Można użyć również metody Awake, w zależności od potrzeb.
    {
        // Znajdź obiekt 'Confiner' z PolygonCollider2D w scenie.
        PolygonCollider2D confinerCollider = GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();

        if (confinerCollider != null)
        {
            // Znajdź komponent CinemachineConfiner na tym samym obiekcie co skrypt.
            CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

            // Jeśli nie znaleziono na tym obiekcie, spróbuj znaleźć w dzieciach (jeśli kamera jest dzieckiem gracza).
            if (confiner == null)
            {
                confiner = GetComponentInChildren<CinemachineConfiner>();
            }

            if (confiner != null)
            {
                // Przypisz PolygonCollider2D do CinemachineConfiner
                confiner.m_BoundingShape2D = confinerCollider;

                // Wywołaj InvalidatePathCache(), jeśli jest taka potrzeba.
                confiner.InvalidatePathCache();
            }
            else
            {
                Debug.LogError("Komponent CinemachineConfiner nie został znaleziony na obiekcie gracza.");
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu 'Confiner' z komponentem PolygonCollider2D w scenie.");
        }
    }


}