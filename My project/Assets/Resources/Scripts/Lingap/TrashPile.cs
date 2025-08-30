using UnityEngine;

public class TrashPile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash"))
        {
            SweepingController trash = other.GetComponent<SweepingController>();
            if (trash != null)
            {
                trash.RemoveTrash();
            }
        }
    }
}