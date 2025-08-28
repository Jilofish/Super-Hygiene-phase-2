using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ArrangeUtensils : MonoBehaviour
{
    [Header("Task & Object Type")]
    [SerializeField] private string taskID;
    [SerializeField] private TapObjectType objectType;

    [Header("Movement")]
    [SerializeField] private Transform targetPosition; // drag Plate 1 here
    [SerializeField] private float moveSpeed = 5f;

    [Header("Wrong Order Feedback")]
    [SerializeField] private float tiltAngle = 15f;      // how much to tilt
    [SerializeField] private float tiltSpeed = 8f;       // how fast the tilt animates
    [SerializeField] private int tiltCount = 2;          // number of tilts

    private Camera cam;
    private bool isTilting = false;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            HandleTouch();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            HandleMouse();
        }
    }

    void HandleMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
                TryDoAction();
        }
    }

    void HandleTouch()
    {
        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = cam.ScreenToWorldPoint(touch.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
                TryDoAction();
        }
    }

    void TryDoAction()
    {
        if (ArrangementSequenceManager.Instance.CanTap(objectType))
        {
            if (!string.IsNullOrEmpty(taskID))
                TaskManager.Instance.IncrementTask(taskID);

            ArrangementSequenceManager.Instance.AdvanceStepIfNeeded(objectType);
            StartCoroutine(MoveToTarget());
        }
        else
        {
            Debug.LogWarning($"Tapped out of order: {objectType}");
            if (!isTilting)
                StartCoroutine(TiltFeedback());
        }
    }

    private IEnumerator MoveToTarget()
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("Target position is missing for " + gameObject.name);
            yield break;
        }

        Vector3 start = transform.position;
        Vector3 end = targetPosition.position;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end; // snap exactly to final
    }

    private IEnumerator TiltFeedback()
    {
        isTilting = true;
        Quaternion originalRot = transform.rotation;

        for (int i = 0; i < tiltCount; i++)
        {
            // tilt right
            yield return RotateTo(Quaternion.Euler(0, 0, tiltAngle));
            // tilt left
            yield return RotateTo(Quaternion.Euler(0, 0, -tiltAngle));
        }

        // reset
        yield return RotateTo(originalRot);

        isTilting = false;
    }

    private IEnumerator RotateTo(Quaternion targetRot)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * tiltSpeed;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
