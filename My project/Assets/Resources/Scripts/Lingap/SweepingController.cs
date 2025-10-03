using UnityEngine;

public class SweepingController : MonoBehaviour
{
        [SerializeField] private string taskID; // optional if you still want to track via TaskManager

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;             // normal gravity so it sits on the invisible floor
        rb.mass = 1.5f;                   // not too light, not too heavy
        rb.linearDamping = 1.5f;     // adds sliding resistance after broom pushes
        rb.angularDamping = 5f;           // stops spinning
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
