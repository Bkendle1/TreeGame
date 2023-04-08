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
    
    private Rigidbody2D rb;
    private bool isGrounded = true;

    //Input Actions
    private PlayerControls controls;
    private Vector2 movementInput;
    private bool jumpInput;
    private bool dashInput;


    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
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
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
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
    
    private void FixedUpdate()
    {
        //check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //move player left or right
        rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);

        //if the player jumps off the ground, jump
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (dashInput)
        {
            float dashDirection = Mathf.Sign(movementInput.x);
            rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -dashForce, dashForce), rb.velocity.y);
        }
        else
        {
            dashInput = false;
        }

        if (rb.velocity.y > jumpForce)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        jumpInput = false;
    }
}
