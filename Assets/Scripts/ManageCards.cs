using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageCards : MonoBehaviour
{
    public GameObject card;
    bool firstCardSelected, secondCardSelected;
    GameObject card1, card2;
    string rowForCard1, rowForCard2;
    bool timerHasStarted;
    float timer;
    int nbMatch = 0;

    void Start()
    {
        DisplayCards();
    }

    void Update()
    {
        if (timerHasStarted)
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                timerHasStarted = false;

                if (card1 != null && card2 != null)
                {
                    if (card1.tag == card2.tag)
                    {
                        Destroy(card1);
                        Destroy(card2);
                        nbMatch++;
                        if (nbMatch == 10) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    else
                    {
                        card1.GetComponent<Tile>().HideCard();
                        card2.GetComponent<Tile>().HideCard();
                    }
                }

                firstCardSelected = false;
                secondCardSelected = false;
                card1 = null;
                card2 = null;
                rowForCard1 = "";
                rowForCard2 = "";
                timer = 0;
            }
        }
    }

    public void DisplayCards()
    {
        int[] shuffledArray1 = CreateShuffledArray();
        int[] shuffledArray2 = CreateShuffledArray();

        for (int i = 0; i < 10; i++)
        {
            AddACard(0, i, shuffledArray1[i]);
            AddACard(1, i, shuffledArray2[i]);
        }
    }

    void AddACard(int row, int rank, int value)
    {
        float cardOriginalScale = card.transform.localScale.x;
        float scaleFactor = (500 * cardOriginalScale) / 100.0f;
        float yScaleFactor = (725 * cardOriginalScale) / 100.0f;

        GameObject cen = GameObject.Find("centerOfScreen");

        Vector3 newPosition = new Vector3(
            cen.transform.position.x + ((rank - 4.5f) * scaleFactor),
            cen.transform.position.y + ((row - 0.5f) * yScaleFactor),
            cen.transform.position.z
        );

        GameObject c = Instantiate(card, newPosition, Quaternion.identity);
        c.tag = "" + (value + 1);
        c.name = row + "_" + value;

        string cardNumber = (value == 0) ? "ace" : "" + (value + 1);
        string nameOfCard = cardNumber + "_of_hearts";

        Sprite s1 = Resources.Load<Sprite>(nameOfCard);

        if (c.TryGetComponent<Tile>(out Tile tileComponent))
        {
            tileComponent.SetOriginalSprite(s1);
        }
    }

    public int[] CreateShuffledArray()
    {
        int[] newArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int t = 0; t < newArray.Length; t++)
        {
            int tmp = newArray[t];
            int r = Random.Range(t, newArray.Length);
            newArray[t] = newArray[r];
            newArray[r] = tmp;
        }

        return newArray;
    }

    public void CardSelected(GameObject card)
    {
        if (timerHasStarted || card == card1) return;

        if (!firstCardSelected)
        {
            string row = card.name.Substring(0, 1);
            rowForCard1 = row;
            firstCardSelected = true;
            card1 = card;
            card1.GetComponent<Tile>().RevealCard();
        }
        else if (!secondCardSelected)
        {
            string row = card.name.Substring(0, 1);
            rowForCard2 = row;
            if (rowForCard2 != rowForCard1)
            {
                card2 = card;
                secondCardSelected = true;
                card2.GetComponent<Tile>().RevealCard();
                CheckCards();
            }
        }
    }

    public void CheckCards()
    {
        RunTimer();
    }

    public void RunTimer()
    {
        timerHasStarted = true;
    }
}