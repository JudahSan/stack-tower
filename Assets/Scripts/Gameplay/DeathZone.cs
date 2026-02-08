using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Settings")]
    public float fallThreshold = -6f;
    
    void Update()
    {
        CheckForFallenShapes();
    }
    
    void CheckForFallenShapes()
    {
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("Shape");
        foreach (GameObject shape in shapes)
        {
            if (shape.transform.position.y < fallThreshold)
            {
                TriggerGameOver();
                return;
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shape"))
        {
            TriggerGameOver();
        }
    }
    
    void TriggerGameOver()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }
}
