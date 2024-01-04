using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : Spell
{
    public Animator animator;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) && !hit)
        {
            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.TakeDamage(damage);
            Debug.Log(damage);
            hit = true;
        }
        
        if (!other.CompareTag(casterTag))
        {
            animator.SetBool("explode", true);
            base.StopMoving();
        }

    }

}
