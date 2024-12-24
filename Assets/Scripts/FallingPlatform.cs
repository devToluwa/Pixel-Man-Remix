using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private float timeBeforeFlashing;
    [SerializeField] private float timeTillDestroy;

    private Color flashColor;
    private Color originalColor; // Declare this at the class level
    [SerializeField] private float flashInterval;
    [SerializeField] private int flashColorAlpha = 40;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        timeTillDestroy = timeTillDestroy + timeBeforeFlashing;

        originalColor = sr.color; // Assign originalColor here
        flashColor = originalColor;
        flashColor.a = flashColorAlpha / 255f; // Alpha should be a float between 0 and 1
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Constants.player_tag))
        {
            StartCoroutine(FallAndDestroy());
            StartCoroutine(FlashPlatformCoroutine());
        }
    }

    IEnumerator FlashPlatformCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeFlashing);
        float elapsedTime = 0f;

        // flashing effect
        while (elapsedTime < timeTillDestroy) // Use '<' to count up to timeTillDestroy
        {
            if (sr.color == originalColor) 
            {
                sr.color = flashColor; 
            }
            else 
            { 
                sr.color = originalColor; 
            }

            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        sr.color = originalColor; // Ensure platform returns to its original color
    }

    IEnumerator FallAndDestroy()
    {
        yield return new WaitForSeconds(timeTillDestroy);
        Destroy(gameObject);
    }
}
