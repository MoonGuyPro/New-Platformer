using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    float speed { get; set; }
    float damage { get; set; }
    float angle { get; set; }

}


[RequireComponent(typeof(Rigidbody2D))]
public class Spell : MonoBehaviour, ISpell
{
    public float speed { get; set; }
    public float damage { get; set; }
    public float angle { get; set; }

    private new Rigidbody2D rigidbody;

    // Metoda Start oznaczona jako protected
    // Start is called before the first frame update
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
