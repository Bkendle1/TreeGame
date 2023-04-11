using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementForces : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = .2f;

    
    
    private Vector2 position;
    private Vector2 velocity;
    private bool isGrounded = true;

    private PlayerControls controls;
    private Vector2 movementInput;
    private bool jumpInput;
    private bool dashInput;
    
    
    void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Run.performed += OnDashPerfomed;
        controls.Enable();

    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Run.performed -= OnDashPerfomed;
        controls.Disable();
    }

    private void FixedUpdate()
    {
        //gravity applied to player
        velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

        //check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        velocity.x = movementInput.x * moveSpeed;

        if (jumpInput && isGrounded)
        {
            velocity.y = jumpForce;
        }

        if (dashInput)
        {

            //apply force in the direction player is facing
            float dashDirection = Mathf.Sign(movementInput.x);
            velocity.x = dashDirection * dashForce;

            velocity.x = Mathf.Clamp(velocity.x, -dashForce, dashForce);
        }

        //update position based on velocity
        position = transform.position;
        position += velocity * Time.fixedDeltaTime;

        transform.position = position;
        velocity = new Vector2(Mathf.Clamp(velocity.x, -moveSpeed, moveSpeed),
            Mathf.Clamp(velocity.y, -Mathf.Infinity, jumpForce));

        //Reset input flags
        jumpInput = false;
        dashInput = false;
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
    private void OnDashPerfomed(InputAction.CallbackContext context)
    {
        dashInput = true;
    }
    
}
