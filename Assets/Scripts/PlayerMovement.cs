using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference sprintAction;
    public Telekinesis telekinesis;

    public Transform cameraTransform;
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    private float currentSpeed;
    public float mouseSensitivity = 0.15f;
    public float gravity = -20f;

    //Stamina
    public float maxStamina = 100f;
    public float staminaDrainPerSec = 25f;
    public float staminaRegenPerSec = 15f;
    public float minStaminaToSprint = 20f; 

    private CharacterController controller;
    private float pitch = 0f;
    private float verticalSpeed = 0f;
    private float stamina;
    private bool exhausted = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();
        sprintAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();
        sprintAction?.action.Disable();
    }

    void Update()
    {
        //Kamera
        if (telekinesis == null || !telekinesis.IsRotatingObject)
        {
            Vector2 look = lookAction.action.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(0f, look.x, 0f);

            pitch -= look.y;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        //Pohyb WASD
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }


        //!Logika staminy!
        bool wantsToSprint = sprintAction != null && sprintAction.action.IsPressed();
        bool isMoving = direction.sqrMagnitude > 0.01f;
            // Can only sprint if not exhausted and stamina is above 0
        bool isSprinting = wantsToSprint && isMoving && controller.isGrounded && !exhausted && stamina > 0f;


        //Vybíjení staminy
        if (isSprinting)
        {
            Debug.Log("běží!!!...");
            stamina -= staminaDrainPerSec * Time.deltaTime;
            if (stamina <= 0f)
            {
                Debug.Log("Unavil se...");
                stamina = 0f;
                exhausted = true; // spustí cooldown
            }
        }
        //Nabíjení staminy
        else
        {
            Debug.Log("Dobíjení...");
            stamina += staminaRegenPerSec * Time.deltaTime;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }

            //zruší stav Exhousted když se stamina nabije na minimální hodnotu kdy je možné běhat
            if (exhausted && stamina >= minStaminaToSprint)
            {
                exhausted = false;
                Debug.Log("Už není vyčerpán...");
            }
        }

        //určení rychlosti chůze
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else 
        {
            currentSpeed = moveSpeed;
        }


        //Auomatiiké určení jestli se hráč dotýká země
        if (controller.isGrounded)
        {
            verticalSpeed = -2f; //hráč je konstantně přitlačován k zemi
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime; //pád
        }

        Vector3 finalMove = direction * currentSpeed + Vector3.up * verticalSpeed;
        controller.Move(finalMove * Time.deltaTime);
    }
}