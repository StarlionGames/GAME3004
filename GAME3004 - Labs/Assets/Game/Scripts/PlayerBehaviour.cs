using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Properties")]
    public float maxSpeed = 10.0f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;
    public Vector3 velocity;

    [Header("Ground Detection Properties")]
    public Transform groundPoint;
    public float groundRadius = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded;

    PlayerInput input => GetComponent<PlayerInput>();
    InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveAction = input.actions["Move"];
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundPoint.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -2.0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        float x = input.x;
        float z = input.y;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * maxSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }
    }
}
