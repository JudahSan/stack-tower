using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float rotationSensitivity = 0.5f;
    
    private Vector2 touchStartPosition;
    private bool isRotating = false;
    private Transform currentShape;
    
    void Update()
    {
        FindCurrentShape();
    }
    
    void FindCurrentShape()
    {
        if (currentShape != null) return;
        
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("Shape");
        foreach (GameObject shape in shapes)
        {
            ShapeController controller = shape.GetComponent<ShapeController>();
            if (controller != null && !controller.IsDropped)
            {
                currentShape = shape.transform;
                break;
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        
        isRotating = true;
        touchStartPosition = eventData.position;
        FindCurrentShape();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isRotating || currentShape == null || GameManager.Instance.IsGameOver) return;
        
        Vector2 currentPosition = eventData.position;
        float deltaX = (currentPosition.x - touchStartPosition.x) * rotationSensitivity;
        
        currentShape.Rotate(0, 0, -deltaX);
        touchStartPosition = currentPosition;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isRotating = false;
    }
}
