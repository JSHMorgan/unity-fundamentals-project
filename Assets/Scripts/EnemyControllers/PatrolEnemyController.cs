using UnityEngine;

public class PatrolEnemyController : MonoBehaviour
{
    [SerializeField]
    private int distance;
    [SerializeField]
    private float speed;

    private Vector2 leftPoint;
    private Vector2 rightPoint;
    private Vector2 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftPoint = new Vector2(transform.position.x - distance / 2.0f, transform.position.y);
        rightPoint = new Vector2(transform.position.x + distance / 2.0f, transform.position.y);
        targetPosition = leftPoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float step = speed * Time.fixedDeltaTime;
        if (transform.position.x == leftPoint.x)
        {
            targetPosition = rightPoint;
            transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
        }

        if (transform.position.x == rightPoint.x)
        {
            targetPosition = leftPoint;
            transform.rotation = Quaternion.AngleAxis(0f, Vector3.up);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        Debug.DrawLine(leftPoint, rightPoint, Color.green);
    }
}
