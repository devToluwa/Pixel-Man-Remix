using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float timerToFall = 2f;
    [SerializeField] private float offScreenPositionToDestroy = -10f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitutde = 0.1f;



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
        // Shake the platform
        Vector2 originalPosition = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            transform.position = originalPosition + (Vector2)Random.insideUnitCircle * shakeMagnitutde;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition; // reset to original position

        // wait before making platform fall, then make it fall
        yield return new WaitForSeconds(timerToFall);
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Continuously monitor the platform's position
        while (transform.position.y > offScreenPositionToDestroy)
        {
            yield return null; // Wait for the next frame
        }
        Destroy(gameObject);
    }
}
