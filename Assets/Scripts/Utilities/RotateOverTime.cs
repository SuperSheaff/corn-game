using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
