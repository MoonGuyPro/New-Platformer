using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Collider variables")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [Header("Attack variables")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private float range;

    [Header("Ranged Attack")] 
    [SerializeField] private Transform firepoint;
    [SerializeField] private GameObject spellPrefab;
    
    
    [SerializeField] private LayerMask playerLayer;
    
    private float cooldownTimer = Mathf.Infinity;
    
    //References
    private Animator animator;
    private EnemyPatrol enemyPatrol;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPatrol = GetComponent<EnemyPatrol>();
    }
    
    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("RangeAttack");
            }
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }
    
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
            new Vector2(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y), 0, Vector2.left, 0, playerLayer);

        
        return hit.collider != null;
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
            new Vector2(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y));
    }*/

    private void RangedAttack()
    {
        float angle = SpellAngle();
        Spell spellComponent = spellPrefab.GetComponent<Spell>();
        spellComponent.casterTag = "Enemy";
        spellComponent.enemyTag = "Player";
        spellComponent.damage = damage;
        

        // Sprawdz, czy obiekt ma komponent Spell
        if (spellComponent != null)
        {
            // Przypisz wartosc do zmiennej angle w skrypcie Spell
            spellComponent.angle = angle;
        }
        else
        {
            Debug.LogError("Obiekt spellPrefab nie zawiera komponentu Spell.");
        }
        Instantiate(spellPrefab, firepoint.position , Quaternion.identity);

    }

    private float SpellAngle()
    {
        if (transform.localScale.x > 0)
        {
            return 0;
        }
        else
        {
            return 180;
        }
    }
}
