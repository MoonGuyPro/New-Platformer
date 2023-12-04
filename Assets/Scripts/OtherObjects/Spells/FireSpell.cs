using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : Spell
{
    public float fireSpeed;
    public float fireDamage;
    protected override void Start()
    {
        speed = fireSpeed;
        damage = fireDamage;

        base.Start();
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
    }
}
