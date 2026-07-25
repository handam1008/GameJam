using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float height = 10f;
    public float speed = 2f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPosition
            + Vector3.up * Mathf.Sin(Time.time * speed) * height;
    }
}