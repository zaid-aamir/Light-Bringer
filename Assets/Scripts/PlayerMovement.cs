using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Glide Settings")]
    [SerializeField] private float glideSpeed = 2f;
    [SerializeField] private float glideHeight = 5f;

    private bool isGrounded = false;
    private bool isGliding = false;
    public bool AnimationStart = false;
    private float glideTargetY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isGliding) // Normal movement
        {
            // Horizontal movement
            float horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

            // Jump
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (isGliding)
        {
            // Glide smoothly up
            rb.position = Vector2.MoveTowards(rb.position, new Vector2(rb.position.x, glideTargetY), glideSpeed * Time.fixedDeltaTime);

            // Stop gliding when reached target
            if (Mathf.Abs(rb.position.y - glideTargetY) < 0.01f)
            {
                isGliding = false;

                // Snap to final position to avoid bobbing
                rb.position = new Vector2(rb.position.x, glideTargetY);

                // Restore gravity and unlock X movement
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Candle"))
        {
            // Start gliding
            isGliding = true;
            AnimationStart = true;

            glideTargetY = rb.position.y + glideHeight;

            // Stop movement and disable gravity
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;

            // Freeze X and rotation during glide
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
