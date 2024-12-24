using UnityEngine;

public class SawTrap : MonoBehaviour
{
    private bool isGrounded;

    [SerializeField] private float moveSpeed;
    public Transform frontCheckPosition;
    public Vector2 frontCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    private void Update()
    {
        MoveForward();
        GroundCheckAndSwitchDirection();
    }

    private void GroundCheckAndSwitchDirection()
    {
        isGrounded = Physics2D.OverlapBox(frontCheckPosition.position, frontCheckSize, 0, groundLayer);

        // reverse direction if no more ground in front
        if (!isGrounded)
        {
            moveSpeed *= -1;
            FlipSprite();
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void FlipSprite()
    {
        if (moveSpeed > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (moveSpeed < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(frontCheckPosition.position, frontCheckSize);
    }
}
