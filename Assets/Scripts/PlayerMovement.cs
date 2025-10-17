using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject handCandle; // Candle that appears in hand
    private SpriteRenderer sp;
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

    [Header("Sprites")]
    public Sprite finishedSprite; // Sprite when holding candle
    public Sprite startSprite;    // Default sprite

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Start with default sprite
        sp.sprite = startSprite;

        // Hide the in-hand candle until needed
        handCandle.SetActive(false);
    }

    void Update()
    {
        if (!isGliding) // Normal movement
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

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
            // Smoothly glide up
            rb.position = Vector2.MoveTowards(rb.position, new Vector2(rb.position.x, glideTargetY), glideSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(rb.position.y - glideTargetY) < 0.01f)
            {
                isGliding = false;

                // Snap to final position
                rb.position = new Vector2(rb.position.x, glideTargetY);

                // Restore gravity and X movement
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                // Change to holding sprite
                sp.sprite = finishedSprite;

                // Activate in-hand candle and attach to player
                handCandle.SetActive(true);
                handCandle.transform.SetParent(transform);
                handCandle.transform.localPosition = new Vector2(0.8f, -0.239f);
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

            // Hide or destroy the pickup candle immediately
            collision.gameObject.SetActive(false);
            // Or: Destroy(collision.gameObject);
        }
    }
}
