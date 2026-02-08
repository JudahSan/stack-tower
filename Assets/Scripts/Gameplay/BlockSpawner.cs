using System.Collections;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spawnPoint;
    
    private GameObject currentShape;
    private ShapeController shapeController;
    private float blockSpeed = 8f;
    private int direction = 1;
    private bool canDrop = true;
    private bool isSpawning = false;
    
    void Update()
    {
        if (!isSpawning || GameManager.Instance.IsGameOver || currentShape == null) return;
        
        HandleHorizontalMovement();
        
        // Desktop input for testing
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space) && canDrop)
        {
            DropCurrentShape();
        }
        #endif
    }
    
    void HandleHorizontalMovement()
    {
        if (shapeController == null || shapeController.IsDropped) return;
        
        // Horizontal movement (auto-bounce)
        float moveAmount = Time.deltaTime * blockSpeed * direction;
        currentShape.transform.position += new Vector3(moveAmount, 0, 0);
        
        // Bounce at limits
        if (Mathf.Abs(currentShape.transform.position.x) > GameManager.Instance.xLimit)
        {
            currentShape.transform.position = new Vector3(
                direction * GameManager.Instance.xLimit, 
                spawnPoint.position.y, 
                0
            );
            direction = -direction;
        }
    }
    
    public void StartSpawning()
    {
        isSpawning = true;
        blockSpeed = 8f; // Reset speed
        SpawnNewShape();
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
    }
    
    void SpawnNewShape()
    {
        if (!isSpawning) return;
        
        // Get shape from pool
        currentShape = GetComponent<PoolManager>().GetShape(
            GameManager.Instance.currentShapeType,
            spawnPoint.position
        );
        
        if (currentShape != null)
        {
            shapeController = currentShape.GetComponent<ShapeController>();
            if (shapeController != null)
            {
                shapeController.Initialize();
            }
            
            // Random starting rotation
            currentShape.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            
            // Increase difficulty
            blockSpeed += 0.5f;
        }
    }
    
    // Called by Drop Button
    public void DropCurrentShape()
    {
        if (currentShape == null || !canDrop) return;
        if (shapeController != null && shapeController.IsDropped) return;
        
        canDrop = false;
        
        if (shapeController != null)
        {
            shapeController.Drop();
        }
        
        GameManager.Instance.AddScore(10);
        
        // Spawn next shape after delay
        StartCoroutine(SpawnNextShape());
    }
    
    IEnumerator SpawnNextShape()
    {
        yield return new WaitForSeconds(GameManager.Instance.dropCooldown);
        
        canDrop = true;
        if (isSpawning && !GameManager.Instance.IsGameOver)
        {
            SpawnNewShape();
        }
    }
}
