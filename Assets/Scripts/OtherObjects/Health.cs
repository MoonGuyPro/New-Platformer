using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    [SerializeField] private float startingHealth;
    [SerializeField] private Behaviour[] components;
    private Animator animator;
    private bool isDead;
    

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            if (!isDead)
            {
                animator.SetTrigger("Die");
            
                foreach (Behaviour comp in components)
                    comp.enabled = false;
                //killed
                isDead = true;
            }

        }
    }
    
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    
    private void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
    }
}
