using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    // Inspector visible variables
    [Header("General")]
    [SerializeField, Tooltip("Speed for left/right player movement")]
    private float moveSpeed;
    [SerializeField, Tooltip("Set the time in seconds for how long it takes to respawn.")]
    private float respawnTimer;
    [SerializeField, Tooltip("Allows the toggling of the legacy input system.")]
    private bool legacyInput;

    [Header("Jump")]
    [SerializeField, Tooltip("Speed of the player jump force.")]
    private float jumpSpeed;
    [SerializeField, Tooltip("Allows the fall speed of the jump to be faster or slower.")]
    private float jumpGravityScale = 3.0f;
    [SerializeField, Tooltip("Turns on the double jump option for players.")]
    private bool allowDoubleJump;

    [Header("Ground Check")]
    [SerializeField, Tooltip("Set the layer for the player ground check.")]
    private LayerMask groundLayer;
    [SerializeField, Tooltip("The distance to send a ray-cast to check whether a player is grounded.")]
    private float groundedRayDistance;
    [SerializeField, Tooltip("Allows the player to jump after moving off a platform. (Seconds)")]
    private float coyoteTime = 0.07f;

    [Header("Dash")]
    [SerializeField, Tooltip("Speed for the Dash.")]
    private float dashSpeed;
    [SerializeField, Tooltip("Cooldown for the Dash ability in seconds.")]
    private float dashCooldown;
    [SerializeField, Tooltip("The distance the Dash ability moves you.")]
    private float dashDistance;

    // Inspector invisible variables.
    private Rigidbody2D rb2d;
    private bool dashAvailable;
    private bool doubleJumpAvailable;
    private bool isGrounded;
    private bool isDashing;
    private bool legacyJumpPressed;
    private bool legacyDashPressed;
    private float defaultGravityScale;
    private float coyoteTimeTimer;
    private int jumpCounter;

    public float Direction { get; private set; }

    public bool IsGrounded => GetGroundedStatus();
    public bool IsIdle => rb2d.linearVelocityX == 0;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (legacyInput)
        {
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    private void Start()
    {
        Direction = 0;
        isGrounded = true;
        isDashing = false;
        dashAvailable = true;
        doubleJumpAvailable = allowDoubleJump;
        defaultGravityScale = rb2d.gravityScale;
    }

    private void Update()
    {
        coyoteTimeTimer = isGrounded switch
        {
            true => coyoteTime,
            _ => coyoteTimeTimer - Time.deltaTime
        };

        // BUG: Breaks jumping on slopes. No slopes so not an issue.
        if (isGrounded && rb2d.linearVelocity.y == 0)
        {
            jumpCounter = 0;
        }

        HandleLegacyInput();
    }

    private void HandleLegacyInput()
    {
        if (!legacyInput)
        {
            return;
        }

        Direction = Input.GetAxisRaw("Horizontal");
        legacyJumpPressed = Input.GetKeyDown(KeyCode.Space);
        legacyDashPressed = Input.GetKeyDown(KeyCode.J);

        if (legacyJumpPressed)
        {
            OnJump();
        }

        if (legacyDashPressed)
        {
            OnDash();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        isGrounded = GetGroundedStatus();
        // Only allow movement when the player is Grounded and is not Dashing.
        if (!isGrounded || isDashing)
        {
            return;
        }
        rb2d.linearVelocity = new Vector2(
            Direction * moveSpeed, 
            rb2d.linearVelocity.y
        );
    }

    private void LateUpdate()
    {
        VerticalTransformFlip();
    }


    private void VerticalTransformFlip()
    {
        switch (rb2d.linearVelocityX)
        {
            case < 0:
                GetComponent<SpriteRenderer>().flipX = true;
                //transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
                break;
            case > 0:
                GetComponent<SpriteRenderer>().flipX = false;
                //transform.rotation = Quaternion.AngleAxis(0f, Vector3.up);
                break;
            default:
                break;
        }
    }

    public void OnMove(InputValue value)
    {
        // When a move key is pressed, get whether they are moving in a negative (left) or positive (right) direction.
        Direction = value.Get<float>();
    }

    public void OnJump()
    {
        // Only allow jumping when the player is within coyote time unless they have double jump available.
        switch (doubleJumpAvailable)
        {
            case true when jumpCounter == 1:
            {
                _ = StartCoroutine(HandleDoubleJump());
                jumpCounter = 0;
                break;
            }
            case true when coyoteTimeTimer < 0f:
            case false when coyoteTimeTimer < 0f:
            {
                return;
            }
            default:
            {
                jumpCounter++;
                break;
            }
        }

        rb2d.linearVelocity = new Vector2(
            Direction * moveSpeed,
            jumpSpeed
        );
        _ = StartCoroutine(HandleJumpGravity());
    }

    public void OnDash()
    {
        bool playerMoving = Direction != 0f;
        if (!dashAvailable || !playerMoving)
        {
            return;
        }

        _ = StartCoroutine(HandleDash());
        _ = StartCoroutine(HandleDashCooldown());
    }

    private bool GetGroundedStatus()
    {
        Vector2 rayDirection = Vector2.down;

        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        Vector2 leftPos = new(
            transform.position.x - collider.size.x / 2,
            transform.position.y
        );
        Vector2 rightPos = new(
            transform.position.x + collider.size.x / 2,
            transform.position.y
        );

        Debug.DrawRay(leftPos, rayDirection * groundedRayDistance, Color.red);
        Debug.DrawRay(rightPos, rayDirection * groundedRayDistance, Color.red);

        RaycastHit2D leftHit = Physics2D.Raycast(leftPos, rayDirection, groundedRayDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPos, rayDirection, groundedRayDistance, groundLayer);
        return leftHit.collider || rightHit.collider;
    }

    private IEnumerator HandleDash()
    {
        float previousDirection = Direction;
        float previousVelocityX = rb2d.linearVelocity.x;
        float previousGravity = rb2d.gravityScale;

        Vector2 dashStartPos = transform.position;
        Vector2 dashForce = new(Direction * dashSpeed, rb2d.linearVelocityY);

        rb2d.AddForce(dashForce, ForceMode2D.Impulse);
        rb2d.gravityScale = 0f;
        isDashing = true;

        yield return new WaitUntil(() =>
        {
            bool isDashFinished = Vector2.Distance(dashStartPos, transform.position) >= dashDistance;
            return isDashFinished;
        });

        isDashing = false;
        rb2d.gravityScale = previousGravity;

        // Apply the previous velocity in the current direction.
        rb2d.linearVelocityX = Mathf.Abs(previousVelocityX) * previousDirection;
    }

    private IEnumerator HandleDashCooldown()
    {
        dashAvailable = false;
        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }

    private IEnumerator HandleDoubleJump()
    {
        doubleJumpAvailable = false;
        yield return new WaitUntil(() => isGrounded);
        doubleJumpAvailable = true;
    }

    private IEnumerator HandleDeath()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(respawnTimer);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Turn gravity off while moving upwards.
    private IEnumerator HandleJumpGravity()
    {
        yield return new WaitUntil(() => rb2d.linearVelocity.y < 0);
        rb2d.gravityScale = jumpGravityScale;
        yield return new WaitUntil(() => isGrounded || rb2d.linearVelocity.y >= 0);
        rb2d.gravityScale = defaultGravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Death!");
            _ = StartCoroutine(HandleDeath());
        }
    }
}
