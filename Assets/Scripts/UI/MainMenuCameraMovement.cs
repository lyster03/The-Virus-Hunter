using UnityEngine;

public class MainMenuCameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float sensitivity = 0.5f;   
    public float maxOffset = 1f;       
    public float smoothSpeed = 5f;     

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Vector3 offset = new Vector3(mouseX * sensitivity, mouseY * sensitivity, 0f);
        offset = Vector3.ClampMagnitude(offset, maxOffset);

        targetPosition = startPosition + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
