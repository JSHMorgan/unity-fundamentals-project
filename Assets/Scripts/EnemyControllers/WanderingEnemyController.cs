using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(CapsuleCollider2D))]
public class WanderingEnemyController : MonoBehaviour
{
    [SerializeField, Tooltip("The speed of the enemy.")]
    private int speed;
    [SerializeField, Tooltip("The distance to send a ray-cast to check whether the enemy is going to go off a ledge.")]
    private float ledgeCheckDistance;

    public Rigidbody2D Rb2D { get; private set; }

    private bool hasFoundLedge = false;
    private void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Rb2D.linearVelocity = new Vector2(
            speed,
            Rb2D.linearVelocity.y
        );
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!hasFoundLedge && CheckChangeDirection() || CheckObstacle())
        {
            hasFoundLedge = true;
            ChangeDirection();
        }
        if (hasFoundLedge && !CheckChangeDirection())
        {
            hasFoundLedge = false;
        }
        VerticalTransformFlip();
    }

    private bool CheckChangeDirection()
    {
        Vector2 direction = Vector2.down;

        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        Vector2 leftPos = new(
            transform.position.x - collider.size.x / 2,
            transform.position.y
        );
        Vector2 rightPos = new(
            transform.position.x + collider.size.x / 2,
            transform.position.y
        );

        Debug.DrawRay(leftPos, direction * ledgeCheckDistance, Color.red);
        Debug.DrawRay(rightPos, direction * ledgeCheckDistance, Color.red);

        RaycastHit2D leftHit = Physics2D.Raycast(leftPos, direction, ledgeCheckDistance);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPos, direction, ledgeCheckDistance);

        return !leftHit || !rightHit;
    }

    private bool CheckObstacle()
    {
        Vector2 direction = (Rb2D.linearVelocityX < 0) ? Vector2.left : Vector2.right;
        Vector2 position = new(transform.position.x, transform.position.y);

        RaycastHit2D[] obstacleHits = Physics2D.RaycastAll(position, direction, ledgeCheckDistance);
        Debug.DrawRay(position, direction, Color.red);

        foreach (RaycastHit2D hit in obstacleHits)
        {
            if (hit.transform.CompareTag("Ground") && hit)
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeDirection()
    {
        Rb2D.linearVelocity = new Vector2(-Rb2D.linearVelocityX, Rb2D.linearVelocityY);
    }

    private void VerticalTransformFlip()
    {
        if (Rb2D.linearVelocityX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if (Rb2D.linearVelocityX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
