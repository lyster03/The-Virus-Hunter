using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private Transform target;
    private float rotationSpeed;
    private float moveSpeed;
    private Rigidbody2D rb;

    public void Initialize(Transform target, float rotationSpeed, float moveSpeed)
    {
        this.target = target;
        this.rotationSpeed = rotationSpeed;
        this.moveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!target || !rb) return;

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotationSpeed;
        rb.velocity = transform.up * moveSpeed;
    }
}