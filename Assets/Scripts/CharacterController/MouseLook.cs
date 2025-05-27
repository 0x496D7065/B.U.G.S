using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody; // Assign CameraHolder or PlayerRoot here
    public float mouseSensitivity = 125f;

    private float xRotation = 0f;

    void Start()
    {
        LockCursor(true);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent flipping

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical look
        playerBody.Rotate(Vector3.up * mouseX); // Horizontal look
    }

    public void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
