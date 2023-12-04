using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : Spell
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            Destroy(gameObject);
    }
    
}
