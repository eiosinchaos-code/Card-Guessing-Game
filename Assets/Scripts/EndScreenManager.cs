using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ExitScreenManager : MonoBehaviour
{
    [Header("UI Text References (TextMeshPro)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI highScoreText;

    [Header("Conditional High Score Save")]
    public Button saveScoreButton;
    public TextMeshProUGUI saveScoreButtonText;

    private int currentScore;
    private int highestScore;
    private string playerName;

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Player 1");
        bool hasWon = PlayerPrefs.GetInt("HasWonGame", 0) == 1;
        int matches = PlayerPrefs.GetInt("FinalMatches", 0);
        int totalPairs = PlayerPrefs.GetInt("TotalPairs", 10);
        float timeRemaining = PlayerPrefs.GetFloat("FinalTimeRemaining", 0f);
        currentScore = PlayerPrefs.GetInt("CurrentSessionScore", 0);

        highestScore = PlayerPrefs.GetInt("HighScore", 0);
        string highScorePlayer = PlayerPrefs.GetString("HighScorePlayer", "None");

        if (titleText != null)
        {
            titleText.text = hasWon ? "VICTORY!" : "GAME OVER";
            titleText.color = hasWon ? Color.green : Color.red;
        }

        if (messageText != null)
        {
            messageText.text = hasWon
                ? $"Congratulations, {playerName}! You successfully matched all cards!"
                : $"Better luck next time, {playerName}! You didn't finish in time.";
        }

        if (statsText != null)
        {
            statsText.text = $"Matches: {matches}/{totalPairs}\nTime Remaining: {Mathf.CeilToInt(timeRemaining)}s\nYour Score: {currentScore}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highestScore} (by {highScorePlayer})";
        }

        // ONLY offer to save if current score is higher than the recorded high score
        if (saveScoreButton != null)
        {
            if (currentScore > highestScore)
            {
                saveScoreButton.gameObject.SetActive(true);
                if (saveScoreButtonText != null) saveScoreButtonText.text = "Save High Score!";
            }
            else
            {
                saveScoreButton.gameObject.SetActive(false);
            }
        }
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", currentScore);
        PlayerPrefs.SetString("HighScorePlayer", playerName);
        PlayerPrefs.Save();

        highestScore = currentScore;

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highestScore} (by {playerName})";
        }

        if (saveScoreButton != null)
        {
            saveScoreButton.interactable = false;
            if (saveScoreButtonText != null) saveScoreButtonText.text = "Score Saved!";
        }
    }

    // --- Navigation Methods ---

    public void PlayAgain()
    {
        SceneManager.LoadScene("chapterCards");
    }

    public void ReturnToStartScreen()
    {
        SceneManager.LoadScene("startScreen");
    }

    public void OpenPreferences()
    {
        SceneManager.LoadScene("preferences");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}