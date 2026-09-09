using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class ManageCards : MonoBehaviour
{
    public GameObject card;

    [Header("Top Bar UI References (TextMeshPro)")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI matchText;

    private bool firstCardSelected, secondCardSelected;
    private GameObject card1, card2;

    private bool cardMismatchTimerActive;
    private float cardMismatchTimer;

    private int nbMatch = 0;
    private int totalPairs;

    private float remainingTime;
    private bool isGameActive = true;
    private string playerName;

    private readonly string[] suits = { "hearts", "diamonds", "clubs", "spades" };
    private readonly string[] cardValues = { "ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king" };

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Player 1");
        if (playerNameText != null)
        {
            playerNameText.text = "Player: " + playerName;
        }

        totalPairs = PlayerPrefs.GetInt("SelectedPairCount", 10);
        remainingTime = PlayerPrefs.GetFloat("SelectedTimeLimit", 120f);

        if (matchText != null)
        {
            matchText.text = "Matches: 0/" + totalPairs;
        }

        DisplayCards();
    }

    void Update()
    {
        if (!isGameActive) return;

        HandleGameTimer();
        HandleCardMismatchTimer();
    }

    private void HandleGameTimer()
    {
        remainingTime -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.CeilToInt(remainingTime) + "s";
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver(false);
        }
    }

    private void HandleCardMismatchTimer()
    {
        if (cardMismatchTimerActive)
        {
            cardMismatchTimer += Time.deltaTime;

            if (cardMismatchTimer >= 1f)
            {
                cardMismatchTimerActive = false;

                if (card1 != null && card2 != null)
                {
                    Tile tile1 = card1.GetComponent<Tile>();
                    Tile tile2 = card2.GetComponent<Tile>();

                    // Check if both tiles exist and match using cardPairID
                    if (tile1 != null && tile2 != null && tile1.cardPairID == tile2.cardPairID)
                    {
                        Destroy(card1);
                        Destroy(card2);
                        nbMatch++;

                        if (matchText != null)
                        {
                            matchText.text = "Matches: " + nbMatch + "/" + totalPairs;
                        }

                        if (nbMatch >= totalPairs)
                        {
                            GameOver(true);
                        }
                    }
                    else
                    {
                        if (tile1 != null) tile1.HideCard();
                        if (tile2 != null) tile2.HideCard();
                    }
                }

                firstCardSelected = false;
                secondCardSelected = false;
                card1 = null;
                card2 = null;
                cardMismatchTimer = 0;
            }
        }
    }

    public void StopGameEarly()
    {
        GameOver(false);
    }

    public void DisplayCards()
    {
        List<string> selectedCardNames = GenerateRandomCardPool(totalPairs);

        List<int> deckIndices = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            deckIndices.Add(i);
            deckIndices.Add(i);
        }
        ShuffleList(deckIndices);

        int columns = (totalPairs >= 15) ? 10 : (totalPairs >= 12 ? 8 : 10);
        int rows = Mathf.CeilToInt((totalPairs * 2.0f) / columns);

        float cardOriginalScale = card.transform.localScale.x;
        float scaleFactor = (500 * cardOriginalScale) / 100.0f;
        float yScaleFactor = (725 * cardOriginalScale) / 100.0f;

        GameObject cen = GameObject.Find("centerOfScreen");
        if (cen == null)
        {
            Debug.LogError("GameObject 'centerOfScreen' missing from scene!");
            return;
        }

        for (int i = 0; i < deckIndices.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 newPosition = new Vector3(
                cen.transform.position.x + ((col - (columns - 1) / 2.0f) * scaleFactor),
                cen.transform.position.y + ((row - (rows - 1) / 2.0f) * yScaleFactor),
                cen.transform.position.z
            );

            int pairID = deckIndices[i];
            GameObject c = Instantiate(card, newPosition, Quaternion.identity);
            c.name = row + "_" + col;

            Sprite cardSprite = Resources.Load<Sprite>(selectedCardNames[pairID]);

            if (c.TryGetComponent<Tile>(out Tile tileComponent))
            {
                tileComponent.cardPairID = pairID; // Sets unique pair identifier
                tileComponent.SetOriginalSprite(cardSprite);
            }
        }
    }

    private List<string> GenerateRandomCardPool(int countNeeded)
    {
        List<string> fullDeck = new List<string>();

        foreach (string suit in suits)
        {
            foreach (string val in cardValues)
            {
                fullDeck.Add(val + "_of_" + suit);
            }
        }

        ShuffleList(fullDeck);
        return fullDeck.GetRange(0, countNeeded);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void CardSelected(GameObject cardObj)
    {
        if (!isGameActive || cardMismatchTimerActive || cardObj == card1) return;

        if (!firstCardSelected)
        {
            firstCardSelected = true;
            card1 = cardObj;
            if (card1.TryGetComponent<Tile>(out Tile t1)) t1.RevealCard();
        }
        else if (!secondCardSelected)
        {
            card2 = cardObj;
            secondCardSelected = true;
            if (card2.TryGetComponent<Tile>(out Tile t2)) t2.RevealCard();
            cardMismatchTimerActive = true;
        }
    }

    private void GameOver(bool hasWon)
    {
        isGameActive = false;

        int score = (nbMatch * 100) + (hasWon ? Mathf.CeilToInt(remainingTime) * 10 : 0);

        PlayerPrefs.SetInt("HasWonGame", hasWon ? 1 : 0);
        PlayerPrefs.SetInt("FinalMatches", nbMatch);
        PlayerPrefs.SetInt("TotalPairs", totalPairs);
        PlayerPrefs.SetFloat("FinalTimeRemaining", remainingTime);
        PlayerPrefs.SetInt("CurrentSessionScore", score);
        PlayerPrefs.Save();

        SceneManager.LoadScene("exitScreen");
    }
}