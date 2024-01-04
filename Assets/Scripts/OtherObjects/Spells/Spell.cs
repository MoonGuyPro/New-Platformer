using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class Spell : MonoBehaviour
{
    public float speed;
    public float damage;
    public string enemyTag;
    public string casterTag;
    public bool hit;
    [HideInInspector]
    public float angle;


    private Rigidbody2D _rigidbody;

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        hit = false;
    }


    protected virtual void FixedUpdate()
    {
        // Oblicz skladowe wektora sily na podstawie kata
        float launchAngleRad = angle * Mathf.Deg2Rad;
        float forceX = Mathf.Cos(launchAngleRad);
        float forceY = Mathf.Sin(launchAngleRad);
        Vector2 direction = new Vector2(forceX, forceY);

        _rigidbody.velocity = direction * speed;
        
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }

    protected virtual void StopMoving()
    {
        speed = 0;
    }
    
    protected virtual void Deactivate()
    {
        Destroy(gameObject);
    }

}
