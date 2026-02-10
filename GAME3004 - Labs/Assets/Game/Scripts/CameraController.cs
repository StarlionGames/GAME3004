using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 1f;
    public Transform playerBody;

    private float XRotation = 0.0f;

    PlayerInput p_Input;
    InputAction mousePos;

    // Start is called before the first frame update
    void Start()
    {
        p_Input = GetComponentInParent<PlayerInput>();
        mousePos = p_Input.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseBase = mousePos.ReadValue<Vector2>();
        
        float mouseX = mouseBase.x * mouseSensitivity;
        float mouseY = mouseBase.y * mouseSensitivity;

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90.0f, 90.0f);

        transform.localRotation = Quaternion.Euler(XRotation, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
