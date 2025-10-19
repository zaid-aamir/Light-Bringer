using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;       // Speed of enemy
    [SerializeField] private float chaseRange = 5f;      // How close the player needs to be
    [SerializeField] private float movementBounds = 10f; // Max horizontal distance from start

    private Transform player;        // Reference to player
    private Rigidbody2D rb;          // Enemy's Rigidbody
    private float startX;            // Enemy's initial X position
    private float yPosition;         // Fixed Y position

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startX = transform.position.x;
        yPosition = transform.position.y;

        // Freeze vertical movement so enemy only moves on X axis
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        // Find player in scene (assumes player tagged "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceX = player.position.x - transform.position.x;
        float moveDir = 0f;

        if (Mathf.Abs(distanceX) <= chaseRange)
        {
            // Player is close → chase or run away
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            if (playerScript != null && Input.GetKey(KeyCode.F))
            {
                // Run away from player
                moveDir = (distanceX < 0) ? 1f : -1f;
            }
            else
            {
                // Chase player
                moveDir = (distanceX > 0) ? 1f : -1f;
            }
        }
        else
        {
            // Player is far → return to original spawn position
            if (Mathf.Abs(transform.position.x - startX) > 0.05f) // small buffer
            {
                moveDir = (transform.position.x < startX) ? 1f : -1f;
            }
            else
            {
                moveDir = 0f; // Stop moving when back at spawn
            }
        }

        // Apply movement with Rigidbody2D
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        // Clamp position inside bounds
        float clampedX = Mathf.Clamp(transform.position.x, startX - movementBounds, startX + movementBounds);
        if (Mathf.Abs(transform.position.x - clampedX) > 0.01f)
        {
            rb.position = new Vector2(clampedX, rb.position.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerScript != null && Input.GetKey(KeyCode.F))
            {
                // Player kills enemy when touching while holding F
                Destroy(gameObject);
            }
            else
            {
                SceneManager.LoadScene("DeathScene");
            }

        }
    }
}
