using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [Header("Objects with Animator components")]
    public GameObject[] objectsToExit;

    public string triggerName = "Exit";

    public void PlayExitAnimations()
    {
        foreach (GameObject obj in objectsToExit)
        {
            Animator animator = obj.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogWarning(obj.name + " has no Animator component!");
            }
        }
    }
}
