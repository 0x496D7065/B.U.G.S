using UnityEngine;

public class WeaponKick : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 initialRotation;

    private Vector3 targetPosition;
    private Vector3 targetRotation;

    private Vector3 currentPosition;
    private Vector3 currentRotation;

    [Header("Kick Settings")]
    [SerializeField] private float kickBackZ;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float snappiness;
    [Header("Rotation Kick Settings")]
    [SerializeField] private float kickRotationX; // Upward tilt
    [SerializeField] private float kickRotationY; // Horizontal wobble
    [SerializeField] private float kickRotationZ; // Side tilt
    [SerializeField] private float rotationReturnSpeed;
    [SerializeField] private float rotationSnappiness;


    void Awake()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Slerp(currentPosition, targetPosition, snappiness * Time.deltaTime);
        transform.localPosition = initialPosition + currentPosition;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, rotationReturnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, rotationSnappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(initialRotation + currentRotation);
    }

    public void PlayKick()
    {
        targetPosition += new Vector3(0, 0, kickBackZ);
        //Debug.Log("kick");
        targetRotation += new Vector3(kickRotationX, Random.Range(-kickRotationY, kickRotationY), Random.Range(-kickRotationZ, kickRotationZ));
    }
}
