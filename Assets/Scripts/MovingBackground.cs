using UnityEngine;
using System.Collections.Generic;

public class MovingBackground : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxY_Position;
    [SerializeField] List<Sprite> listOfBackgrounds;

    private Vector2 originalPosition;
    private SpriteRenderer backgroundSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        backgroundSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        originalPosition = transform.position;
        int randomIndex = Random.Range(0, listOfBackgrounds.Count);
        backgroundSprite.sprite = listOfBackgrounds[randomIndex];
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.up * moveSpeed;
        if (transform.position.y >= maxY_Position)
        {
            transform.position = originalPosition;
        }
    }
}
