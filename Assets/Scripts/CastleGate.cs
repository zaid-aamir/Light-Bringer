using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CastleGate : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "CastleScene"; // name of your new scene
    [SerializeField] private Text gateText; // assign a UI Text in Canvas (e.g. "Press Q to enter")

    private bool isPlayerInRange = false;

    void Start()
    {
        if (gateText != null)
            gateText.gameObject.SetActive(false); // hide at start
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (gateText != null)
                gateText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (gateText != null)
                gateText.gameObject.SetActive(false);
        }
    }
}
