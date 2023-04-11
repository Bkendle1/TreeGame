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
    [SerializeField] private float coyoteTime = 1f;

    private float coyoteTimeCounter;
    private bool jumpInput;
    private bool dashInput;
    private bool isFacingRight;
    private bool hasInteracted = false;
    private bool isMoving;
    
    private Rigidbody2D rb;
    private Animator anim;
    
    //Input Actions
    private PlayerControls controls;
    private Vector2 movementInput;

    //Singleton Setup
    //allow classes to reference but not alter
    public static Movement Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
        
        controls.Player.Run.performed += OnDashPerformed;
        controls.Player.Run.canceled += OnDashCanceled;

        controls.Player.Interaction.performed += OnInteracted;
        
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;

        controls.Player.Run.performed -= OnDashPerformed;
        controls.Player.Run.canceled -= OnDashCanceled;

        controls.Player.Interaction.performed -= OnInteracted;

        controls.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        isMoving = true;
        
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
        isMoving = false;
        
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpInput = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        coyoteTimeCounter = 0f;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashInput = true;
    }
    private void OnDashCanceled(InputAction.CallbackContext context)
    {
        dashInput = false;
    }

    private void OnInteracted(InputAction.CallbackContext context)
    {
        hasInteracted = true;
    }

    public bool GetInteractedPressed()
    {
        bool result = hasInteracted;
        hasInteracted = false;
        return result;
    }
    private void Update()
    {
        anim.SetBool("isGrounded", isGrounded());

        //handle player movement if dialogue is playing
        if (isMoving && !DialogueManager.Instance.dialogueIsPlaying)
        {
            anim.SetBool("isMoving", true);
        } else if (DialogueManager.Instance.dialogueIsPlaying)
        {
            anim.SetBool("isMoving", false);
            rb.velocity = new Vector2(0,rb.velocity.y);
        }

        if (!isFacingRight && movementInput.x < 0f && !DialogueManager.Instance.dialogueIsPlaying)
        {
            Flip();
        } else if (isFacingRight && movementInput.x > 0f && !DialogueManager.Instance.dialogueIsPlaying)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        //player can't jump or dash if dialogue is playing
        // (player's movements are also stopped elsewhere in the script)
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }

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
        //Coyote Timer
        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        //if the player jumps off the ground, jump
        if (jumpInput && coyoteTimeCounter > 0f)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }
}
