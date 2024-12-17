using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;

    private bool isGrounded;

    // Refrences
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
            // FIX MOVEMENT ANIMTIONS
            //animator.SetBool(Constants.IS_RUNNING_ANIM_TAG, true);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -moveSpeed;
            // FIX MOVEMENT ANIMTIONS
            //animator.SetBool(Constants.IS_RUNNING_ANIM_TAG, true);
        }
        else
        {
            moveInput = 0f;
            // FIX MOVEMENT ANIMTIONS
            //animator.SetBool(Constants.IS_RUNNING_ANIM_TAG, false);
        }

        rb.linearVelocity = new Vector2(moveInput, rb.linearVelocity.y);
    }

    private void HandleJumping()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("We hit the ground and are pressing space or up arrow");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                isGrounded = false;
                // animator.SetTrigger(Constants.IS_JUMPING_ANIM_TAG);
            }
        }
    }

    private void HandleAnimations()
    {
        // FIX MOVEMENT ANIMTIONS
        //animator.SetFloat(Constants.IS_RUNNING_FLOAT_ANIM_TAG, rb.linearVelocityX);
    }

    private void HandleFlipping()
    {
        if (rb.linearVelocity.x > 0)
        {
            Debug.Log("Facing Right");
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (rb.linearVelocity.x < 0)
        {
            Debug.Log("Facing Left");
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Constants.GROUND_TAG))
        {
            isGrounded = true;
            HandleJumping();
            Debug.Log("Hit the ground");
        }
    }

}
