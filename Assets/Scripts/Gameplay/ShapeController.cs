using UnityEngine;

public class ShapeController : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb2D;
    
    [Header("Settings")]
    public Color shapeColor = Color.red;
    public float scale = 1f;
    
    public bool IsDropped { get; private set; }
    
    void Start()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = shapeColor;
        
        transform.localScale = Vector3.one * scale;
    }
    
    public void Initialize()
    {
        IsDropped = false;
        
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            rb2D.simulated = false;
            rb2D.gravityScale = 2f;
        }
    }
    
    public void Drop()
    {
        IsDropped = true;
        
        if (rb2D != null)
        {
            rb2D.simulated = true;
        }
    }
    
    // Desktop rotation
    void OnMouseDrag()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (!IsDropped)
        {
            float rotation = Input.GetAxis("Mouse X") * 100f;
            transform.Rotate(0, 0, -rotation);
        }
        #endif
    }
}
