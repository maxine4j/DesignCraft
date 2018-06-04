using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Animator animator;
    private CameraController cameraController;

    public float movementSpeed = 1.0f;
    public float turnSpeed = 1.0f;

    private const float BASE_MOVESPEED_MOD = 0.3f;
    private const float BASE_TURNSPEED_MOD = 3.0f;
    private const float BASE_BACKPEDDLE_MOD = 0.1f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        cameraController = GetComponentInChildren<CameraController>();
    }

    void FixedUpdate()
    {
        Vector3 finalDelta = Vector3.zero;
        // running
        if (Input.GetKey(KeyCode.W))
        {
            finalDelta = transform.forward * movementSpeed * BASE_MOVESPEED_MOD;
            if (animator)
                animator.SetBool("Running", true);
        }
        else
        {
            if (animator)
                animator.SetBool("Running", false);
        }
        // back peddling
        if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        {
            finalDelta = transform.forward * -BASE_BACKPEDDLE_MOD;
            if (animator)
                animator.SetBool("BackPeddling", true);
        }
        else
        {
            if (animator)
                animator.SetBool("BackPeddling", false);
        }
        // turning and strafing
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetMouseButton(1))
                finalDelta += -transform.right * movementSpeed * BASE_MOVESPEED_MOD;
            else
                transform.Rotate(Vector3.up * -turnSpeed * BASE_TURNSPEED_MOD);
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetMouseButton(1))
                finalDelta += transform.right * movementSpeed * BASE_MOVESPEED_MOD;
            else
                transform.Rotate(Vector3.up * turnSpeed * BASE_TURNSPEED_MOD);
        }

        // clamp the final delta to our movespeed
        finalDelta = Vector3.ClampMagnitude(finalDelta, movementSpeed * BASE_MOVESPEED_MOD);
        // if we are backpeddling clamp it to that slower speed
        if (Input.GetKey(KeyCode.S))
            finalDelta = Vector3.ClampMagnitude(finalDelta, BASE_BACKPEDDLE_MOD);
        transform.position += finalDelta;
    }
}
