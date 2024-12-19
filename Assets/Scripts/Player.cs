// i hate my life
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundContactCount = 0;

    private bool isGrounded;

    // Refrences
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Debug.Log($"IsGrounded: {isGrounded}");
    }

    private void FixedUpdate()
    {
        HandlePlayerMovement();
        HandleJumping();

        HandleFlipping();
        HandleAnimations();
    }



    private void HandlePlayerMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = moveSpeed;
            animator.SetBool(Constants.IS_RUNNING_ANIM_STRING, true);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -moveSpeed;
            animator.SetBool(Constants.IS_RUNNING_ANIM_STRING, true);
        }
        else
        {
            moveInput = 0f;
            animator.SetBool(Constants.IS_RUNNING_ANIM_STRING, false);
        }

        rb.linearVelocity = new Vector2(moveInput, rb.linearVelocity.y);
    }

    private void HandleJumping()
    {
        // add if rb.velocity.y>mathf.epsilon
        // set is grounded = false
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
        }
    }

    private void HandleAnimations()
    {
        animator.SetFloat(Constants.Y_VELOCITY_ANIM_STRING, rb.linearVelocity.y);
        animator.SetBool(Constants.IS_GROUNDED_ANIM_STRING, isGrounded);
    }

    private void HandleFlipping()
    {
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Constants.GROUND_TAG) && rb.linearVelocity.y <= 0)
        {
            groundContactCount++;
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Constants.GROUND_TAG))
        {
            groundContactCount--;
            if (groundContactCount <= 0)
            {
                isGrounded = false;
                groundContactCount = 0;
            }
        }
    }

}
