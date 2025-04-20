using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;

    [Space]

    [SerializeField] float sensitivity;
    [SerializeField] float scrollSensitivity;
    [SerializeField] float offset;
    [SerializeField] Vector2 xRotationClamp;
    [SerializeField] Vector2 offsetClamp;

    [Space]

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    
    [Space]
    
    [SerializeField] float xRotation;
    [SerializeField] float yRotation;

    private void Start()
    {
        // Initialise position
        transform.localEulerAngles = new Vector3(xRotation, yRotation, 0f);
        transform.position = target.position - transform.forward * offset;
    }

    private void LateUpdate()
    {
        // Mouse button is held down
        if (Input.GetMouseButton(2))
        {
            // Disable cursor
            Cursor.lockState = CursorLockMode.Locked;

            // Store mouse X and Y axes
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            // Update X and Y rotation
            yRotation += invertX ? -mouseX : mouseX;
            xRotation += invertY ? mouseY : -mouseY;
            xRotation = Mathf.Clamp(xRotation, xRotationClamp.x, xRotationClamp.y);

            // Apply X and Y rotation to camera
            transform.localEulerAngles = new Vector3(xRotation, yRotation, 0f);
        }

        // Mouse button is released
        if (Input.GetMouseButtonUp(2))
        {
            // Enable cursor
            Cursor.lockState = CursorLockMode.None;
        }
        
        // Update distance from the camera to the target
        offset += -Input.mouseScrollDelta.y * scrollSensitivity;
        offset = Mathf.Clamp(offset, offsetClamp.x, offsetClamp.y);

        // Apply offset position to the camera
        transform.position = target.position - transform.forward * offset;
    }
}
