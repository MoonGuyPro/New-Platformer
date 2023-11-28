using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public PlayerCombat playerCombat;

    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    public bool isJumping = false;



    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        
        if (Input.GetButtonDown("Jump") && !playerCombat.isCastingSpell)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, isJumping);
        

    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        isJumping = false;
    }
}
