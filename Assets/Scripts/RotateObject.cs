using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Rotation speed for each axis (X, Y, Z) in degrees per second
    [Header("Rotation Speeds (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Example: Rotate only around the Y axis

    void Update()
    {
        // Rotate the object based on the defined speed and deltaTime
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
