using UnityEngine;
using UnityEngine.UI;

/**
 * The Utilities script handles all basic system functionality; such as
 * pausing, locking the cursor to the viewport and more.
 */
public class UtilitiesScript : MonoBehaviour
{
    public GameObject pauseText;
    public GameObject newWaveText;
    private bool paused = false;
    public GameObject gameOverMenu;
    public GameObject startMenu;
    public Camera cam;
    private bool cursorLocked;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        cursorLocked = true;

        Application.targetFrameRate = 300;
        
    }

    /**
     * Quits the game.
     */
    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {

        // pause
        if (Input.GetKeyDown(KeyCode.Space) && !startMenu.activeSelf && !gameOverMenu.activeSelf)
        {
            if (!paused)
            {
                if (newWaveText.activeSelf)
                {
                    newWaveText.SetActive(false);
                }
                paused = true;
                
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
               
                Time.timeScale = 1;
            }
            pauseText.SetActive(paused);

        }

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            // locking and unlocking cursor
            if (cursorLocked)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                    cursorLocked = false;

                }
            }
            else
            { 
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

                    if (pos.x >= 0.0f && pos.x <= 1.0f && pos.y >= 0.0f && pos.y <= 1.0f)
                    {
                        Cursor.lockState = CursorLockMode.Confined;
                        cursorLocked = true;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                QuitGame();
            }
        }
        
    }
}
