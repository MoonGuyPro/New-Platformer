using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float currentHealth;
    [SerializeField] private float startingHealth;
    [SerializeField] private Behaviour[] components;

    [Header("iFrames")] 
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    
    private Animator animator;
    private bool isDead;
    

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Invunerability());
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
    
    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(6, 10, true);
        //duration
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0,0.5f);
            yield return new WaitForSeconds(iFramesDuration/(numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration/(numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(6, 10 , false);
    }
}
