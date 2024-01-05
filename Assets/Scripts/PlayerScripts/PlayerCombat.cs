using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    public GameObject spellPrefab;
    public Animator animator;
    public PlayerMovement playerMovement;
    public Transform magicWandPosition;

    [SerializeField] private int damage;
    public float timeBetweenSpells;
    public bool isCastingSpell = false;
    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }


    void Update()
    {
        if (Input.GetButtonDown("SpellCast"))
        {
            if (!isCastingSpell && !playerMovement.isJumping)
            {
                StartCoroutine(SpellCastControler());
            }
            
        }

    }

    private IEnumerator SpellCastControler()
    {
        isCastingSpell = true;

        animator.SetBool("IsAttacking", true);
        
        yield return new WaitForSeconds(timeBetweenSpells);
        CastSpell();

        isCastingSpell = false;
        animator.SetBool("IsAttacking", false);
    }

    private float PadControll()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
        {
            // Oblicz k�t w stopniach
            float angle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg;
            return angle;
        }
        else
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

    private void CastSpell()
    {
        float angle = PadControll();
        Spell spellComponent = spellPrefab.GetComponent<Spell>();
        spellComponent.casterTag = "Player";
        spellComponent.enemyTag = "Enemy";
        spellComponent.damage = damage;
        
        // Sprawd�, czy obiekt ma komponent Spell
        if (spellComponent != null)
        {
            // Przypisz warto�� do zmiennej angle w skrypcie Spell
            spellComponent.angle = angle;
        }
        else
        {
            Debug.LogError("Obiekt spellPrefab nie zawiera komponentu Spell.");
        }
        Instantiate(spellPrefab, magicWandPosition.position , Quaternion.identity);
    }

    private void PlayerDead()
    {
        uiManager.GameOver();
        Time.timeScale = 0f;
    }
}
