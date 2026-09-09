using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Preferences UI Elements (TextMeshPro)")]
    public TMP_InputField nameInputField;
    public TMP_Dropdown cardCountDropdown;
    public TMP_Dropdown timerDropdown;

    void Start()
    {
        LoadPreferencesIntoUI();
    }

    private void LoadPreferencesIntoUI()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "Player 1");
        int savedCardIndex = PlayerPrefs.GetInt("SelectedCardCountIndex", 0);
        int savedTimerIndex = PlayerPrefs.GetInt("SelectedTimerIndex", 0);

        if (nameInputField != null) nameInputField.text = savedName;
        if (cardCountDropdown != null) cardCountDropdown.value = savedCardIndex;
        if (timerDropdown != null) timerDropdown.value = savedTimerIndex;
    }

    public void SavePreferencesAndReturn()
    {
        // Save Player Name
        string nameToSave = "Player 1";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            nameToSave = nameInputField.text;
        }
        PlayerPrefs.SetString("PlayerName", nameToSave);

        // Save Card Count Selection (Options: 10, 12, 14, 16, 20 Pairs)
        int cardIndex = 0;
        if (cardCountDropdown != null) cardIndex = cardCountDropdown.value;

        PlayerPrefs.SetInt("SelectedCardCountIndex", cardIndex);
        int[] pairOptions = { 10, 12, 14, 16, 20 };
        PlayerPrefs.SetInt("SelectedPairCount", pairOptions[cardIndex]);

        // Save Timer Selection (Options: 30s, 60s, 90s, 120s, 180s, 999s)
        int timerIndex = 0;
        if (timerDropdown != null) timerIndex = timerDropdown.value;

        PlayerPrefs.SetInt("SelectedTimerIndex", timerIndex);
        float[] timeOptions = { 30f, 60f, 90f, 120f, 180f, 999f };
        PlayerPrefs.SetFloat("SelectedTimeLimit", timeOptions[timerIndex]);

        PlayerPrefs.Save();

        LoadStartScreen();
    }

    // --- Scene Management Navigation Methods ---

    public void LoadChapterCards()
    {
        SceneManager.LoadScene("chapterCards");
    }

    public void LoadPreferences()
    {
        SceneManager.LoadScene("preferences");
    }

    public void LoadStartScreen()
    {
        SceneManager.LoadScene("startScreen");
    }

    public void LoadExitScreen()
    {
        SceneManager.LoadScene("exitScreen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}