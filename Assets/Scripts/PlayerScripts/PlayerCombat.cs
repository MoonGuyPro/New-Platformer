using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject spellPrefab;
    public float timeBetweenSpells;
    public Animator animator;
    public PlayerMovement playerMovement;

    public bool isCastingSpell = false;
    public float spellForce = 10f;

    void Update()
    {
        if (Input.GetButtonDown("SpellCast"))
        {
            if (!isCastingSpell && !playerMovement.isJumping)
            {
                StartCoroutine(SpellCastControler());
            }
            
        }
        
    }

    private IEnumerator SpellCastControler()
    {
        isCastingSpell = true;

        animator.SetBool("IsAttacking", true);
        
        yield return new WaitForSeconds(timeBetweenSpells);
        CastSpell();

        isCastingSpell = false;
        animator.SetBool("IsAttacking", false);
    }

    private float PadControll()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            // Oblicz k¹t w stopniach
            float angle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;
            return angle;
        }
        else
        {
            return 0;
        }
    }

    private void CastSpell()
    {
        float angle = PadControll();
        Spell spellComponent = spellPrefab.GetComponent<Spell>();

        // SprawdŸ, czy obiekt ma komponent Spell
        if (spellComponent != null)
        {
            // Przypisz wartoœæ do zmiennej angle w skrypcie Spell
            spellComponent.angle = angle;
        }
        else
        {
            Debug.LogError("Obiekt spellPrefab nie zawiera komponentu Spell.");
        }
        Instantiate(spellPrefab, transform.position, Quaternion.identity);
    }
}
