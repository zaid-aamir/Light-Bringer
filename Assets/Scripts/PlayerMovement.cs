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
    [SerializeField] private Sprite handOutSprite; // <-- new sprite for F-key

    [Header("UI")]
    [SerializeField] private Text noteText;

    [System.Serializable]
    public class NoteData
    {
        public string noteTag;     // e.g. "Note1", "Note2"
        [TextArea] public string noteMessage; // message shown
    }

    [SerializeField] private NoteData[] notes; // all notes set in inspector

    // Candle positions relative to player
    private Vector2 candleIdleRight = new Vector2(0.8f, -0.239f);
    private Vector2 candleIdleLeft = new Vector2(-0.756f, -0.239f);
    private Vector2 candleHandOutRight = new Vector2(0.8f, -0.013f);
    private Vector2 candleHandOutLeft = new Vector2(-0.756f, -0.013f);

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        sp.sprite = startSprite;

        if (handCandle != null)
        {
            handCandle.SetActive(false);
        }

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

            // Start with candle on right by default
            handCandle.transform.localPosition = candleIdleRight;
        }
    }

    void Update()
    {
        // --- Movement ---
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        // Flip sprite and candle based on movement direction
        if (horizontalInput > 0.01f)
        {
            sp.flipX = false;
            if (!Input.GetKey(KeyCode.F)) handCandle.transform.localPosition = candleIdleRight;
        }
        else if (horizontalInput < -0.01f)
        {
            sp.flipX = true;
            if (!Input.GetKey(KeyCode.F)) handCandle.transform.localPosition = candleIdleLeft;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        // --- F-key sprite switch and candle movement ---
        if (Input.GetKeyDown(KeyCode.F) && handOutSprite != null)
        {
            sp.sprite = handOutSprite;
            if (handCandle != null)
            {
                handCandle.SetActive(true);
                handCandle.transform.SetParent(transform);
                handCandle.transform.localPosition = sp.flipX ? candleHandOutLeft : candleHandOutRight;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            sp.sprite = startSprite;
            if (handCandle != null)
            {
                handCandle.transform.localPosition = sp.flipX ? candleIdleLeft : candleIdleRight;
            }
        }

        // --- Death check ---
        if (transform.position.y <= -20f)
        {
            SceneManager.LoadScene("DeathScene");
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

                if (handCandle != null)
                {
                    handCandle.SetActive(true);
                    handCandle.transform.SetParent(transform);
                    handCandle.transform.localPosition = sp.flipX ? candleIdleLeft : candleIdleRight;
                }
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

        foreach (NoteData note in notes)
        {
            if (collision.gameObject.CompareTag(note.noteTag))
            {
                Destroy(collision.gameObject);

                if (noteText != null)
                {
                    noteText.text = note.noteMessage;
                    noteText.gameObject.SetActive(true);

                    StartCoroutine(HideNoteTextAfterDelay(10f));
                }
                break;
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
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
