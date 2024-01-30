using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask wallAndGroundLayer;
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Wall Collider Variables")]
    [SerializeField] private float rangeWallCollider;
    [SerializeField] private float wallColliderDistance;
    
    [Header("Ground Collider Variables")]
    [SerializeField] private float rangeGroundCollider;
    [SerializeField] private float groundColliderDistance;

    [Header("Movement parameters")] 
    public float speed;

    [Header("Idle Behaviour")] 
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header(("Enemy Animator"))] 
    [SerializeField] private Animator anim;

    private int direction;
    private Collider2D playerCollider;


    private void Start()
    {
        direction = 1;
        playerCollider = new Collider2D();
    }
    
    private void OnDisable()
    {
        anim.SetBool("Moving", false);
    }


    private void Update()
    {
        if (DetectPlayer() && !WallInSight())
        {
            FollowPlayer();
            MoveInDirection();
        }
        else if (!WallInSight())
        {
            MoveInDirection();
        }
        else if (idleTimer > idleDuration)
        {
            direction *= -1;
            MoveInDirection();
        }
    }
    
    private bool DetectPlayer()
    {
        float horizontalDetectionRange = 3f; // Możesz dostosować ten zakres
        float verticalDetectionRange = 2f; // Mniejszy zakres pionowy

        playerCollider = Physics2D.OverlapBox(
            transform.position, 
            new Vector2(horizontalDetectionRange, verticalDetectionRange), 
            0, 
            playerLayer);

        if (playerCollider != null)
        {
            return true; // Gracz wykryty
        }

        return false; // Gracz nie został wykryty
    }

    private void FollowPlayer()
    {
        Vector3 playerDirection = playerCollider.transform.position - transform.position;
        if (playerDirection.x < 0)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        if (WallInSight())
        {
            idleTimer = 0; // Resetuje licznik bezczynności
        }
    }


    private void MoveInDirection()
    {
        idleTimer = 0;
        anim.SetBool("Moving", true);
        
        //Change enemy face direction
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
        
        //Move enemy in direction
        transform.position = new Vector3(transform.position.x + Time.deltaTime * direction * speed,
            transform.position.y, transform.position.z);
    }

    private bool WallInSight()
    {
        RaycastHit2D hitWall = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * rangeWallCollider * transform.localScale.x * wallColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeWallCollider, boxCollider.bounds.size.y/2), 0, Vector2.left, 0, wallAndGroundLayer);
        
        Vector3 wektor = boxCollider.bounds.center + transform.right * rangeGroundCollider * transform.localScale.x * groundColliderDistance + new Vector3(0, -0.5f, 0);
        
        RaycastHit2D hitGround = Physics2D.BoxCast(wektor, 
            new Vector2(boxCollider.bounds.size.x * rangeGroundCollider, boxCollider.bounds.size.y/2), 0, Vector2.left, 0, wallAndGroundLayer);

        if (hitWall.collider != null || hitGround.collider == null)
        {
            anim.SetBool("Moving", false);
            idleTimer += Time.deltaTime;
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * rangeWallCollider * transform.localScale.x * wallColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeWallCollider, boxCollider.bounds.size.y/2));
        
        Gizmos.color = Color.blue;
        Vector3 wektor = boxCollider.bounds.center + transform.right * rangeGroundCollider * transform.localScale.x * groundColliderDistance + new Vector3(0, -0.5f, 0);
        Gizmos.DrawWireCube(wektor, 
            new Vector2(boxCollider.bounds.size.x * rangeGroundCollider, boxCollider.bounds.size.y/2));
    }
    */
    
}
