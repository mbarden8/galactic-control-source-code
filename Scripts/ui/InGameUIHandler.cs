using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/**
 * Handles updating the in-game UI based on what occurs during
 * the gameplay.
 */
public class InGameUIHandler : MonoBehaviour
{
    public HeartUpdaterScript[] heartUpdaters;

    public Text waveText;
    public Text enemyText;
    public Text powerupText;
    public GameObject startMenu;
    public GameObject gameOverMenu;
    public GameObject optionsMenu;
    public Sprite unselected;
    public Sprite selected;
    public Image fullScreenOptions;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Fullscreen", 0) == 0)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            fullScreenOptions.sprite = unselected;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            fullScreenOptions.sprite = selected;
        }
    }

    /**
     * Hides the ui elements on screen.
     */
    public void GameHideUI()
    {
        foreach (HeartUpdaterScript heart in heartUpdaters)
        {
            heart.DisableImage();
        }

        waveText.enabled = false;
        enemyText.enabled = false;
    }

    /**
     * Disables the start screen menu.
     */
    public void StartHideUI()
    {
        startMenu.SetActive(false);
    }

    /**
     * Enables the options menu.
     */
    public void EnableOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    /**
     * Disables the options menu.
     */
    public void DisableOptionsMenu()
    {
        optionsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    /**
     * Toggles the full screen option.
     */
    public void SetFullScreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            fullScreenOptions.sprite = selected;
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            fullScreenOptions.sprite = unselected;
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
    }

    /**
     * Shows the ui elements on screen.
     */
    public void GameEnableUI()
    {
        foreach (HeartUpdaterScript heart in heartUpdaters)
        {
            heart.EnableImage();
        }

        waveText.enabled = true;
        enemyText.enabled = true;
    }

    /**
     * Enables the powerup text in the bottom left corner.
     * 
     * @param id The powerup id to display.
     */
    public void ShowPowerupText(string id)
    {
        powerupText.text = id;
        
        powerupText.enabled = true;
    }

    /**
     * Hides the powerup text in the bottom left corner.
     */
    public void HidePowerupText()
    {
        powerupText.enabled = false;
    }

    /**
     * Enables the game over menu.
     */
    public void EnableGameOver()
    {
        gameOverMenu.SetActive(true);
        GameHideUI();
    }

    /**
     * Starts a new game.
     */
    public void NewGame()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /**
     * Updates the display of our lives on the in-game ui based on
     * the player's health.
     * 
     * @param health How much health the player has left.
     */
    public void UpdateLives(float health)
    {
        foreach (HeartUpdaterScript heart in heartUpdaters)
        {
            heart.UpdateHeartDisplay(health);
        }

    }
}
