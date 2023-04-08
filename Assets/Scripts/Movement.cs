using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private LayerMask groundLayer;
    
    private bool jumpInput;
    private bool dashInput;
    private bool isFacingRight;
    
    private Rigidbody2D rb;
    private Animator anim;
    
    //Input Actions
    private PlayerControls controls;
    private Vector2 movementInput;


    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Run.performed += OnDashPerformed;
        controls.Player.Run.canceled += OnDashCanceled;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Run.performed += OnDashPerformed;
        controls.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        anim.SetBool("isMoving", true);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
        anim.SetBool("isMoving", false);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpInput = true;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashInput = true;
    }
    private void OnDashCanceled(InputAction.CallbackContext context)
    {
        dashInput = false;
    }

    private void Update()
    {
        anim.SetBool("isGrounded", isGrounded());
        if (!isFacingRight && movementInput.x < 0f)
        {
            Flip();
        } else if (isFacingRight && movementInput.x > 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        
        //move player left or right
        rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);

        Jump();
        Dash();
    }

    private void Dash()
    {
        //if player presses run button and is moving, run
        if (dashInput && movementInput != Vector2.zero)
        {
            float dashDirection = Mathf.Sign(movementInput.x);
            rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -dashForce, dashForce), rb.velocity.y);
        }
    }
    
    private void Jump()
    {
        //if the player jumps off the ground, jump
        if (jumpInput && isGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        jumpInput = false;
    }
    
    private bool isGrounded()
    {
        //check if player is on the ground
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
