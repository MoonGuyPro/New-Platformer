using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask wallAndGroundLayer;
    
    [Header("Wall Collider Variables")]
    [SerializeField] private float rangeWallCollider;
    [SerializeField] private float wallColliderDistance;
    
    [Header("Ground Collider Variables")]
    [SerializeField] private float rangeGroundCollider;
    [SerializeField] private float groundColliderDistance;

    [Header("Enemy")] 
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")] 
    [SerializeField] private float speed;
    
    
    
    
    
    
    private Vector2 boxCastVector2;


    private void Awake()
    {

    }

    private bool WallInSight()
    {
        RaycastHit2D hitWall = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * rangeWallCollider * transform.localScale.x * wallColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeWallCollider, boxCollider.bounds.size.y/2), 0, Vector2.left, 0, wallAndGroundLayer);
        
        RaycastHit2D hitGround = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * rangeGroundCollider * transform.localScale.x * groundColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeGroundCollider, boxCollider.bounds.size.y/2), 0, Vector2.left, 0, wallAndGroundLayer);

        if (hitWall.collider != null || hitGround != null)
        {
            //change direction
        }
            
        
        return hitWall.collider != null || hitGround != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * rangeWallCollider * transform.localScale.x * wallColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeWallCollider, boxCollider.bounds.size.y/2));
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxCollider.bounds.min + transform.right * rangeGroundCollider * transform.localScale.x * groundColliderDistance, 
            new Vector2(boxCollider.bounds.size.x * rangeGroundCollider, boxCollider.bounds.size.y/2));
    }
}
