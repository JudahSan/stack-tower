using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState { Menu, Playing, GameOver, Paused }
public enum ShapeType { Heart, Star, Circle, Square, Rose, Arrow }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float spawnHeight = 4f;
    public float xLimit = 2.5f;
    public float dropCooldown = 0.5f;
    public float rotationSpeed = 100f;

    [Header("Current Shape")]
    [SerializeField] private ShapeType currentShapeType = ShapeType.Heart;

    [Header("Performance")]
    public int targetFPS = 30;
    public int poolSize = 20;

    // Game state
    public GameState CurrentState { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver => CurrentState == GameState.GameOver;

    // Available shapes for random selection
    private List<ShapeType> availableShapes = new List<ShapeType>();

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    void Initialize()
    {
        // Performance settings
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = 1f / 30f;

        // Initialize available shapes list
        InitializeAvailableShapes();

        SetGameState(GameState.Menu);
    }

    void InitializeAvailableShapes()
    {
        // Start with all possible shapes
        availableShapes = new List<ShapeType>
        {
            ShapeType.Heart,
            ShapeType.Star,
            ShapeType.Circle,
            ShapeType.Square,
            ShapeType.Rose,
            ShapeType.Arrow
        };

        // Verify which shapes actually exist in PoolManager
        PoolManager poolManager = GetComponent<PoolManager>();
        if (poolManager != null && poolManager.shapePools != null)
        {
            List<ShapeType> validShapes = new List<ShapeType>();

            foreach (ShapeType shapeType in availableShapes)
            {
                // Check if this shape exists in PoolManager
                if (poolManager.shapePools.Exists(pool => pool.shapeType == shapeType && pool.prefab != null))
                {
                    validShapes.Add(shapeType);
                }
                else
                {
                    Debug.LogWarning($"Shape {shapeType} not found in PoolManager. It will be excluded.");
                }
            }

            availableShapes = validShapes;

            if (availableShapes.Count == 0)
            {
                Debug.LogError("No valid shapes found in PoolManager! Add at least one shape prefab.");
                availableShapes.Add(ShapeType.Heart); // Fallback
            }
        }

        Debug.Log($"Available shapes: {availableShapes.Count}");
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Menu:
                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null) uiManager.ShowMenu();
                break;

            case GameState.Playing:
                Score = 0;
                uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.ShowGameUI();
                    uiManager.UpdateScore(Score);
                }

                BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
                if (spawner != null) spawner.StartSpawning();
                break;

            case GameState.GameOver:
                uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null) uiManager.ShowGameOver(Score);

                spawner = FindFirstObjectByType<BlockSpawner>();
                if (spawner != null) spawner.StopSpawning();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    public void AddScore(int points)
    {
        Score += points;

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateScore(Score);
        }
    }

    // Get a random shape from available shapes
    public ShapeType GetRandomShape()
    {
        if (availableShapes.Count == 0)
        {
            Debug.LogWarning("No available shapes, returning Heart as fallback");
            return ShapeType.Heart;
        }

        // Get random shape
        int randomIndex = Random.Range(0, availableShapes.Count);
        ShapeType randomShape = availableShapes[randomIndex];

        // Update current shape (optional, for debugging)
        currentShapeType = randomShape;

        return randomShape;
    }

    // Get current shape (for debugging or UI display)
    public ShapeType GetCurrentShape()
    {
        return currentShapeType;
    }

    // Add a shape to available pool (if you want to unlock shapes)
    public void AddAvailableShape(ShapeType shapeType)
    {
        if (!availableShapes.Contains(shapeType))
        {
            availableShapes.Add(shapeType);
            Debug.Log($"Added {shapeType} to available shapes");
        }
    }

    // Remove a shape from available pool (if you want to disable shapes)
    public void RemoveAvailableShape(ShapeType shapeType)
    {
        if (availableShapes.Contains(shapeType))
        {
            availableShapes.Remove(shapeType);
            Debug.Log($"Removed {shapeType} from available shapes");

            // Ensure we always have at least one shape
            if (availableShapes.Count == 0)
            {
                availableShapes.Add(ShapeType.Heart);
                Debug.LogWarning("No shapes available, added Heart as fallback");
            }
        }
    }

    // Get all available shapes (for debugging)
    public List<ShapeType> GetAllAvailableShapes()
    {
        return new List<ShapeType>(availableShapes);
    }

    public void RestartGame()
    {
        // Reset score
        Score = 0;

        // Reset time scale if game was paused
        Time.timeScale = 1f;

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
