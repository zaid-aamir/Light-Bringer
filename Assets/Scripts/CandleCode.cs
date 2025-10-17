using System.Collections;
using UnityEngine;

public class CandleCode : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private float glideSpeed = 2f;

    private PlayerMovement playerscript;
    private bool isGliding = false;

    void Start()
    {
        playerscript = Player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerscript.AnimationStart && !isGliding)
        {
            isGliding = true;
            StartCoroutine(GlideToPlayer());
        }
    }

    private IEnumerator GlideToPlayer()
    {
        Vector3 targetPos = Player.transform.position + new Vector3(0, 1f, 0); // slightly above player

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, glideSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
