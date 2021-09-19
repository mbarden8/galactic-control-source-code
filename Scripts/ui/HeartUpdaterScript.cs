using UnityEngine.UI;
using UnityEngine;

/**
 * The HeartUpdaterScript updates the heart display UI in accordance to
 * how much health the player has remaining.
 */
public class HeartUpdaterScript : MonoBehaviour
{

    public float fullHeart = 3f;
    public float halfHeart = 2.5f;
    public float emptyHeart = 2f;
    private Image heartDisplay;
    public Sprite[] heartSprites;

    void Start()
    {
        heartDisplay = this.GetComponent<Image>();
    }

    /**
     * Enables this heart image.
     */
    public void EnableImage()
    {
        heartDisplay.enabled = true;
    }

    /**
     * Disables this heart image.
     */
    public void DisableImage()
    {
        heartDisplay.enabled = false;
    }

    /**
     * Updates the sprite depending on how much health the player has
     * remaining.
     * 
     * @param health The amount of health the player has.
     */
    public void UpdateHeartDisplay(float health)
    {
        if (health >= fullHeart)
        {
            heartDisplay.sprite = heartSprites[0];
        }
        else if (health == halfHeart)
        {
            heartDisplay.sprite = heartSprites[1];
        }
        else
        {
            heartDisplay.sprite = heartSprites[2];
        }
    }

}
