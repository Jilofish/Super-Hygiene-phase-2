using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class DirtEraserHandler : MonoBehaviour
{
    [Header("Task Settings")]
    [SerializeField] private string taskID;

    [Header("Brush Erase Settings")]
    public RenderTexture maskTexture;
    public Texture2D brushTexture;
    public Material eraseMaterial;

    private Camera cam;
    private SpriteRenderer spriteRenderer;
    private bool isErasing = false;

    private bool hasCompleted = false;

    void Awake()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Clear the mask texture to black at the beginning
        RenderTexture.active = maskTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
    void Update()
    {
        if (!isErasing)
        {
            // Detect first touch or click on this object
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPos = cam.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
                TryStartErasing(touchPos);
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                TryStartErasing(mousePos);
            }
        }
        else
        {
            // Continue drawing brush only if actively touching/clicking
            Vector2? input = GetActiveInputPosition();
            if (input.HasValue)
            {
                Vector2 worldPos = cam.ScreenToWorldPoint(input.Value);
                Vector2 uv = WorldToUV(worldPos);

                if (uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1)
                {
                    DrawBrush(uv);
                }
            }

            // Optionally check for full erase (not required, can destroy manually)
            if (!hasCompleted && CheckEraseThreshold(0.70f)) // 90% erased
            {
                DoAction();
            }
        }
    }

    void TryStartErasing(Vector2 worldPos)
    {
        // Draw a small ray for 1 second for visualization
        Debug.DrawRay(worldPos, Vector2.up * 0.1f, Color.red, 1f);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            isErasing = true;
        }
    }

    Vector2? GetActiveInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return Mouse.current.position.ReadValue();

        return null;
    }

    void DrawBrush(Vector2 uv)
    {
        if (brushTexture == null || eraseMaterial == null)
        {
            return;
        }

        // Temporary RT (copy current mask)
        RenderTexture temp = RenderTexture.GetTemporary(maskTexture.width, maskTexture.height, 0, maskTexture.format);
        Graphics.Blit(maskTexture, temp);

        // Pass brush data to material
        eraseMaterial.SetTexture("_BrushTex", brushTexture);

        // BrushUV = (center, sizeRatio)
        float margin = 1.2f; // scale brush 20% larger
        float brushScaleX = brushTexture.width / (float)maskTexture.width;
        float brushScaleY = brushTexture.height / (float)maskTexture.height;
        eraseMaterial.SetVector("_BrushUV", new Vector4(uv.x, uv.y, brushScaleX, brushScaleY));

        // Blend into mask
        Graphics.Blit(temp, maskTexture, eraseMaterial);
        RenderTexture.ReleaseTemporary(temp);

    }



    Vector2 WorldToUV(Vector2 worldPos)
    {
        Vector2 local = transform.InverseTransformPoint(worldPos);
        Sprite sprite = spriteRenderer.sprite;
        Rect rect = sprite.rect;
        float ppu = sprite.pixelsPerUnit;
        Vector2 size = rect.size / ppu;

        float uvX = (local.x + size.x / 2f) / size.x;
        float uvY = (local.y + size.y / 2f) / size.y;

        return new Vector2(uvX, uvY);
    }

    void DoAction()
    {
        hasCompleted = true;

        Debug.Log($"[Action] Task complete for ID: {taskID}");

        if (!string.IsNullOrEmpty(taskID))
        {
            TaskManager.Instance.IncrementTask(taskID);
            Debug.Log($"[TaskManager] Incremented task: {taskID}");
        }
        else
        {
            Debug.LogWarning("[Action] Task ID was empty.");
        }

        Destroy(gameObject);
    }

    // Optional: Check how much of the mask has been erased (white)
    bool CheckEraseThreshold(float threshold)
    {
        Texture2D readTex = new Texture2D(maskTexture.width, maskTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = maskTexture;
        readTex.ReadPixels(new Rect(0, 0, maskTexture.width, maskTexture.height), 0, 0);
        readTex.Apply();
        RenderTexture.active = null;

        Color[] pixels = readTex.GetPixels();

        float whitePixels = 0;

        foreach (Color c in pixels)
        {
            if (c.r > 0.5f) whitePixels += 1f;
        }

        float percentWhite = whitePixels / pixels.Length;
        Destroy(readTex);


        return percentWhite >= threshold;
    }
}
