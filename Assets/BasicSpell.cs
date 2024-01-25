using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpell : Spell
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) && !hit)
        {
            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.TakeDamage(damage);

            hit = true;
        }
        
        if (!other.CompareTag(casterTag))
        {
            base.StopMoving();
            base.Deactivate();
        }
    }
}
