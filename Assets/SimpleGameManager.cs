using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SimpleGameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isGameOver = false;
    
    private float survivalTime = 0f;
    private bool gameStarted = false;
    
    private TextMeshProUGUI scoreText;
    private GameObject pauseScreen;
    private GameObject gameOverScreen;
    
    public static SimpleGameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        gameStarted = true;
        CreateUI();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            TogglePause();
        }
        
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)
        {
            TogglePause();
        }
        
        if (gameStarted && !isGamePaused && !isGameOver)
        {
            survivalTime += Time.deltaTime;
            UpdateScoreDisplay();
        }
    }
    
    private void CreateUI()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
        
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        CreateScoreText(canvas);
        CreatePauseScreen(canvas);
        CreateGameOverScreen(canvas);
    }
    
    private void CreateScoreText(Canvas canvas)
    {
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(canvas.transform);
        
        scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
        scoreText.text = "Time: 0:00";
        scoreText.fontSize = 24;
        scoreText.color = Color.white;
        scoreText.alignment = TextAlignmentOptions.TopLeft;
        
        RectTransform rectTransform = scoreObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(100, -30);
        rectTransform.sizeDelta = new Vector2(200, 50);
    }
    
    private void CreatePauseScreen(Canvas canvas)
    {
        pauseScreen = new GameObject("PauseScreen");
        pauseScreen.transform.SetParent(canvas.transform);
        
        Image bg = pauseScreen.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        
        RectTransform bgRect = pauseScreen.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        GameObject textObj = new GameObject("PauseText");
        textObj.transform.SetParent(pauseScreen.transform);
        
        TextMeshProUGUI pauseText = textObj.AddComponent<TextMeshProUGUI>();
        pauseText.text = "PAUSED";
        pauseText.fontSize = 48;
        pauseText.color = Color.white;
        pauseText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.4f);
        textRect.anchorMax = new Vector2(0.5f, 0.4f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(400, 100);
        
        GameObject lightObj = new GameObject("LightText");
        lightObj.transform.SetParent(pauseScreen.transform);
        
        TextMeshProUGUI lightText = lightObj.AddComponent<TextMeshProUGUI>();
        lightText.text = "LIGHT";
        lightText.fontSize = 35;
        lightText.color = Color.white;
        lightText.alignment = TextAlignmentOptions.Center;
        lightText.fontStyle = FontStyles.Bold;
        
        RectTransform lightRect = lightObj.GetComponent<RectTransform>();
        lightRect.anchorMin = new Vector2(0.5f, 0.65f);
        lightRect.anchorMax = new Vector2(0.5f, 0.65f);
        lightRect.anchoredPosition = new Vector2(-50, 0);
        lightRect.sizeDelta = new Vector2(120, 50);
        
        GameObject stepObj = new GameObject("StepText");
        stepObj.transform.SetParent(pauseScreen.transform);
        
        TextMeshProUGUI stepText = stepObj.AddComponent<TextMeshProUGUI>();
        stepText.text = "STEP";
        stepText.fontSize = 35;
        stepText.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        stepText.alignment = TextAlignmentOptions.Center;
        stepText.fontStyle = FontStyles.Bold;
        
        RectTransform stepRect = stepObj.GetComponent<RectTransform>();
        stepRect.anchorMin = new Vector2(0.5f, 0.65f);
        stepRect.anchorMax = new Vector2(0.5f, 0.65f);
        stepRect.anchoredPosition = new Vector2(65, 0);
        stepRect.sizeDelta = new Vector2(120, 50);
        
        GameObject controlsObj = new GameObject("ControlsText");
        controlsObj.transform.SetParent(pauseScreen.transform);
        
        TextMeshProUGUI controlsText = controlsObj.AddComponent<TextMeshProUGUI>();
        controlsText.text = "CONTROLS:\n↑ / W - Jump\n↓ / S - Crouch\nSPACE - Switch Form\nESC / P - Pause";
        controlsText.fontSize = 16;
        controlsText.color = Color.white;
        controlsText.alignment = TextAlignmentOptions.TopRight;
        
        RectTransform controlsRect = controlsObj.GetComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(1f, 1f);
        controlsRect.anchorMax = new Vector2(1f, 1f);
        controlsRect.anchoredPosition = new Vector2(-150, -100);
        controlsRect.sizeDelta = new Vector2(180, 120);
        
        pauseScreen.SetActive(false);
    }
    
    private void CreateGameOverScreen(Canvas canvas)
    {
        gameOverScreen = new GameObject("GameOverScreen");
        gameOverScreen.transform.SetParent(canvas.transform);
        
        Image bg = gameOverScreen.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);
        
        RectTransform bgRect = gameOverScreen.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(gameOverScreen.transform);
        
        TextMeshProUGUI gameOverText = textObj.AddComponent<TextMeshProUGUI>();
        gameOverText.text = "GAME OVER";
        gameOverText.fontSize = 48;
        gameOverText.color = Color.red;
        gameOverText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.6f);
        textRect.anchorMax = new Vector2(0.5f, 0.6f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(400, 100);
        
        GameObject scoreObj = new GameObject("FinalScoreText");
        scoreObj.transform.SetParent(gameOverScreen.transform);
        
        TextMeshProUGUI finalScoreText = scoreObj.AddComponent<TextMeshProUGUI>();
        finalScoreText.text = "Final Time: 0:00";
        finalScoreText.fontSize = 24;
        finalScoreText.color = Color.white;
        finalScoreText.alignment = TextAlignmentOptions.Center;
        
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 0.4f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.4f);
        scoreRect.anchoredPosition = Vector2.zero;
        scoreRect.sizeDelta = new Vector2(400, 50);
        
        // Create Restart Button
        GameObject restartButtonObj = new GameObject("RestartButton");
        restartButtonObj.transform.SetParent(gameOverScreen.transform);
        
        Button restartButton = restartButtonObj.AddComponent<Button>();
        Image buttonImage = restartButtonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 0.8f); // Green background
        
        RectTransform buttonRect = restartButtonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.25f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.25f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(200, 50);
        
        // Button Text
        GameObject buttonTextObj = new GameObject("RestartButtonText");
        buttonTextObj.transform.SetParent(restartButtonObj.transform);
        
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "RESTART";
        buttonText.fontSize = 20;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // Add button click event
        restartButton.onClick.AddListener(RestartGame);
        
        gameOverScreen.SetActive(false);
    }
    
    public void TogglePause()
    {
        if (isGameOver) return;
        
        isGamePaused = !isGamePaused;
        
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            if (pauseScreen != null)
                pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseScreen != null)
                pauseScreen.SetActive(false);
        }
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f;
        
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            
            TextMeshProUGUI[] allTexts = gameOverScreen.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (text.text.Contains("Final Time"))
                {
                    text.text = $"Final Time: {FormatTime(survivalTime)}";
                    break;
                }
            }
        }
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Time: {FormatTime(survivalTime)}";
        }
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes}:{seconds:00}";
    }
    
    public float GetSurvivalTime()
    {
        return survivalTime;
    }
    
    public int GetScore()
    {
        return Mathf.FloorToInt(survivalTime);
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame() called - button clicked!");
        
        // Reset game state
        isGameOver = false;
        isGamePaused = false;
        survivalTime = 0f;
        gameStarted = true;
        
        // Reset time scale
        Time.timeScale = 1f;
        Debug.Log("Time scale reset to 1");
        
        // Hide game over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
            Debug.Log("Game over screen hidden");
        }
        
        // Clear all active obstacles
        Spawner spawner = FindFirstObjectByType<Spawner>();
        if (spawner != null)
        {
            spawner.ClearAllObstacles();
            spawner.ResetSpeed();
            Debug.Log("Obstacles cleared and speed reset");
            
            // Restart tutorial if enabled
            if (spawner.enableTutorial)
            {
                spawner.RestartTutorial();
                Debug.Log("Tutorial restarted");
            }
        }
        else
        {
            Debug.LogError("Spawner not found!");
        }
        
        // Reset player position and state
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
        {
            // Try to find inactive player
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "Player" && obj.GetComponent<PlayerMovement>() != null)
                {
                    playerMovement = obj.GetComponent<PlayerMovement>();
                    break;
                }
            }
        }
        
        if (playerMovement != null)
        {
            // Reactivate player if it was deactivated
            if (!playerMovement.gameObject.activeInHierarchy)
            {
                playerMovement.gameObject.SetActive(true);
                Debug.Log("Player reactivated");
            }
            
            // Reset player to starting position
            playerMovement.transform.position = new Vector3(-7f, -4f, 0f);
            Debug.Log("Player position reset");
            
            // Reset player velocity
            Rigidbody2D playerRb = playerMovement.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                Debug.Log("Player velocity reset");
            }
        }
        else
        {
            Debug.LogError("PlayerMovement not found!");
        }
        
        // Reset player form to light
        PlayerFormSwitcher formSwitcher = FindFirstObjectByType<PlayerFormSwitcher>();
        if (formSwitcher == null && playerMovement != null)
        {
            // Try to get it from the same player object
            formSwitcher = playerMovement.GetComponent<PlayerFormSwitcher>();
        }
        
        if (formSwitcher != null)
        {
            formSwitcher.SetForm(PlayerFormSwitcher.Form.Light);
            Debug.Log("Player form reset to Light");
        }
        else
        {
            Debug.LogError("PlayerFormSwitcher not found!");
        }
        
        Debug.Log("Game restart completed successfully!");
    }
}
