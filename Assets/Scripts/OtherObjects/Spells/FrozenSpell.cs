using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenSpell : Spell
{
    [SerializeField] [Range(0f, 1f)] private float slowFactor;
    [SerializeField] private float slowDuration;
    
    public Animator animator;
    
    private EnemyPatrol enemyPatrol;
    private SpriteRenderer enemySpriteRenderer;
    private bool applySlow;
    
    protected override void Awake()
    {
        base.Awake();
        applySlow = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (applySlow)
        {
            StartCoroutine(SlowDown());
            applySlow = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) && !hit)
        {
            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.TakeDamage(damage);
            enemyPatrol = other.GetComponent<EnemyPatrol>();
            if (other.GetComponent<SpriteRenderer>() != null)
            {
                //enemySpriteRenderer = other.GetComponent<SpriteRenderer>();
            }

            applySlow = true;
            hit = true;
        }
        
        if (!other.CompareTag(casterTag))
        {
            animator.SetBool("explode", true);
            base.StopMoving();
        }
    }

    private IEnumerator SlowDown()
    {
        float originalSpeed = enemyPatrol.speed;
        enemyPatrol.speed *= slowFactor; // Zmniejszenie prędkości
        //enemySpriteRenderer.color = new Color(0, 1, 1, 1);
        yield return new WaitForSeconds(slowDuration); // Czas trwania spowolnienia
        //enemySpriteRenderer.color = Color.white;
        enemyPatrol.speed = originalSpeed; // Przywrócenie oryginalnej prędkości
    }
}
