using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    private bool isMoving;
    private bool isFacingRight;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 1f;
    private float coyoteTimeCounter;
    private bool jumpInput;
    
    [Header("Dashing")]
    [SerializeField] private float dashForce = 10f;
    private bool dashInput;

    [Header("BurstStep")] //this should technically be called the dash but that would require changing variables
    [SerializeField] private float burstVelocity = 2f;
    [SerializeField] private float burstDuration = 2f;
    [SerializeField] private float burstCooldownDuration = 2f;
    [SerializeField] private float maxUpwardBurstVelocity = 10f; // the limit to the player's y-velocity during burst step
    private float burstCooldownTimer; // what we'll be counting down from 
    private Vector2 burstDirection;
    private bool isBurstStepping;
    private bool canBurstStep = true;
    private bool burstStepInput;

    [Header("KnockBack")] 
    [SerializeField] private float KBForce = 10f;
    [SerializeField] private float KBDuration = .2f;
    
    [Header("Cinemachine")] 
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    
    [SerializeField] private BoxCollider2D ColliderBlocker;
    
    private bool hasInteracted = false;
    private bool submitPressed;
    
    private Rigidbody2D rb;
    private Animator anim;
    private TrailRenderer trailRenderer;
    
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
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        //Ignore collisions this gameObject's box collider and child box collider (CollisionBlocker) that has 
        //a kinematic rigid body preventing the player from pushing enemies and vice versa
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), ColliderBlocker, true);
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
        
        controls.Player.Run.performed += OnDashPerformed;
        controls.Player.Run.canceled += OnDashCanceled;

        controls.Player.BurstStep.performed += OnBurstStepPerformed;
        controls.Player.BurstStep.canceled += OnBurstStepCanceled;
        
        
        controls.Player.Interaction.performed += OnInteracted;
        
        //I know this is script is supposed to handle player movements 
        //and that this should be in an input manager, but if this is 
        //the only non-player input we're using, might as well put it here
        controls.UI.Submit.performed += OnSubmitPressed;
        
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

        controls.Player.BurstStep.performed -= OnBurstStepPerformed;
        controls.Player.BurstStep.canceled -= OnBurstStepCanceled;

        
        
        controls.Player.Interaction.performed -= OnInteracted;
                
        controls.UI.Submit.performed -= OnSubmitPressed;

        controls.Disable();
    }

    #region PlayerInputs

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
        //disable jump input so player doesn't jump after dialogue finishes
        if(DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }
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

    private void OnBurstStepPerformed(InputAction.CallbackContext context)
    {
        burstStepInput = true;
    }

    private void OnBurstStepCanceled(InputAction.CallbackContext context)
    {
        burstStepInput = false;
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
    
    #endregion

    #region UI Inputs
    private void OnSubmitPressed(InputAction.CallbackContext context)
    {
        submitPressed = true;
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }
    public void RegisterSubmitPressed() 
    {
        submitPressed = false;
    }
   
    #endregion
    private void Update()
    {
        anim.SetBool("isGrounded", isGrounded());
        anim.SetBool("isBurstStepping", isBurstStepping);

        //handle player movement if dialogue is playing
        if (isMoving && !DialogueManager.Instance.dialogueIsPlaying)
        {
            anim.SetBool("isMoving", true);
        } else if (!isMoving || DialogueManager.Instance.dialogueIsPlaying)
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
        
        Dash();
        Jump();
        BurstStep();
    }

    private void Dash()
    {
        //if player presses run button and is moving, run
        if (dashInput && movementInput != Vector2.zero)
        {
            float dashDirection = Mathf.Sign(movementInput.x);
            rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -dashForce, dashForce), rb.velocity.y);
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }

    private void BurstStep()
    {
        if (burstStepInput && canBurstStep)
        {
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            Flip();
            //setup cooldown timer
            burstCooldownTimer = burstCooldownDuration;
            
            isBurstStepping = true;
            canBurstStep = false;
            burstDirection = movementInput; //direction of burst will be based off of the player's  current movement direction
            
            //get burst direction based on where they're facing if they're not moving
            if (burstDirection == Vector2.zero)
            {
                burstDirection = new Vector2(-transform.localScale.x, 0);
            }
            //Add stopping dash
            StartCoroutine(StopBurstStepping());
        }


        if (isBurstStepping)
        {
            rb.velocity = burstDirection.normalized * burstVelocity;
            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Clamp(rb.velocity.y, -maxUpwardBurstVelocity, maxUpwardBurstVelocity));
            trailRenderer.emitting = true;
            //rb.AddForce(burstDirection.normalized * burstVelocity, ForceMode2D.Impulse);
            
            //disable collisions between player layer (gameObject.layer) and objects in enemy layer (7)
            Physics2D.IgnoreLayerCollision(gameObject.layer, 7,true);
            Physics2D.IgnoreLayerCollision(gameObject.layer, 9,true);

            return;
        }
        else
        {
            //decrement burst cooldown timer after burst step is complete
            burstCooldownTimer -= Time.fixedDeltaTime;
        }

        if (burstCooldownTimer <= 0)
        {
            canBurstStep = true;
        }
        else
        {
            canBurstStep = false;
        }
    }

    private IEnumerator StopBurstStepping()
    {
        yield return new WaitForSeconds(burstDuration);
        isBurstStepping = false;
        trailRenderer.emitting = false;
        
        //re-enable collisions between player layer (gameObject.layer) and objects in enemy layer (7)
        Physics2D.IgnoreLayerCollision(gameObject.layer, 7,false);
        Physics2D.IgnoreLayerCollision(gameObject.layer, 9,false);

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
        //prevent player from flipping sprites if they're burst stepping
        if (!isBurstStepping)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
        
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }
}
