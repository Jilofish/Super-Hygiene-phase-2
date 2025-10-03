using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BroomController : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;

    private bool isDragging = false;
    private Vector2 dragOffset;
    private Vector2 startPos;  // original position

    [SerializeField] private float returnSpeed = 5f; // how fast it goes back

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = transform.position; // remember initial position
    }

    private void Update()
    {
        // Mouse input
        if (Mouse.current != null)
        {
            Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
                if (hit != null && hit.gameObject == gameObject)
                {
                    isDragging = true;
                    dragOffset = (Vector2)transform.position - mouseWorld;
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }
        }

        // Touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            var touch = Touchscreen.current.primaryTouch;
            Vector2 touchWorld = cam.ScreenToWorldPoint(touch.position.ReadValue());

            if (touch.press.wasPressedThisFrame)
            {
                Collider2D hit = Physics2D.OverlapPoint(touchWorld);
                if (hit != null && hit.gameObject == gameObject)
                {
                    isDragging = true;
                    dragOffset = (Vector2)transform.position - touchWorld;
                }
            }

            if (touch.press.wasReleasedThisFrame)
            {
                isDragging = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            Vector2 inputPos = rb.position; 

            if (Mouse.current != null && Mouse.current.leftButton != null && Mouse.current.leftButton.isPressed)
            {
                inputPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                inputPos = cam.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            }

            rb.MovePosition(inputPos + dragOffset);
        }
        else
        {
            rb.MovePosition(Vector2.Lerp(rb.position, startPos, returnSpeed * Time.fixedDeltaTime));
        }
    }
}
