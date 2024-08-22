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
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        umbrella.SetActive(false);
    }

    void Update()
    {
        Move();
        Jump();
        OpenUmbrella();
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * maxSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void OpenUmbrella()
    {
        if(!isGrounded && rb.velocity.y < -0.01f)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.gravityScale = 0.3f;
                umbrella.SetActive(true);
            } 
        }
        else 
        {
            rb.gravityScale = 2.0f;
            umbrella.SetActive(false);
        }

    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnUmbrellaOpen(InputAction.CallbackContext context)
    {
        //context.performed
        if (context.started)
        {
            // Þemsiye açýldý
            Debug.Log("Umbrella Opened");
        }
        else if (context.canceled)
        {
            // Þemsiye kapandý
            Debug.Log("Umbrella Closed");
        }
    }
}
