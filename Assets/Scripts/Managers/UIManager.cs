using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ShapeUI
{
    public ShapeType shapeType;
    public Sprite sprite;
    public string displayName;
}

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject menuScreen;
    public GameObject gameScreen;
    public GameObject gameOverScreen;
    public GameObject shapeSelectionScreen;
    
    [Header("Game UI")]
    public Text scoreText;
    public Text highScoreText;
    public Text currentShapeText;
    public Image currentShapeImage;
    
    [Header("Game Over")]
    public Text finalScoreText;
    public Text newHighScoreText;
    
    [Header("Shape Selection")]
    public Transform shapeButtonContainer;
    public GameObject shapeButtonPrefab;
    public List<ShapeUI> shapeUIs = new List<ShapeUI>();
    
    private Dictionary<ShapeType, ShapeUI> shapeDictionary;
    private int highScore = 0;
    
    void Start()
    {
        InitializeShapeDictionary();
        LoadHighScore();
        ShowMenu();
    }
    
    void InitializeShapeDictionary()
    {
        shapeDictionary = new Dictionary<ShapeType, ShapeUI>();
        foreach (ShapeUI shapeUI in shapeUIs)
        {
            shapeDictionary[shapeUI.shapeType] = shapeUI;
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
        SetScreen(shapeSelectionScreen, false);
    }
    
    public void ShowGameUI()
    {
        SetScreen(menuScreen, false);
        SetScreen(gameScreen, true);
        SetScreen(gameOverScreen, false);
        SetScreen(shapeSelectionScreen, false);
        
        UpdateScore(0);
        UpdateCurrentShape(GameManager.Instance.currentShapeType);
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
            if (newHighScoreText != null)
                newHighScoreText.gameObject.SetActive(true);
        }
    }
    
    public void ShowShapeSelection()
    {
        SetScreen(shapeSelectionScreen, true);
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    public void UpdateCurrentShape(ShapeType shapeType)
    {
        if (shapeDictionary.ContainsKey(shapeType))
        {
            if (currentShapeText != null)
                currentShapeText.text = shapeDictionary[shapeType].displayName;
            
            if (currentShapeImage != null)
                currentShapeImage.sprite = shapeDictionary[shapeType].sprite;
        }
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
    
    public void OnShapeSelected(ShapeType shapeType)
    {
        GameManager.Instance.ChangeShape(shapeType);
        ShowGameUI();
    }
}
