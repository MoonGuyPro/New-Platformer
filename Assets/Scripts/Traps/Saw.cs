using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask detectionLayer;

    [Header("Detection Settings")]
    [SerializeField] private float frontDetectionDistance = 0.5f;
    [SerializeField] private float groundDetectionDistance = 0.5f;

    private int direction = 1; // 1 for right, -1 for left

    private void Update()
    {
        if (ShouldChangeDirection())
        {
            ChangeDirection();
        }

        Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime * direction);
    }

    private bool ShouldChangeDirection()
    {
        // Check for front obstacle
        RaycastHit2D frontHit = Physics2D.Raycast(boxCollider.bounds.center, Vector2.right * direction, frontDetectionDistance, detectionLayer);
        if (frontHit.collider != null)
        {
            return true; // Front obstacle detected
        }

        // Wysyłanie Raycasta w kierunku ruchu, ale nieco niżej
        Vector2 raycastStartPoint = (Vector2)boxCollider.bounds.center + new Vector2(direction * boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y);
        RaycastHit2D groundHit = Physics2D.Raycast(raycastStartPoint, Vector2.down, groundDetectionDistance, detectionLayer);

        // Jeśli nie ma terenu przed obiektem, zmienia kierunek
        if (groundHit.collider == null)
        {
            return true;
        }

        return false;
    }

    private void ChangeDirection()
    {
        direction *= -1;
        // Optional: Flip the GameObject to face the new direction
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Visualize the detection rays in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(boxCollider.bounds.center, boxCollider.bounds.center + Vector3.right * direction * frontDetectionDistance);
        // Wizualizacja Raycasta sprawdzającego teren przed obiektem
        Gizmos.color = Color.blue;
        Vector2 raycastStartPoint = (Vector2)boxCollider.bounds.center + new Vector2(direction * boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y);
        Gizmos.DrawLine(raycastStartPoint, raycastStartPoint + Vector2.down * groundDetectionDistance);
    }
    
}
