using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Elements")]
    public Image buttonImage;
    public Text buttonText;
    
    [Header("Colors")]
    public Color normalColor = Color.green;
    public Color pressedColor = Color.red;
    public string dropText = "DROP";
    public string droppingText = "READY";
    
    void Start()
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
        
        if (buttonText != null)
            buttonText.text = dropText;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        
        if (buttonImage != null)
            buttonImage.color = pressedColor;
        
        if (buttonText != null)
            buttonText.text = droppingText;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
        if (spawner != null && GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            spawner.DropCurrentShape();
        }
        
        if (buttonImage != null)
            buttonImage.color = normalColor;
        
        if (buttonText != null)
            buttonText.text = dropText;
    }
}
