using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;

    [Header("Settings")]
    public float blockSpeed = 8f;
    public float speedIncrease = 0.5f;
    public int direction = 1;

    private GameObject currentShape;
    private ShapeController shapeController;
    private bool canDrop = true;
    private bool isSpawning = false;

    void Update()
    {
        if (!isSpawning || GameManager.Instance == null || GameManager.Instance.IsGameOver) return;

        if (currentShape != null && !IsShapeDropped())
        {
            HandleMovement();

            // Desktop input for testing
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && canDrop)
            {
                DropCurrentShape();
            }
        }
    }

    void HandleMovement()
    {
        float moveAmount = Time.deltaTime * blockSpeed * direction;
        currentShape.transform.position += new Vector3(moveAmount, 0, 0);

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

    bool IsShapeDropped()
    {
        return shapeController != null && shapeController.IsDropped;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        blockSpeed = 8f;
        direction = 1;
        SpawnNewShape();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    void SpawnNewShape()
    {
        if (!isSpawning || GameManager.Instance.IsGameOver) return;

        PoolManager poolManager = GetComponent<PoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("PoolManager not found!");
            return;
        }

        // Get RANDOM shape from GameManager
        ShapeType randomShape = GameManager.Instance.GetRandomShape();

        Debug.Log($"Spawning random shape: {randomShape}");

        currentShape = poolManager.GetShape(randomShape, spawnPoint.position);

        if (currentShape != null)
        {
            shapeController = currentShape.GetComponent<ShapeController>();
            if (shapeController != null)
            {
                shapeController.Initialize();
            }

            // Random rotation
            currentShape.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            // Increase difficulty
            blockSpeed += speedIncrease;
        }
        else
        {
            Debug.LogError($"Failed to spawn shape: {randomShape}");
        }
    }

    public void DropCurrentShape()
    {
        if (currentShape == null || !canDrop || IsShapeDropped()) return;

        canDrop = false;

        if (shapeController != null)
        {
            shapeController.Drop();
        }

        GameManager.Instance.AddScore(10);

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
