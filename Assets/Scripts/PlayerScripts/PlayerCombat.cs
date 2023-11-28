using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject spellPrefab;
    public float timeBetweenSpells;
    public Animator animator;

    public bool isCastingSpell = false;

    private float angle;


    void Update()
    {
        if (Input.GetButtonDown("SpellCast"))
        {
            if (!isCastingSpell)
            {
                StartCoroutine(SpellCastControler());
            }
            
        }
        
    }

    private IEnumerator SpellCastControler()
    {
        isCastingSpell = true;

        animator.SetBool("IsAttacking", true);
        if (angle != 0)
        {
            CastSpell();
        }

        yield return new WaitForSeconds(timeBetweenSpells);
        

        isCastingSpell = false;
        animator.SetBool("IsAttacking", false);
    }

    private float PadControll()
    {
        // Odczytaj wej�cie z dr��ka pada
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Sprawd�, czy dr��ek pada ma jakiekolwiek wej�cie
        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            // Oblicz k�t na podstawie wej�cia z dr��ka pada
            angle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;

            return angle;
        }
        else
        {
            return 0;
        }
    }

    private void CastSpell()
    {
        angle = PadControll();

        // Utw�rz instancj� obiektu kuli 2D
        GameObject spell = Instantiate(spellPrefab, transform.position, Quaternion.identity);

        // Ustaw rotacj� kuli zgodnie z kierunkiem
        spell.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Dodaj si�� do kuli, aby lecia�a w okre�lonym kierunku
        Rigidbody2D spellRb = spell.GetComponent<Rigidbody2D>();
        spellRb.AddForce(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), ForceMode2D.Impulse);
    }
}
