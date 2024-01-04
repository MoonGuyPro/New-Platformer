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
    

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("Die");
            
            foreach (Behaviour comp in components)
                comp.enabled = false;
            
            //killed
        }
    }

    public void DestroyObject()
    {
        DestroyObject(gameObject);
    }
    
    private void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
    }
}
