using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenSpell : Spell
{
    [SerializeField] [Range(0f, 1f)] private float slowFactor;
    [SerializeField] private float slowDuration;
    private float enemySpeed;
    public Animator animator;
    protected override void Awake()
    {
        base.Awake();
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
            EnemyPatrol enemyPatrol = other.GetComponent<EnemyPatrol>();
            enemySpeed = enemyPatrol.speed;
            ApplySlow(slowFactor, slowDuration);
            enemyPatrol.speed = enemySpeed;
            hit = true;
        }
        
        if (!other.CompareTag(casterTag))
        {
            animator.SetBool("explode", true);
            base.StopMoving();
        }
    }
    
    private void ApplySlow(float slowFactor, float duration)
    {
        StartCoroutine(SlowDown(slowFactor, duration));
    }
    
    private IEnumerator SlowDown(float slowFactor, float duration)
    {
        float originalSpeed = enemySpeed;
        enemySpeed *= slowFactor; // Zmniejszenie prędkości

        yield return new WaitForSeconds(duration); // Czas trwania spowolnienia

        enemySpeed = originalSpeed; // Przywrócenie oryginalnej prędkości
    }
}
