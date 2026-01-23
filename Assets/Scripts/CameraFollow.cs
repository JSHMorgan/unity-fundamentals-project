using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform target;
    [SerializeField, Tooltip("Offset distance in the x & y directions.")] 
    private Vector2 offset = new(0f, 0f);
    [SerializeField, Tooltip("Smoothing time for camera follow in seconds.")] 
    private float smoothTime = 0.15f;

    [Header("Vertical Clamping")]
    [SerializeField, Tooltip("Set a minimum camera value in the Y direction.")]
    private float yMinClamp = 0;
    [SerializeField, Tooltip("Set a maximum camera value in the Y direction.")]
    private float yMaxClamp = 10;

    [Header("Horizontal Clamping")]
    [SerializeField, Tooltip("Set a minimum camera value in the X direction.")]
    private float xMinClamp = 0;
    [SerializeField, Tooltip("Set a maximum camera value in the X direction.")] 
    private float xMaxClamp = 10;
    private Vector3 velocity;


    // Update is called once per frame
    private void Update()
    {
        Vector3 targetPos = target.position;
        targetPos.x = Mathf.Clamp(targetPos.x + offset.x, xMinClamp, xMaxClamp);
        targetPos.y = Mathf.Clamp(targetPos.y + offset.y, yMinClamp, yMaxClamp);
        targetPos.z = transform.position.z;
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
