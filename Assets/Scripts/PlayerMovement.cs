using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (!isDead)
        {
            if (context.started)
            {
                singAreaVisual.SetActive(true);
                singAreaCollider.enabled = true;
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
}
