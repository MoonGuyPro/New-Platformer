using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Spell : MonoBehaviour
{
    public float speed;
    public float damage;
    public float angle;

    private new Rigidbody2D rigidbody;

    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }


    protected virtual void FixedUpdate()
    {
        // Oblicz sk³adowe wektora si³y na podstawie k¹ta
        float launchAngleRad = angle * Mathf.Deg2Rad;
        float forceX = Mathf.Cos(launchAngleRad);
        float forceY = Mathf.Sin(launchAngleRad);
        Vector2 direction = new Vector2(forceX, forceY);

        rigidbody.velocity = direction * speed;
    }
}
