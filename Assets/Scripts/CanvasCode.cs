using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasCode : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Image fogImage;        // The UI Image covering the area
    [SerializeField] private Sprite revealedSprite; // Sprite to show after candle
    [SerializeField] private Text visionText;       // Text to display

    [Header("Player Reference")]
    [SerializeField] private PlayerMovement playerScript;

    [Header("Text Settings")]
    [SerializeField] private float textDisplayTime = 2f; // Time text stays visible
    [SerializeField] private float fadeSpeed = 2f;        // Speed of fade in/out

    void Start()
    {
        // Hide text at the start and set alpha to 0
        if (visionText != null)
        {
            visionText.gameObject.SetActive(false);
            Color c = visionText.color;
            c.a = 0f;
            visionText.color = c;
        }
    }

    void Update()
    {
        // Check if the player's animation finished
        if (playerScript.AnimationStart)
        {
            // Trigger only once
            playerScript.AnimationStart = false;

            // Change UI image
            ChangeUI();

            // Show and fade the vision text
            if (visionText != null)
                StartCoroutine(FadeTextCoroutine());
        }
    }

    private void ChangeUI()
    {
        if (fogImage != null && revealedSprite != null)
        {
            fogImage.sprite = revealedSprite;
        }
    }

    private IEnumerator FadeTextCoroutine()
    {
        visionText.gameObject.SetActive(true);

        // Fade in
        Color c = visionText.color;
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            visionText.color = c;
            yield return null;
        }

        // Stay visible
        yield return new WaitForSeconds(textDisplayTime);

        // Fade out
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            visionText.color = c;
            yield return null;
        }

        visionText.gameObject.SetActive(false);
    }
}
