using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FallingBlockController : MonoBehaviour
{
    public enum State
    {
        Fall,
        Reset,
        Rest
    }
    public State CurrentState => currentState;

    [SerializeField, Tooltip("Downwards speed")]
    private float downwardSpeed;
    [SerializeField, Tooltip("Upwards speed")] 
    private float upwardSpeed;
    [SerializeField, Tooltip("Slam cooldown")]
    private float cooldown;

    private Vector3 restingPosition;
    private State currentState;
    
    private void Start()
    {
        restingPosition = transform.position;
        currentState = State.Fall;
    }
    
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Fall:
            {
                float step = downwardSpeed * Time.fixedDeltaTime;
                transform.position += (Vector3.down * step);
                break;
            }
            case State.Reset:
            {
                HandleReset();
                break;
            }
            case State.Rest:
            default:
            {
                break;
            }
        }
    }

    private void HandleReset()
    {
        float step = upwardSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, restingPosition, step);
        if (transform.position != restingPosition)
        {
            return;
        }

        currentState = State.Rest;
        _ = StartCoroutine(HandleSlamCooldown());
    }
    private IEnumerator HandleSlamCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        currentState = State.Fall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            currentState = State.Reset;
        }
    }
}

