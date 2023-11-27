using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float timeBetweenSpells;
    public Animator animator;

    public bool isCastingSpell = false;

    void Update()
    {
        if (Input.GetButtonDown("SpellCast"))
        {
            if (!isCastingSpell)
            {
                StartCoroutine(SpellCast());
            }
            
        }
    }

    private IEnumerator SpellCast()
    {
        isCastingSpell = true;

        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(timeBetweenSpells);

        isCastingSpell = false;
        animator.SetBool("IsAttacking", false);

    }
}
