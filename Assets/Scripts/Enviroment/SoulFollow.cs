using UnityEngine;

public class SoulFollow : MonoBehaviour
{
    private Transform target;
    public float moveSpeed = 8f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

       
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
