// i hate my life
// help
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement")]
    private float horizontalMovement;
    [SerializeField] private float moveSpeed;



    [Header("Jumping")]
    [SerializeField] private float jumpPower = 20f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int jumpsRemaining;


    [Header("Ground Checks")]
    [SerializeField] private bool isGrounded;
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;


    [Header("Gravity")]
    [SerializeField] private float baseGravity;
    [SerializeField] private float maxFallSpeed = 18f;
    [SerializeField] private float fallSpeedMultiplier = 2f;


    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private bool isWallSliding;


    [Header("Wall Jumping")]
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpDirection;
    [SerializeField] private float wallJumpTime = 0.5f;
    [SerializeField] private float wallJumpTimer;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 20f);


    [Header("Wall Check")]
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private LayerMask wallLayer;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        baseGravity = rb.gravityScale;
    }

    private void Update()
    {
        GroundCheck();
        ProcessAnimations();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        // moving da character
        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

        Debug.Log($"Horizontal movement: {horizontalMovement}");
    }



    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
        }
        else if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void ProcessAnimations()
    {

        bool isGrounded = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0, groundLayer);

        void leftRightAnim()
        {
            if (horizontalMovement != 0)
            {
                animator.SetBool(Constants.anim_string_ISMOVING, true);
            }
            else
            {
                animator.SetBool(Constants.anim_string_ISMOVING, false);
            }
        }

        void jumpAnim()
        {
            if (rb.linearVelocity.y < 0)
            {
                animator.SetFloat(Constants.anim_string_ISFALLING, rb.linearVelocity.y);
            }
            else if (isGrounded)
            {
                animator.SetBool(Constants.anim_string_ISFALLING, false);
            }
        }

        void isGroundedCheckAnim()
        {
            animator.SetFloat(Constants.anim_string_Y_VELOCITY, rb.linearVelocity.y);

            animator.SetBool(Constants.anim_string_ISGROUNDED, isGrounded);
        }

        leftRightAnim();
        jumpAnim();
        isGroundedCheckAnim();

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                animator.SetTrigger(Constants.anim_string_trigger_JUMP);
                jumpsRemaining--;
            }
            else if (context.canceled)
            {
                // NOTE we are like doing a half jump when we dont fully press,
                // Need to change the number to something dynamic like how much we press the button
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * 0.5f);
                jumpsRemaining--;
            }
        }

        // wall jumping
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            // we need to force a flip when we wall jump
            if (transform.localScale.x != wallJumpDirection)
            {
                if (horizontalMovement > 0)
                {
                    transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                }
                else if (horizontalMovement < 0)
                {
                    transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                }
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); // wall jump will last .5seconds, can jump again after .6seconds
        }
    }

    private void Flip()
    {
        if (horizontalMovement > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (horizontalMovement < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPosition.position, wallCheckSize, 0, wallLayer);
    }

    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; // fall faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); // cap the fall speed
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        // We need to be not on the ground && On a wall & movement!= 0
        if (!isGrounded && WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            animator.SetBool(Constants.anims_string_ISWALLSLIDING, true);
            // the below caps our fall rate when wallsliding
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));


            //as soon as we wall slide we can wall jump again
            CancelInvoke(nameof(CancelWallJump));
        }
        else
        {
            isWallSliding = false;
            animator.SetBool(Constants.anims_string_ISWALLSLIDING, false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckSize);
    }
}
