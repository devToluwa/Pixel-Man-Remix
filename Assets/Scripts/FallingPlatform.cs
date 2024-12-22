using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float offScreenPositionToDestroy = -10f;
    [SerializeField] private float timerToFall = 2f;

    [SerializeField] private Color colorToChangeTo;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private float flashDuration = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Constants.player_tag))
        {
            StartCoroutine(FallAndDestroy());
        }
    }

    IEnumerator FallAndDestroy()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        Color flashColor = colorToChangeTo;

        float elapsedTime = 0f;
        // Flashing effect
        while (elapsedTime < flashDuration)
        {
            sr.color = sr.color == originalColor ? flashColor : originalColor;
            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }
        sr.color = originalColor; // Reset to the original color

        // Wait before making the platform fall
        yield return new WaitForSeconds(timerToFall);


        // Destroy the platform
        Destroy(gameObject);
    }
}
