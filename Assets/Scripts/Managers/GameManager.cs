using UnityEngine;
using UnityEngine.SceneManagement;

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
    public ShapeType currentShapeType = ShapeType.Heart;
    
    [Header("Performance")]
    public int targetFPS = 30;
    public int poolSize = 20;
    
    // Game state
    public GameState CurrentState { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver => CurrentState == GameState.GameOver;
    
    // References - use Find() to get them
    private PoolManager poolManager;
    private UIManager uiManager;
    private BlockSpawner blockSpawner;
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
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
        // Performance
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = 1f / 30f;
        
        // Get references
        poolManager = GetComponent<PoolManager>();
        uiManager = GetComponent<UIManager>();
        blockSpawner = GetComponent<BlockSpawner>();
        
        SetGameState(GameState.Menu);
    }
    
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        
        switch (newState)
        {
            case GameState.Menu:
                uiManager?.ShowMenu();
                break;
            case GameState.Playing:
                Score = 0;
                uiManager?.ShowGameUI();
                blockSpawner?.StartSpawning();
                break;
            case GameState.GameOver:
                uiManager?.ShowGameOver(Score);
                blockSpawner?.StopSpawning();
                break;
        }
    }
    
    public void AddScore(int points)
    {
        Score += points;
        uiManager?.UpdateScore(Score);
    }
    
    public void ChangeShape(ShapeType shapeType)
    {
        currentShapeType = shapeType;
        uiManager?.UpdateCurrentShape(shapeType);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
