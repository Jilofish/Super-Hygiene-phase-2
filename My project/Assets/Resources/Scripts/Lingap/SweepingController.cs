using UnityEngine;

public class SweepingController : MonoBehaviour
{
        [SerializeField] private string taskID; // optional if you still want to track via TaskManager

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0.5f;   // 👈 gives trash a downward pull
        rb.mass = 1.5f;           // heavier so it doesn’t scatter
        rb.linearDamping = 2f;             // slows down sideways movement
        rb.angularDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.tag = "Trash";
}

    // Called by the TrashPile when trash enters the area
    public void RemoveTrash()
    {
        if (!string.IsNullOrEmpty(taskID))
        {
            TaskManager.Instance.IncrementTask(taskID);
        }

        Destroy(gameObject);
    }
}
