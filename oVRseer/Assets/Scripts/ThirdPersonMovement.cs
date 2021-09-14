using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public Vector3 velocity;
    public bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        // Check if player is on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
 
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            // Calculate how player should turn
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg+ cam.eulerAngles.y;
            // Smooth the turn
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // Rotate the player
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calculate the new direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Move the player in the xz plane
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if(Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        // Move in y
        controller.Move(velocity * Time.deltaTime);
    }
}
