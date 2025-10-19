using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    public SpriteRenderer sp;        // Player sprite renderer
    public Sprite startSprite;       // Default sprite
    public Sprite extendHandSprite;  // Sprite with hand extended
    public Transform handCandle;     // Candle object reference

    // Hand positions
    public Vector2 candleOriginalPos;   // Default/original pos
    public Vector2 rightHandPos = new Vector2(0.8f, -0.013f);
    public Vector2 leftHandPos = new Vector2(-0.756f, -0.239f);

    void Start()
    {
        if (sp == null) sp = GetComponent<SpriteRenderer>();
        if (handCandle != null) candleOriginalPos = handCandle.localPosition;
    }

    void Update()
    {
        // Extend hand with F
        if (Input.GetKey(KeyCode.F))
        {
            sp.sprite = extendHandSprite;

            // Flip candle depending on facing direction
            if (sp.flipX)
            {
                handCandle.localPosition = leftHandPos;
            }
            else
            {
                handCandle.localPosition = rightHandPos;
            }
        }

        // Release F → reset
        if (Input.GetKeyUp(KeyCode.F))
        {
            sp.sprite = startSprite;

            if (handCandle != null)
            {
                handCandle.localPosition = candleOriginalPos;
            }
        }
    }
}
