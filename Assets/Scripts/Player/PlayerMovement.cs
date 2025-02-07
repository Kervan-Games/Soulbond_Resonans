using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


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

    private GameObject rope;

    private GameObject[] spiritObjects;
    private GameObject[] strongSpiritObjects;  
    private GameObject[] walkerSpiritObjects;

    private bool isPaused = false;
    public GameObject pauseMenu;

    public Volume volume;
    public GameObject newScreamVisual;

    public ParticleSystem screamParticle;

    private bool isJumpPressedDown = false;
    private bool isInJumpPad = false;

    // ATTACK PHASE:
    private bool attackPhase = false;
    private bool isAttacking = false;
    private bool canAttack = true;

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

        spiritObjects = GameObject.FindGameObjectsWithTag("Spirit");

        spirits = new Spirit[spiritObjects.Length];
        for (int i = 0; i < spiritObjects.Length; i++)
        {
            spirits[i] = spiritObjects[i].GetComponent<Spirit>();
        }

        strongSpiritObjects = GameObject.FindGameObjectsWithTag("StrongSpirit");

        strongSpirits = new Spirit[strongSpiritObjects.Length];
        for (int i = 0; i < strongSpiritObjects.Length; i++)
        {
            strongSpirits[i] = strongSpiritObjects[i].GetComponent<Spirit>();
        }

        walkerSpiritObjects = GameObject.FindGameObjectsWithTag("WalkerSpirit");

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!attackPhase)
            {
                animator.SetTrigger("attackPhase");
                attackPhase = true;
            }
        }
        if (!isDead)
        {
            if (!isInLanes && !attackPhase)
            {
                if (!isInDialogue)
                {
                    if (!isClimbing && !isSinging)
                        Move();
                    else
                    {
                        currentSpeed = 0;
                        rb.velocity = new Vector2(0f, rb.velocity.y); // here is not necessary 
                    }
                    GroundCheck();
                    OpenUmbrella();
                    if (!isSinging || isFlipping)
                    {
                        HandleFlipping();
                    }
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
            else if (attackPhase)
            {
                if (umbrellaThrow.activeSelf)
                {
                    umbrellaThrow.SetActive(false);
                }

                if (!isAttacking)
                {
                    Move();
                }
                else
                {
                    currentSpeed = 0;
                    rb.velocity = new Vector2(0f, rb.velocity.y); // here is not necessary 
                }
                GroundCheck();
                OpenUmbrella();
                if (!isAttacking || isFlipping)
                {
                    HandleFlipping();
                }
                UpdateStamina();
            }
        }
        if (isDead && isFlipping)
            HandleFlipping();
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
            animator.SetBool("isUmbrellaFly", false);
        }
        else
        {
            animator.SetBool("isUmbrellaFly", false); 
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
                if(!isGrounded)
                    animator.SetBool("isUmbrellaFly", true);
                else
                    animator.SetBool("isUmbrellaFly", false);
            }
            else
            {
                rb.gravityScale = 2.0f;
                //umbrella.SetActive(false);
                umbrellaScript.SetIsFlying(false);
                flyTrail.emitting = false;
                animator.SetBool("isUmbrellaFly", false);
            }
        }
        else if (inWind)
        {
            umbrellaScript.SetIsFlying(true);
            rb.gravityScale = 0f;
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
        if (moveInput.x > 0 && !isFacingRight && !isFlipping && !isClimbing)
        {
            isFacingRight = true;
            targetScaleX = Mathf.Abs(transform.localScale.x);
            isFlipping = true; 
        }
        else if (moveInput.x < 0 && isFacingRight && !isFlipping && !isClimbing)
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
        if (!isDead && !isPaused)
        {
            moveInput = context.ReadValue<Vector2>();
        }
            
    }

    public void OnClimbInput(InputAction.CallbackContext context)
    {
        if (!isDead && !isPaused)
        {
            climbInput = context.ReadValue<Vector2>();
        }
        if(context.canceled)
            jumpCancelled = false;

    }

    public void OnLaneInput(InputAction.CallbackContext context)
    {
        if (!isDead && !isPaused)
        {
            laneInput = context.ReadValue<Vector2>();
        }

    }

    public void OnLaneUp(InputAction.CallbackContext context)
    {
        if (!isDead && isInLanes && !isPaused)
        {
            if (context.performed && currentLane < lanePositions.Length - 1)
            {
                currentLane++;
            }
        }
    }
    public void OnLaneDown(InputAction.CallbackContext context)
    {
        if (!isDead && isInLanes && !isPaused)
        {
            if (context.performed && currentLane > 0)
            {
                currentLane--;
            }
        }
    }

    public void OnUmbrellaOpen(InputAction.CallbackContext context) // fly with umbrella
    {
        if(!isDead && !isPaused)
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
        if (!isDead && canShotUmbrella && !isInDialogue && !isPaused)
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
        if (!isDead && !isInDialogue && !isInLanes && !isPaused)
        {
            if (context.performed && isGrounded && !isSinging && !isAttacking)
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
            else if(context.performed && isInJumpPad && !isJumpPressedDown)
            {
                isJumpPressedDown = true;
                StartCoroutine(ResetIsJumpPressed());
            }
        }
        
    }
    private IEnumerator ResetIsJumpPressed()
    {
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("jump false");
        isJumpPressedDown = false;
    }


    public void OnPausePressed(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            if (!isPaused)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                SetDepthOfField(true);
                isPaused = true;
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;
                SetDepthOfField(false);
            }
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

    }
    public void OnSingPressed(InputAction.CallbackContext context)
    {
        if (!attackPhase)
        {
            if (!isDead && canSing && !isInDialogue && !isPaused && isGrounded && !isClimbing)
            {
                if (context.started && !isReloading)
                {
                    animator.SetBool("isSinging", true);
                    isSinging = true;
                }

                if (context.canceled)
                {
                    //ThrowAllTouchingSpirits();
                    animator.SetBool("isSinging", false);
                    //singAreaVisual.SetActive(false);
                    singAreaCollider.enabled = false;
                    //newScreamVisual.SetActive(false);
                    isSinging = false;
                    screamParticle.Stop();
                }
            }
            else
            {
                animator.SetBool("isSinging", false);
                isSinging = false;
                //newScreamVisual.SetActive(false);
                screamParticle.Stop();

            }
        }
        else if (attackPhase)
        {
            if (!isDead && canAttack && !isPaused && isGrounded && !isClimbing)
            {
                if (context.performed)
                {
                    animator.SetTrigger("attack");
                    canAttack = false;
                    isAttacking = true;
                }

            }
        }

    }

    public void SetSingAreaCollider(bool sett)
    {
        singAreaCollider.enabled = sett;
    }

    public void ThrowAllTouchingSpirits()
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
                /*foreach (Spirit spirit in spirits)
                {
                    spirit.ThrowSpirit();
                }
                foreach (WalkerSpirit walkerSpirit in walkerSpirits)
                {
                    walkerSpirit.ThrowSpirit();
                }*/
                //singAreaVisual.SetActive(false);
                singAreaCollider.enabled = false;
                isSinging = false;

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
    public void SetSingAreaColliderTrue()
    {
        //screamParticle.Play();
        SetSingAreaCollider(true);
        //newScreamVisual.SetActive(true);
    }

    public void StartSingParticle()
    {
        screamParticle.Play();
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
            animator.SetBool("isSinging", false);
            SetSingAreaCollider(false);
            //newScreamVisual.SetActive(false);
            screamParticle.Stop();
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

    public void DieVisual()
    {

        playerHealth.DieVisual();
    }

    public void Die()
    {
        isDead = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        animator.SetFloat("speed", 0f);// animator setbool isdead ekle -> ölüm animasyonu
        SetDepthOfField(true);
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
                animator.SetBool("isClimbing", true);
                flyTrail.emitting = false;

                if (runParticles.isPlaying)
                {
                    runParticles.Stop();
                }

                if (rb.velocity.y > 0)
                {
                    animator.SetFloat("climbSpeed", 4f);
                }
                else
                {
                    animator.SetFloat("climbSpeed", 0f);
                }

                if (isFacingRight)
                    transform.position = new Vector3(rope.transform.position.x - 0.59f, transform.position.y, transform.position.z);
                else
                    transform.position = new Vector3(rope.transform.position.x + 0.59f, transform.position.y, transform.position.z);
            }
            else if (climbInput.y <= -0.1 && !jumpCancelled)
            {
                isClimbing = true;
                rb.velocity = new Vector2(rb.velocity.x * 0.75f, -4f);
                animator.SetBool("isClimbing", true);

                if (runParticles.isPlaying)
                {
                    runParticles.Stop();
                }

                if (rb.velocity.y < 0)
                {
                    animator.SetFloat("climbSpeed", -4f);
                }
                else
                {
                    animator.SetFloat("climbSpeed", 0f);
                }
                
            }
            else if(isClimbing && !isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x * 0.75f, 0f);
                animator.SetBool("isClimbing", true);
                animator.SetFloat("climbSpeed", 0f);
            }
            else
            {
                isClimbing = false;
                animator.SetBool("isClimbing", false);
                animator.SetFloat("climbSpeed", 0f);
            }
        }
        else
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
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

    public void SetAnimatorIsDeadTrue()
    {
        isDead = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        animator.SetFloat("speed", 0f);
        animator.SetBool("isDead", true);
    }

    public void SetRope(GameObject _rope)
    {
        rope = _rope;
    }

    public void UpdateSpiritAmounts()
    {
        spiritObjects = GameObject.FindGameObjectsWithTag("Spirit");

        spirits = new Spirit[spiritObjects.Length];
        for (int i = 0; i < spiritObjects.Length; i++)
        {
            spirits[i] = spiritObjects[i].GetComponent<Spirit>();
        }

        strongSpiritObjects = GameObject.FindGameObjectsWithTag("StrongSpirit");

        strongSpirits = new Spirit[strongSpiritObjects.Length];
        for (int i = 0; i < strongSpiritObjects.Length; i++)
        {
            strongSpirits[i] = strongSpiritObjects[i].GetComponent<Spirit>();
        }

        walkerSpiritObjects = GameObject.FindGameObjectsWithTag("WalkerSpirit");

        walkerSpirits = new WalkerSpirit[walkerSpiritObjects.Length];
        for (int i = 0; i < walkerSpiritObjects.Length; i++)
        {
            walkerSpirits[i] = walkerSpiritObjects[i].GetComponent<WalkerSpirit>();
        }
    }

    public void SetDepthOfField(bool depth)
    {
        if (volume.profile.TryGet<DepthOfField>(out DepthOfField depthOfField))
        {
            depthOfField.active = depth; 
        }

    }

    public void SetIsPaused(bool isp)
    {
        isPaused = isp;
    }

    public void SetCurrentSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    public bool GetIsJumpPressedDown() 
    { 
        return isJumpPressedDown; 
    }

    public void SetIsInJumpPad(bool isIn)
    {
        isInJumpPad = isIn;
    }

    public void SetCanAttackTrue()
    {
        canAttack = true;
        isAttacking = false;
    }

    public bool GetIsAttacking() { return isAttacking; }

}
