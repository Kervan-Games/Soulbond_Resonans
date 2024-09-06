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
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [SerializeField] GameObject umbrella;

    private Vector2 moveInput;

    private bool canUmbrella;

    private float currentSpeed = 0f;
    private float smoothTime = 0.1f; 
    private float velocitySmoothing = 0f;

    private Spirit[] spirits;
    private WalkerSpirit[] walkerSpirits; 

    private bool isFacingRight;
    private bool isDead;

    public GameObject singArea;
    public GameObject singAreaVisual;
    private Collider2D singAreaCollider;

    private bool canSing = true;
    private bool isSinging = false;
    private float singCoolDown = 3f;
    private float singDuration = 3f;

    public Image staminaBar;
    private float maxStamina = 100f;
    private float currentStamina;
    private float decreaseRate = 50f;
    private float increaseRate = 30f;
    private bool isReloading = false;
    private bool isHoldingSpirit = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        singAreaCollider = singArea.GetComponent<Collider2D>();
        umbrella.SetActive(false);
        canUmbrella = false;
        isFacingRight = true;
        isDead = false;
        singAreaVisual.SetActive(false);
        singAreaCollider.enabled = false;

        currentStamina = maxStamina;

        GameObject[] spiritObjects = GameObject.FindGameObjectsWithTag("Spirit");

        spirits = new Spirit[spiritObjects.Length];
        for (int i = 0; i < spiritObjects.Length; i++)
        {
            spirits[i] = spiritObjects[i].GetComponent<Spirit>();
        }

        GameObject[] walkerSpiritObjects = GameObject.FindGameObjectsWithTag("WalkerSpirit");

        walkerSpirits = new WalkerSpirit[walkerSpiritObjects.Length];
        for (int i = 0; i < walkerSpiritObjects.Length; i++)
        {
            walkerSpirits[i] = walkerSpiritObjects[i].GetComponent<WalkerSpirit>();
        }
    }

    void Update()
    {
        if (!isDead)
        {
            Move();
            GroundCheck();
            OpenUmbrella();
            HandleFlipping();
            UpdateStamina();
        }
    }

    private void Move()
    {
        float targetSpeed = moveInput.x * maxSpeed;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocitySmoothing, smoothTime);
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OpenUmbrella()
    {
        if(!isGrounded && rb.velocity.y < -0.01f)
        {
            if (canUmbrella)
            {
                rb.gravityScale = 0.3f;
                umbrella.SetActive(true);
            }
            else
            {
                rb.gravityScale = 2.0f;
                umbrella.SetActive(false);
            }
        }
        else 
        {
            rb.gravityScale = 2.0f;
            umbrella.SetActive(false);
        }
    }

    private void HandleFlipping()
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1; 
            transform.localScale = theScale;
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1; 
            transform.localScale = theScale;
        }
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            moveInput = context.ReadValue<Vector2>();
        }
            
    }

    public void OnUmbrellaOpen(InputAction.CallbackContext context)
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
    
    public void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (!isDead)
        {
            if (context.performed && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
        
    }

    public void OnSingPressed(InputAction.CallbackContext context)
    {
        if (!isDead && canSing)
        {
            if (context.started && !isReloading)
            {
                singAreaVisual.SetActive(true);
                singAreaCollider.enabled = true;
                isSinging = true;
                //StartCoroutine(StopSing());
            }

            if (context.canceled)
            {
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


    //******* NOT USING *******
    IEnumerator EnableSing()
    {
        yield return new WaitForSeconds(singCoolDown);
        canSing = true;
        Debug.Log("Can sing.");
    }

    IEnumerator StopSing()
    {
        yield return new WaitForSeconds(singDuration);
        if (isSinging)
        {
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
            canSing = false;
            isSinging = false;
            Debug.Log("Sing stopped by duration.");
            StartCoroutine(EnableSing());
        }
    }
}
