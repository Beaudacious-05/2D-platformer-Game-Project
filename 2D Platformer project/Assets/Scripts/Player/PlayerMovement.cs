using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float accelerationTime = 0.1f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public int maxJumps = 2;
    public float coyoteTime = 0.2f;
    public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 2f;
    public float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private float velocityX;
    private float currentVelocity;
    private int jumpsRemaining;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Update ground & coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jump if buffer and coyote conditions are met
        if (jumpBufferCounter > 0f && (jumpsRemaining > 0 || coyoteTimeCounter > 0f))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsRemaining--;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }

        // Cut jump short if button released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
        }
    }

    void FixedUpdate()
    {
        // Smooth horizontal movement
        float targetVelocityX = moveInput * moveSpeed;
        velocityX = Mathf.SmoothDamp(rb.velocity.x, targetVelocityX, ref currentVelocity, accelerationTime);
        rb.velocity = new Vector2(velocityX, rb.velocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset jumps on ground
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }

        // Better fall gravity
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
