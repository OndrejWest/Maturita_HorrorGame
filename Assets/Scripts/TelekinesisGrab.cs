using UnityEngine;
using UnityEngine.InputSystem;

public class Telekinesis : MonoBehaviour
{
    public InputActionReference grabAction;
    public InputActionReference scrollAction;
    public Transform cameraTransform;

    public float grabDistance = 5f;
    public float holdDistance = 2.5f;
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 5f;
    public float scrollSpeed = 0.01f;
    public float pullSpeed = 10f;

    private Rigidbody heldObject;

    void OnEnable()
    {
        grabAction?.action.Enable();
        scrollAction?.action.Enable();
    }

    void OnDisable()
    {
        grabAction?.action.Disable();
        scrollAction?.action.Disable();
    }

    void Update()
    {
        // 1. Chycení předmětu
        if (grabAction.action.WasPressedThisFrame())
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, grabDistance))
            {
                if (hit.rigidbody != null)
                {
                    heldObject = hit.rigidbody;
                    heldObject.useGravity = false;
                    // Volitelně: začni držet přesně tam, kde jsi předmět chytil (v rámci povoleného rozsahu)
                    holdDistance = Mathf.Clamp(hit.distance, minHoldDistance, maxHoldDistance);
                }
            }
        }

        // 2. Puštění předmětu
        if (grabAction.action.WasReleasedThisFrame() && heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject = null;
        }

        // 3. Přibližování / oddalování jen když držíš předmět
        if (heldObject != null && scrollAction != null)
        {
            float scrollY = scrollAction.action.ReadValue<Vector2>().y;
            holdDistance += scrollY * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }
    }

    void FixedUpdate()
    {
        if (heldObject == null) return;

        Vector3 targetPoint = cameraTransform.position + cameraTransform.forward * holdDistance;
        heldObject.linearVelocity = (targetPoint - heldObject.position) * pullSpeed;
    }
}