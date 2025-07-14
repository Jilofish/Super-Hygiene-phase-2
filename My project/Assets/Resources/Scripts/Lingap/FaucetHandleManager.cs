using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class FaucetHandleManager : MonoBehaviour
{
    [Header("Scene Transitions")]
    public GameObject currentArea;        // Example: Area_1
    public GameObject faucetMinigame;     // World-space faucet screen with handle, etc.
    public GameObject AreaUI;          // UI Canvas for the area

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Touch Input
        if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            TryActivate(pos);
        }
        // Mouse Input
        else if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            TryActivate(pos);
        }
    }

    void TryActivate(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (currentArea != null) currentArea.SetActive(false);
            if (faucetMinigame != null) faucetMinigame.SetActive(true);
            if (AreaUI != null) AreaUI.SetActive(false);

            // Optional: destroy trigger to prevent reuse
            Destroy(gameObject);
        }
    }
}
