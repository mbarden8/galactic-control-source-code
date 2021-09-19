using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/**
 * Handles the enemy spawning in the game. Both beauforts and enemy space
 * ships will spawn off-screen, using a given offset. Space lasers will spawn
 * at a random y location on screen and will take up the width of the entire
 * viewport size.
 */
public class EnemySpawning : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] enemyWarnings;
    public GameObject player;
    public Camera mainCamera;

    public float spawnOffset = 0.3f;
    public float warningSpawnOffset = 0.5f;
    public int maxEnemiesOnScreen = 10;
    public float enemySpawnRate = 1.5f;
    public float timeBetweenWaves = 5f;

    public Text enemyText;
    public Text waveText;
    public GameObject newWaveText;

    public Volume vol;
    public AudioManager am;

    private EnemySpawnProbabilities spawnCalc;

    private int enemiesInWave = 0;
    private int enemiesToSpawn = 0;
    private int enemiesOnScreen = 0;
    private int enemyCounterDisplay;
    private int currentWave = 0;
    private float beaufSpeed = 3.4f;
    public float maxBeaufSpeed = 4.5f;

    private void Start()
    {
        spawnCalc = this.GetComponent<EnemySpawnProbabilities>();
    }

    /**
     * Called when the player selects "Start Game" on the main menu.
     */
    public void StartGame()
    {
        player.SetActive(true);
        NewWave();
        StartCoroutine(CalculateEnemySpawn());
    }

    /**
     * Called when a new wave begins, updates the number of enemies
     * as well as enemy speed.
     */
    private void NewWave()
    {
        if (beaufSpeed < maxBeaufSpeed)
        {
            beaufSpeed += 0.025f;
        }
        currentWave++;
        
        enemiesInWave += 5;
        enemiesToSpawn = enemiesInWave;
        spawnCalc.IncrementSpecialEnemies(currentWave);
        enemyCounterDisplay = enemiesInWave;
        player.GetComponent<PlayerController>().IncreasePlayerForce();
        UpdateEnemyCounterUI();
        UpdateRoundDisplay();
    }

    /**
     * Spawns enemies off screen. Given on offset, enemies will be spawned in
     * on a specific side of the screen. Player's will be prompted a warning 
     * telling the player what direction the enemies are coming from.
     */
    private IEnumerator CalculateEnemySpawn()
    {

        while (true)
        {
            yield return new WaitForSeconds(enemySpawnRate);

            if (enemiesToSpawn > 0 && enemiesOnScreen < maxEnemiesOnScreen)
            {
                int enemy = spawnCalc.EnemyToSpawn(enemiesToSpawn, currentWave);

                if (enemy == 1)
                {
                    SpawnLaser();
                }

                else
                {
                    EnemyPosCalc(enemy);
                }
            }
        }
 
    }

    /**
     * Spawns laser.
     */
    private void SpawnLaser()
    {
        float spawnX = 0.5f; // spawn in middle of screen
        float spawnY = Random.Range(0.0f, 1.0f); // random y spawn on screen

        Vector3 pos = mainCamera.ViewportToWorldPoint(
            new Vector3(spawnX, spawnY, 0f));

        pos.z = 0;
        GameObject laser = Instantiate(enemies[1], pos, Quaternion.identity);
        laser.GetComponent<SpaceLaserScript>().SetPlayerObject(player);
        player.GetComponent<PlayerController>().am.PlayLaserWindup();
    }

    /**
     * Calculates the spawn position of either beaufort
     * or the enemy ship.
     * 
     * @param id The id of the enemy we are spawning.
     */
    private void EnemyPosCalc(int id)
    {
        // 0: left, 1: right, 2: top, 3: bottom
        int spawnSide = Random.Range(0, 3);

        float spawnX;
        float spawnY;

        float warningSpawnX;
        float warningSpawnY;

        if (spawnSide == 0)
        {
            spawnX = -spawnOffset;
            spawnY = Random.Range(0.0f, 1.0f);

            warningSpawnX = spawnX + warningSpawnOffset;
            warningSpawnY = spawnY;
        }

        else if (spawnSide == 1)
        {
            spawnX = 1 + spawnOffset;
            spawnY = Random.Range(0.0f, 1.0f);

            warningSpawnX = spawnX - warningSpawnOffset;
            warningSpawnY = spawnY;
        }

        else if (spawnSide == 2)
        {
            spawnX = Random.Range(0.0f, 1.0f);
            spawnY = 1 + spawnOffset;

            warningSpawnX = spawnX;
            warningSpawnY = spawnY - warningSpawnOffset;
        }

        else
        {
            spawnX = Random.Range(0.0f, 1.0f);
            spawnY = -spawnOffset;

            warningSpawnX = spawnX;
            warningSpawnY = spawnY + warningSpawnOffset;
        }

        Vector3 pos = mainCamera.ViewportToWorldPoint(
            new Vector3(spawnX, spawnY, 0f));

        Vector3 warningPos = mainCamera.ViewportToWorldPoint(
            new Vector3(warningSpawnX, warningSpawnY, 0f));

        pos.z = 0;
        warningPos.z = 0;


        SpawnEnemy(id, pos, Quaternion.identity);
        SpawnWarning(id, warningPos);
    }

    /**
     * Spawns the given enemy at the given position.
     * 
     * @param id The id of the enemy
     * @param pos The position to be spawned at
     * @param q The quaternion of the enemy to be spawned 
     */
    private void SpawnEnemy(int id, Vector3 pos, Quaternion q)
    {
        GameObject enemy = Instantiate(enemies[id], pos, q);
        if (id == 0)
        {
            enemy.GetComponent<BeaufortScript>().Init(player, beaufSpeed, am);
        }
        else
        {
            enemy.GetComponent<EnemyShipScript>().InitializeReferences(player, mainCamera);
        }
        enemiesToSpawn--;
        enemiesOnScreen++;
    }

    /**
     * Updates the enemy counter when an enemy is killed.
     */
    public void SetEnemyCounter()
    {
        enemyCounterDisplay--;
        enemiesOnScreen--;
        
        UpdateEnemyCounterUI();
    }

    /**
     * Updates enemy counter ui
     */
    private void UpdateEnemyCounterUI()
    {
        enemyText.text = "Enemies: " + enemyCounterDisplay;

        if (enemyCounterDisplay == 0)
        {
            StartCoroutine(EndOfWave());
        }
    }

    /**
     * Displays the new wave text and gives player a break
     * between waves.
     */
    private IEnumerator EndOfWave()
    {
        yield return new WaitForSeconds(1.5f);
        newWaveText.SetActive(true);
        yield return new WaitForSeconds(timeBetweenWaves);
        newWaveText.SetActive(false);
        NewWave();
    }

    /**
     * Spawns the given enemy warning at the given position.
     * 
     * @param id The id of the enemy who's warning will be displayed.
     * @param pos The position the warning will be spawned at.
     */
    private void SpawnWarning(int id, Vector3 pos)
    {
        Instantiate(enemyWarnings[id], pos, Quaternion.identity);
    }

    /**
     * Updates the ui to stay consistent with the current wave.
     */
    private void UpdateRoundDisplay()
    {
        waveText.text = "Wave " + currentWave;
    }

}
