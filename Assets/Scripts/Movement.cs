using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = System.Numerics.Quaternion;
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
    [Tooltip("Rate at which stamina decreases when dashing")] [SerializeField]
    private int dash_decreaseValue = 2;

    [Header("BurstStep")] //this should technically be called the dash but that would require changing variables
    [SerializeField] private float burstVelocity = 2f;
    [SerializeField] private float burstDuration = 2f;
    [SerializeField] private float burstCooldownDuration = 2f;
    [SerializeField] private float maxUpwardBurstVelocity = 10f; // the limit to the player's y-velocity during burst step
    [Tooltip("Rate at which stamina decreases when burst stepping")] [SerializeField]
    private int burstStep_decreaseValue = 2;
    private float burstCooldownTimer; // what we'll be counting down from 
    private Vector2 burstDirection;
    private bool isBurstStepping;
    private bool canBurstStep = true;
    private bool burstStepInput;

    [Header("KnockBack")] 
    [SerializeField] private float KBForce = 10f;
    [SerializeField] public float KBDuration = .2f;
    [HideInInspector] public float KBTimer;
    [HideInInspector] public bool KnockFromRight;

    [Header("Stamina")]
    [SerializeField] public int maxStamina = 10;
    [SerializeField] private StaminaBar staminaBar;
    [Tooltip("Rate at which stamina increases")] 
    [SerializeField] private int increaseValue = 2;
    private int currentStamina;
    private bool hasStaminaToUse;
    
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
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
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
        if(DialogueManager.Instance.dialogueIsPlaying || PauseMenu.Instance.isPaused)
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
        trailRenderer.emitting = false;
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

        // if game is paused, prevent player from moving in place
        if (PauseMenu.Instance.isPaused)
        {
            return;
        }
        
        //handle player movement if dialogue is playing
        if (isMoving && !DialogueManager.Instance.dialogueIsPlaying)
        {
            anim.SetBool("isMoving", true);
        }
        else if (!isMoving || DialogueManager.Instance.dialogueIsPlaying)
        {
            anim.SetBool("isMoving", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (!isFacingRight && movementInput.x < 0f && !DialogueManager.Instance.dialogueIsPlaying)
        {
            Flip();
        }
        else if (isFacingRight && movementInput.x > 0f && !DialogueManager.Instance.dialogueIsPlaying)
        {
            Flip();
        }

        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            staminaBar.gameObject.SetActive(false);
        }
        else
        {
            staminaBar.gameObject.SetActive(true);
        }

        //if the player depletes all stamina, pause use of stamina until its refilled to max
        if (currentStamina <= 0 && hasStaminaToUse)
        {
            hasStaminaToUse = false;
            currentStamina += increaseValue;
            staminaBar.IncreaseStamina(increaseValue);
        } else if (!hasStaminaToUse && currentStamina == maxStamina)
        {
            hasStaminaToUse = true;
        }
    }

    private void FixedUpdate()
    {
        //player can't jump or dash if dialogue is playing
        // (player's movements are also stopped elsewhere in the script)
        if (DialogueManager.Instance.dialogueIsPlaying || GameManager.Instance.IsGamePaused)
        {
            return;
        }
        
        //stop emitting trail if player tries to burst step or dash with no stamina
        if (!hasStaminaToUse)
        {
            trailRenderer.emitting = false;
        }
        
        //can control player if they're not being knocked back
        if (KBTimer <= 0)
        {
            //move player left or right
            rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);
            
            Jump();
            BurstStep();
            Dash();
        }
        else // apply knock back (KB)
        {
            //if player is hit from right, apply force towards the left
            if (KnockFromRight)
            {
                //rb.velocity = new Vector2(-KBForce, KBForce);
                
                //set velocity to 45 degrees multiplied by KBForce in the left direction
                rb.velocity = UnityEngine.Quaternion.AngleAxis(45f, Vector2.right) * Vector2.left * KBForce;
                
                //rb.AddForce(new Vector2(-KBForce, KBForce), ForceMode2D.Impulse);
            }
            else // else apply force towards the right
            {
                //rb.velocity = new Vector2(KBForce, KBForce);
                
                //set velocity to 45 degrees multiplied by KBForce in the right direction
                rb.velocity = UnityEngine.Quaternion.AngleAxis(45f, Vector2.right) * Vector2.right * KBForce;
                
                //rb.AddForce(new Vector2(KBForce, KBForce), ForceMode2D.Impulse);
            }
            KBTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        //if player presses run button and is moving, run
        if (dashInput && movementInput != Vector2.zero && hasStaminaToUse)
        {
            //decrease stamina
            currentStamina -= dash_decreaseValue;
            
            //update stamina bar UI
            staminaBar.DecreaseStamina(dash_decreaseValue);
            
            trailRenderer.emitting = true;

            float dashDirection = Mathf.Sign(movementInput.x);
            rb.AddForce(Vector2.right * dashDirection * dashForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -dashForce, dashForce), rb.velocity.y);
        }
        else
        {
            //increase stamina
            currentStamina += increaseValue;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
            }
            
            //update stamina bar UI
            staminaBar.IncreaseStamina(increaseValue);
        }
    }
    
    private void BurstStep()
    {
        if (burstStepInput && canBurstStep && hasStaminaToUse)
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
            //decrease stamina when burst stepping
            currentStamina -= burstStep_decreaseValue;
            //update stamina UI
            staminaBar.DecreaseStamina(burstStep_decreaseValue);
            
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
            //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        jumpInput = false;

    }
    
    public void SetMaxStamina(int value)
    {
        //increase max stamina
        maxStamina += value;
        //update stamina UI so max value is new max stamina
        staminaBar.SetMaxStamina(maxStamina);
        //restore stamina
        currentStamina = maxStamina;
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
