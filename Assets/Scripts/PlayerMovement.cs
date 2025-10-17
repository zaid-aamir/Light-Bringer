using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject handCandle;
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
    public Sprite finishedSprite;
    public Sprite startSprite;

    [Header("UI")]
    [SerializeField] private Text noteText;
    [SerializeField] private string noteMessage = "Continue the path to the castle...";

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        sp.sprite = startSprite;
        handCandle.SetActive(false);

        if (noteText != null)
        {
            noteText.gameObject.SetActive(false);
        }

        // Tower scene: start with torch
        if (SceneManager.GetActiveScene().name == "Tower")
        {
            sp.sprite = finishedSprite;
            handCandle.SetActive(true);
            handCandle.transform.SetParent(transform);
            handCandle.transform.localPosition = new Vector2(0.8f, -0.239f);
        }
    }

    void Update()
    {
        if (!isGliding)
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
            rb.position = Vector2.MoveTowards(rb.position, new Vector2(rb.position.x, glideTargetY), glideSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(rb.position.y - glideTargetY) < 0.01f)
            {
                isGliding = false;

                rb.position = new Vector2(rb.position.x, glideTargetY);

                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                sp.sprite = finishedSprite;

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
            isGliding = true;
            AnimationStart = true;
            glideTargetY = rb.position.y + glideHeight;

            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

            collision.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Note1"))
        {
            Destroy(collision.gameObject);

            if (noteText != null)
            {
                noteText.text = noteMessage;
                noteText.gameObject.SetActive(true);

                StartCoroutine(HideNoteTextAfterDelay(10f));
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            // Stop horizontal velocity when touching a wall
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Prevent wall sticking by locking horizontal movement
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private IEnumerator HideNoteTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (noteText != null)
        {
            noteText.gameObject.SetActive(false);
        }
    }
}
