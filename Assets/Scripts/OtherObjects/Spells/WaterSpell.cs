using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpell : Spell
{
    public float waterSpeed;
    public float waterDamage;
    protected override void Start()
    {
        speed = waterSpeed;
        damage = waterDamage;

        base.Start();
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
    }
}
