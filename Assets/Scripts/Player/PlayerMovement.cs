using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private float maxSpeed = 7.5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;

    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] GameObject umbrella;

    private Vector2 moveInput;
    private Vector2 laneInput;
    private Vector2 climbInput;

    private bool canUmbrella;

    private float currentSpeed = 0f;
    private float smoothTime = 0.1f; 
    private float velocitySmoothing = 0f;

    private Spirit[] spirits;
    private Spirit[] strongSpirits;
    private WalkerSpirit[] walkerSpirits; 

    private bool isFacingRight;
    private bool isDead;

    public GameObject singArea;
    public GameObject singAreaVisual;
    private Collider2D singAreaCollider;

    private bool canSing = true;
    private bool isSinging = false;

    public Image staminaBar;
    private float maxStamina = 100f;
    private float currentStamina;
    private float decreaseRate = 50f;
    private float increaseRate = 30f;
    private bool isReloading = false;
    private bool isHoldingSpirit = false;

    public GameObject umbrellaThrow;
    public GameObject umbrellaThrowVisual;
    private Collider2D umbrellaThrowCollider;
    private bool isUsingUmbrellaAsThrow = false;

    private Animator animator;

    private bool isInLanes = false;

    private float[] lanePositions = { -32f, -27f, -22f };
    private int currentLane = 0;
    public float transitionSpeed = 10f;

    private bool canShotUmbrella = true;
    private bool isUmbrellaOpen = false;

    private bool isInDialogue = false;

    public Umbrella umbrellaScript;
    private PlayerHealth playerHealth;

    private bool inWind = false;

    private float flipSpeed = 25f; 
    private float targetScaleX;
    private bool isFlipping = false;

    public ParticleSystem runParticles;
    public ParticleSystem jumpParticles;
    private TrailRenderer flyTrail;

    private bool isClimbing;
    private bool canClimb;
    private bool jumpCancelled = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        singAreaCollider = singArea.GetComponent<Collider2D>();
        umbrellaThrowCollider = umbrellaThrow.GetComponent<Collider2D>();
        playerHealth = GetComponent<PlayerHealth>();
        umbrella.SetActive(false);
        canUmbrella = false;
        isFacingRight = true;
        isDead = false;

        singAreaVisual.SetActive(false);
        singAreaCollider.enabled = false;

        umbrellaThrowVisual.SetActive(false);
        umbrellaThrowCollider.enabled = false;

        currentStamina = maxStamina;

        GameObject[] spiritObjects = GameObject.FindGameObjectsWithTag("Spirit");

        spirits = new Spirit[spiritObjects.Length];
        for (int i = 0; i < spiritObjects.Length; i++)
        {
            spirits[i] = spiritObjects[i].GetComponent<Spirit>();
        }

        GameObject[] strongSpiritObjects = GameObject.FindGameObjectsWithTag("StrongSpirit");

        strongSpirits = new Spirit[strongSpiritObjects.Length];
        for (int i = 0; i < strongSpiritObjects.Length; i++)
        {
            strongSpirits[i] = strongSpiritObjects[i].GetComponent<Spirit>();
        }

        GameObject[] walkerSpiritObjects = GameObject.FindGameObjectsWithTag("WalkerSpirit");

        walkerSpirits = new WalkerSpirit[walkerSpiritObjects.Length];
        for (int i = 0; i < walkerSpiritObjects.Length; i++)
        {
            walkerSpirits[i] = walkerSpiritObjects[i].GetComponent<WalkerSpirit>();
        }

        targetScaleX = transform.localScale.x;
        flyTrail = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (!isDead)
        {
            if (!isInLanes)
            {
                if (!isInDialogue)
                {
                    Move();
                    GroundCheck();
                    OpenUmbrella();
                    HandleFlipping();
                    UpdateStamina();
                    Climb();
                }
                else if (isInDialogue)
                {
                    GroundCheck();
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    animator.SetFloat("speed", 0);
                }
                    
            }
            
            else if (isInLanes)
            {
                /*rb.velocity = new Vector2(maxSpeed, rb.velocity.y);

                float targetY = lanePositions[currentLane];
                float distanceToTarget = targetY - transform.position.y;

                float targetVelocityY = (distanceToTarget > 0) ? transitionSpeed : -transitionSpeed;
                float newYVelocity = Mathf.Lerp(rb.velocity.y, targetVelocityY, transitionSpeed * Time.deltaTime);

                rb.velocity = new Vector2(rb.velocity.x, newYVelocity);

                if (Mathf.Abs(distanceToTarget) < 0.05f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    //transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                }
                else if (Mathf.Abs(distanceToTarget) < 1f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0, transitionSpeed * Time.deltaTime));
                }*/

                float targetSpeed = laneInput.y * maxSpeed;
                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocitySmoothing, smoothTime*2);
                rb.velocity = new Vector2(maxSpeed, currentSpeed);
                //animator.SetFloat("speed", Mathf.Abs(currentSpeed));

                GroundCheck();
                if (isGrounded && isUmbrellaOpen)
                {
                    umbrella.SetActive(false);
                    isUmbrellaOpen = false;
                }
                else if(!isGrounded && !isUmbrellaOpen)
                {
                    umbrella.SetActive(true);
                    isUmbrellaOpen = true;
                }

            }
        }
    }

    private void Move()
    {
        float targetSpeed = moveInput.x * maxSpeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocitySmoothing, smoothTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(currentSpeed));
        if(isGrounded && Mathf.Abs(currentSpeed) > 0.1f)
        {
            if (runParticles.isPlaying == false)
            {
                runParticles.Play();
            }
        }
        else
        {
            if (runParticles.isPlaying)
            {
                runParticles.Stop();
            }
        }

    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
        if(!isGrounded)
        {
            animator.SetBool("isJumping", false);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OpenUmbrella()
    {
        if(!isGrounded && rb.velocity.y < -0.01f && !inWind && !isClimbing)
        {
            if (canUmbrella && isUsingUmbrellaAsThrow == false)
            {
                rb.gravityScale = 0.3f;
                //umbrella.SetActive(true);
                umbrellaScript.SetIsFlying(true);
                flyTrail.emitting = true;
            }
            else
            {
                rb.gravityScale = 2.0f;
                //umbrella.SetActive(false);
                umbrellaScript.SetIsFlying(false);
                flyTrail.emitting = false;
            }
        }
        else if (inWind)
        {
            umbrellaScript.SetIsFlying(true);
            rb.gravityScale = 2.0f;
            flyTrail.emitting = true;

        }
        else if (isClimbing)
        {
            rb.gravityScale = 0f;
        }
        else 
        {
            rb.gravityScale = 2.0f;
            //umbrella.SetActive(false);
            umbrellaScript.SetIsFlying(false);
            flyTrail.emitting = false;
        }
    }

    private void HandleFlipping()
    {
        if (moveInput.x > 0 && !isFacingRight && !isFlipping)
        {
            isFacingRight = true;
            targetScaleX = Mathf.Abs(transform.localScale.x);
            isFlipping = true; 
        }
        else if (moveInput.x < 0 && isFacingRight && !isFlipping)
        {
            isFacingRight = false;
            targetScaleX = -Mathf.Abs(transform.localScale.x); 
            isFlipping = true; 
        }

        if (isFlipping)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.MoveTowards(currentScale.x, targetScaleX, flipSpeed * Time.deltaTime);
            transform.localScale = currentScale;

            if (Mathf.Approximately(currentScale.x, targetScaleX))
            {
                isFlipping = false; 
            }
        }
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            moveInput = context.ReadValue<Vector2>();
        }
            
    }

    public void OnClimbInput(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            climbInput = context.ReadValue<Vector2>();
        }
        if(context.canceled)
            jumpCancelled = false;

    }

    public void OnLaneInput(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            laneInput = context.ReadValue<Vector2>();
        }

    }

    public void OnLaneUp(InputAction.CallbackContext context)
    {
        if (!isDead && isInLanes)
        {
            if (context.performed && currentLane < lanePositions.Length - 1)
            {
                currentLane++;
            }
        }
    }
    public void OnLaneDown(InputAction.CallbackContext context)
    {
        if (!isDead && isInLanes)
        {
            if (context.performed && currentLane > 0)
            {
                currentLane--;
            }
        }
    }

    public void OnUmbrellaOpen(InputAction.CallbackContext context) // fly with umbrella
    {
        if(!isDead)
        {
            if (context.started)
            {
                canUmbrella = true;
            }
            else if (context.canceled)
            {
                canUmbrella = false;
            }
        }        
    }

    public void OnUmbrellaUse(InputAction.CallbackContext context) // deflect with umbrella
    {
        if (!isDead && canShotUmbrella && !isInDialogue)
        {
            if (context.started)
            {
                //umbrellaThrowVisual.SetActive(true);
                umbrellaThrowCollider.enabled = true;
                isUsingUmbrellaAsThrow = true;
                umbrellaScript.SetIsThrowing(true);
            }
            else if (context.canceled)
            {
                //umbrellaThrowVisual.SetActive(false);
                umbrellaThrowCollider.enabled = false;
                isUsingUmbrellaAsThrow = false;
                umbrellaScript.SetIsThrowing(false);
            }
        }
    }
    
    public void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (!isDead && !isInDialogue && !isInLanes)
        {
            if (context.performed && isGrounded)
            {
                animator.SetBool("isJumping", true);
                animator.SetTrigger("jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpParticles.Play();
            }
            else if (context.performed && isClimbing)
            {
                isClimbing = false;
                animator.SetBool("isJumping", true);
                animator.SetTrigger("jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpParticles.Play();
                jumpCancelled = true;
            }
        }
        
    }

    public void OnSingPressed(InputAction.CallbackContext context)
    {
        if (!isDead && canSing && !isInDialogue)
        {
            if (context.started && !isReloading)
            {
                singAreaVisual.SetActive(true);
                singAreaCollider.enabled = true;
                isSinging = true;
            }

            if (context.canceled)
            {
                foreach (Spirit spirit in spirits)
                {
                    if (spirit.GetInSingArea() || spirit.GetIsTouching())
                    {
                        spirit.ThrowSpirit();
                    }  
                }
                foreach (WalkerSpirit walkerSpirit in walkerSpirits)
                {
                    if (walkerSpirit.GetInSingArea() || walkerSpirit.GetIsTouching())
                    {
                        walkerSpirit.ThrowSpirit();
                    }
                }

                foreach (Spirit strongSpirit in strongSpirits)
                {
                    if (strongSpirit.GetInSingArea() || strongSpirit.GetIsTouching())
                    {
                        strongSpirit.ThrowSpirit();
                    }
                }

                singAreaVisual.SetActive(false);
                singAreaCollider.enabled = false;
                //canSing = false;
                isSinging = false;
                //Debug.Log("Can NOT sing.");
                //StartCoroutine(EnableSing());
            }
        }

    }

    public void SetMoveSpeed(float speed)
    {
        maxSpeed = speed;
    }

    public float GetMoveSpeed() 
    { 
        return maxSpeed; 
    }

    public void SetIsDead(bool dead)
    {
        isDead = dead;
    }

    public float GetJumpForce()
    {
        return jumpForce;
    }

    public void SetJumpForce(float force)
    {
        jumpForce = force;
    }

    

    private void UpdateStamina() 
    {
        //Debug.Log("Current Stamina = " + currentStamina); 
        //Debug.Log("Can Sing = " + canSing);
        if (isSinging)
        {
            if (currentStamina > 0)
            {
                if (!isHoldingSpirit)
                {
                    currentStamina -= decreaseRate * Time.deltaTime;
                }
                else if (isHoldingSpirit)
                {
                    currentStamina -= decreaseRate / 2f * Time.deltaTime;
                }
                else
                {
                    Debug.LogError("Unknown error during stamina regeneration!");
                }

                staminaBar.fillAmount = currentStamina / 100f;
            }
            else
            {
                isReloading = true;
                foreach (Spirit spirit in spirits)
                {
                    spirit.ThrowSpirit();
                }
                foreach (WalkerSpirit walkerSpirit in walkerSpirits)
                {
                    walkerSpirit.ThrowSpirit();
                }
                singAreaVisual.SetActive(false);
                singAreaCollider.enabled = false;
                //canSing = false;
                isSinging = false;
                //Debug.Log("Can NOT sing.");
                //StartCoroutine(EnableSing());
            }
        }
        else if (!isSinging)
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += increaseRate * Time.deltaTime;
                staminaBar.fillAmount = currentStamina / 100f;
            }
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                staminaBar.fillAmount = currentStamina / 100f;
                isReloading = false;
            }
        }

        UpdateStaminaBarTransparency();
    }

    public void SetIsHoldingSpirit(bool isHolding)
    {
        isHoldingSpirit = isHolding;
    }

    void UpdateStaminaBarTransparency()
    {
        Color staminaColor = staminaBar.color;

        if (isReloading)
        {
            staminaColor.a = 0.3f; 
        }
        else
        {
            staminaColor.a = 1.0f;
        }

        staminaBar.color = staminaColor; 
    }

    public void SetInLanes(bool lane)
    {
        isInLanes = lane;
    }

    public void SetCanUmbrellaShot(bool shot)
    {
        canShotUmbrella = shot;
    }

    public void SetIsInDialogue(bool inD)
    {
        isInDialogue = inD;
    }

    public bool GetCanUmbrella()
    {
        return canUmbrella;
    }

    public bool GetIsDead() { return isDead; }

    public void Die()
    {
        isDead = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        animator.SetFloat("speed", 0f);// animator setbool isdead ekle -> ölüm animasyonu
        playerHealth.Die();

    }

    public void SetInWind(bool wind)
    {
        inWind = wind;
    }

    private void Climb()
    {
        if (canClimb)
        {
            if (climbInput.y >= 0.1 && !jumpCancelled)
            {
                isClimbing = true;
                rb.velocity = new Vector2(rb.velocity.x * 0.75f, 4f);
            }
            else if (climbInput.y <= -0.1 && !jumpCancelled)
            {
                isClimbing = true;
                rb.velocity = new Vector2(rb.velocity.x * 0.75f, -4f);
            }
            else if(isClimbing && !isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.75f, 0f);
            }
            else
            {
                isClimbing = false;
            }
        }
        else
        {
            isClimbing = false;
        }

        if(climbInput.y == 0f && jumpCancelled == true)
        {
            jumpCancelled = false;
        }

        if(isClimbing && umbrellaScript.GetIsFlying() == true)
        {
            umbrellaScript.SetIsFlying(false);
        }
        
        
    }
    
    public void SetIsClimbing(bool climb)
    {
        isClimbing = climb;
    }
    public bool GetIsClimbing() { return isClimbing; }


    public void SetCanClimb(bool Climb)
    {
        canClimb = Climb;
    }
    public bool GetCanClimb() { return canClimb; }
}
