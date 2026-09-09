using UnityEngine;

public class Tile : MonoBehaviour
{
    public int cardPairID; // Stores the pair index to avoid relying on Unity Tags
    private bool tileRevealed = false;
    public Sprite originalSprite;
    public Sprite hiddenSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideCard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        if (tileRevealed) return;

        GameObject managerObj = GameObject.Find("gameManager"); // Ensure exact hierarchy name
        if (managerObj == null)
        {
            Debug.LogError("Could not find GameObject named 'gameManager' in the scene!", this);
            return;
        }

        ManageCards gameManager = managerObj.GetComponent<ManageCards>();
        if (gameManager == null)
        {
            Debug.LogError("ManageCards script is missing from the gameManager GameObject!", managerObj);
            return;
        }

        gameManager.CardSelected(gameObject);
    }

    public void HideCard()
    {
        GetComponent<SpriteRenderer>().sprite = hiddenSprite;
        tileRevealed = false;
    }

    public void RevealCard()
    {
        GetComponent<SpriteRenderer>().sprite = originalSprite;
        tileRevealed = true;
    }

    public void SetOriginalSprite(Sprite newSprite)
    {
        originalSprite = newSprite;
    }
}