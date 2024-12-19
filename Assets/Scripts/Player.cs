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
    public Transform groundCheckPosition;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGravity;
    [SerializeField] private float maxFallSpeed = 18f;
    [SerializeField] private float fallSpeedMultiplier = 2f;

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
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        GroundCheck();
        HandlingGravity();
        HandleAnimations();

        Flip();

        Debug.Log($"Horizontal movement: {horizontalMovement}");
    }

    private void HandlingGravity()
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

    private void HandleAnimations()
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
        }
    }



}
