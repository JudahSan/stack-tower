using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject menuScreen;
    public GameObject gameScreen;
    public GameObject gameOverScreen;
    
    [Header("Game UI - TextMeshPro")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text fpsText;
    
    [Header("Game Over UI")]
    public TMP_Text finalScoreText;
    public TMP_Text newHighScoreText;
    
    private int highScore = 0;
    private float deltaTime = 0.0f;
    
    void Start()
    {
        LoadHighScore();
        ShowMenu();
    }
    
    void Update()
    {
        // Update FPS counter
        if (fpsText != null)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
            
            // Color code FPS
            if (fps >= 28) fpsText.color = Color.green;
            else if (fps >= 25) fpsText.color = Color.yellow;
            else fpsText.color = Color.red;
        }
    }
    
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreDisplay();
    }
    
    void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = $"High: {highScore}";
    }
    
    public void ShowMenu()
    {
        SetScreen(menuScreen, true);
        SetScreen(gameScreen, false);
        SetScreen(gameOverScreen, false);
    }
    
    public void ShowGameUI()
    {
        SetScreen(menuScreen, false);
        SetScreen(gameScreen, true);
        SetScreen(gameOverScreen, false);
        
        UpdateScore(0);
    }
    
    public void ShowGameOver(int score)
    {
        SetScreen(gameScreen, false);
        SetScreen(gameOverScreen, true);
        
        if (finalScoreText != null)
            finalScoreText.text = $"Score: {score}";
        
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            
            if (newHighScoreText != null)
                newHighScoreText.gameObject.SetActive(true);
        }
        else
        {
            if (newHighScoreText != null)
                newHighScoreText.gameObject.SetActive(false);
        }
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    void SetScreen(GameObject screen, bool active)
    {
        if (screen != null)
            screen.SetActive(active);
    }
    
    // Button Events
    public void OnPlayButton()
    {
        GameManager.Instance.SetGameState(GameState.Playing);
    }
    
    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void OnMenuButton()
    {
        GameManager.Instance.SetGameState(GameState.Menu);
    }
    
    public void OnQuitButton()
    {
        GameManager.Instance.QuitGame();
    }
}
