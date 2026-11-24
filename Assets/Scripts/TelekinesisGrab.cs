using UnityEngine;
using UnityEngine.InputSystem;

public class Telekinesis : MonoBehaviour
{
    //Input Actions
    public InputActionReference grabAction;
    public InputActionReference scrollAction;
    public InputActionReference throwAction;
    public InputActionReference rotateAction;
    public InputActionReference lookAction; 

    //Nastavení
    public Transform cameraTransform;
    public float grabDistance = 5f;
    public float holdDistance = 2.5f;
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 5f;
    public float scrollSpeed = 0.01f;
    public float pullSpeed = 10f;
    public float throwForce = 12f;
    public float rotateSpeed = 0.5f;
    public float heldAngularDamping = 10f;
    private float originalAngularDamping;

    private Rigidbody heldObject; // rigidbody drženého objektu

    // Pomocná vlastnost, aby SimplePlayer věděl, kdy se otáčí předmět a ne kamera
    public bool IsRotatingObject => heldObject != null && rotateAction != null && rotateAction.action.IsPressed();

    void OnEnable()
    {
        grabAction?.action.Enable();
        scrollAction?.action.Enable();
        throwAction?.action.Enable();
        rotateAction?.action.Enable();
        lookAction?.action.Enable();
    }

    void OnDisable()
    {
        grabAction?.action.Disable();
        scrollAction?.action.Disable();
        throwAction?.action.Disable();
        rotateAction?.action.Disable();
        lookAction?.action.Disable();
    }

    void Update()
    {
        // Chytání předmětu
        if (grabAction != null && grabAction.action.WasPressedThisFrame())
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, grabDistance)) //zatím neviditelný "paprsek" který zajišťuje míření na danný objekt, který si hráč přeje zvednout --> vlastnosti uloží pod promněnou hit
            {
                if (hit.rigidbody != null) //pokud něco oporavdu drží pokračuje
                {
                    heldObject = hit.rigidbody;
                    heldObject.useGravity = false; //zruší gravitaci aby hráč mohl předmět bezproblémů zvednout
                    holdDistance = Mathf.Clamp(hit.distance, minHoldDistance, maxHoldDistance); // udržuje objekt před hráčem v max a min vzdálenosti

                    //při chycení objektu se objektu omezí rotace (!Zatím nechám být. nevím jestli se mi to tak zamlouvá.!)
                    /*originalAngularDamping = heldObject.angularDamping; //uložení původního omezení rotace
                    heldObject.angularDamping = heldAngularDamping; //nastaví to na 10f --> objekt se nebude protáčet*/


                }
            }
        }

        // Puštění předmětu
        if (grabAction != null && grabAction.action.WasReleasedThisFrame() && heldObject != null)
        {
            heldObject.useGravity = true;
            //heldObject.angularDamping = originalAngularDamping;
            heldObject = null;
            
        }

        // Přibližování / oddalování kolečkem
        if (heldObject != null && scrollAction != null)
        {
            float scrollY = scrollAction.action.ReadValue<Vector2>().y;
            holdDistance += scrollY * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }

        // Házení drženého předmětu
        if (heldObject != null && throwAction != null && throwAction.action.WasPressedThisFrame())
        {
            
            Rigidbody thrown = heldObject;
            thrown.useGravity = true;
            //heldObject.angularDamping = originalAngularDamping;

            heldObject = null;

            // Vystřelí předmět směrem, kam míří kamera
            thrown.linearVelocity = cameraTransform.forward * throwForce;
        }

        // Otáčení drženého předmětu
        if (IsRotatingObject && lookAction != null)
        {
            Vector2 lookDelta = lookAction.action.ReadValue<Vector2>();
            heldObject.transform.Rotate(cameraTransform.up, -lookDelta.x * rotateSpeed, Space.World);
            heldObject.transform.Rotate(cameraTransform.right, lookDelta.y * rotateSpeed, Space.World);

        }
    }

    void FixedUpdate()
    {
        if (heldObject == null) return;

        if (IsRotatingObject)
        {
            heldObject.angularVelocity = Vector3.zero;
        }

        Vector3 targetPoint = cameraTransform.position + cameraTransform.forward * holdDistance;
        heldObject.linearVelocity = (targetPoint - heldObject.position) * pullSpeed;

        
    }
}