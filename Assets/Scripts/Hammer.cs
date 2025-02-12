using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float rotationAmount = 45f;
    [SerializeField] private float startOffset = 0f;
    [SerializeField] private float smoothFactor = 5f;
    private float startRotation;
    private float targetRotation;

    void Start()
    {
        startRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        float time = Mathf.Sin((Time.time + startOffset) * rotationSpeed) * 0.5f + 0.5f; // Eased oscillation
        float angle = Mathf.Lerp(-rotationAmount, rotationAmount, time);
        targetRotation = startRotation + angle;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * smoothFactor);
    }
}

