using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenSpell : Spell
{
    [SerializeField] [Range(0f, 1f)] private float slowFactor;
    [SerializeField] private float slowDuration;
    
    public Animator animator;
    
    private EnemyPatrol enemyPatrol;
    private bool applySlow;
    private SpriteRenderer enemySpriteRenderer;
    
    protected override void Awake()
    {
        base.Awake();
        applySlow = false;
        damage = 1;
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
            enemySpriteRenderer = other.GetComponent<SpriteRenderer>();
            if (enemySpriteRenderer != null)
            {
                enemySpriteRenderer.color = new Color(0f, 0.952f, 0.839f, 1f); // Ustawienie koloru cyjanowego
                Debug.Log(enemySpriteRenderer.color);
            }
            Debug.Log(enemySpriteRenderer.color);

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
        if (enemySpriteRenderer != null)
        {
            enemySpriteRenderer.color = new Color(0f, 0.952f, 0.839f, 1f); // Ustawienie koloru cyjanowego
        }
        yield return new WaitForSeconds(slowDuration); // Czas trwania spowolnienia
        if (enemySpriteRenderer != null)
        {
            enemySpriteRenderer.color = Color.white; // Przywrócenie oryginalnego koloru
        }
        enemyPatrol.speed = originalSpeed; // Przywrócenie oryginalnej prędkości
    }
}
