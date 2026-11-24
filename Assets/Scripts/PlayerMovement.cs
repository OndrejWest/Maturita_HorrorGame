using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;

    public Transform cameraTransform;
    public float moveSpeed = 6f;
    public float mouseSensitivity = 0.15f;
    public float gravity = -20f;

    private CharacterController controller;
    private float pitch = 0f;
    private float verticalSpeed = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();
    }

    void Update()
    {
        // 1. Rozhlížení myší
        Vector2 look = lookAction.action.ReadValue<Vector2>() * mouseSensitivity;
        transform.Rotate(0f, look.x, 0f); // otáčení hráče doleva/doprava

        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f); // náklon kamery nahoru/dolů

        // 2. Pohyb WASD
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = transform.right * move.x + transform.forward * move.y;

        // 3. Gravitace
        if (controller.isGrounded)
            verticalSpeed = -2f;
        else
            verticalSpeed += gravity * Time.deltaTime;

        Vector3 finalMove = direction * moveSpeed + Vector3.up * verticalSpeed;
        controller.Move(finalMove * Time.deltaTime);
    }
}