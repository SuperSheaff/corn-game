using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    public float amplitude = 0.5f;   // Height of the float
    public float frequency = 1f;     // Speed of the float

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0f, yOffset, 0f);
    }
}
