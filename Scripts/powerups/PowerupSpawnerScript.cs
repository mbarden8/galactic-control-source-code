using UnityEngine;
using System.Collections.Generic;

/**
 * The powerup spawner is in charge of dictating which powerup to spawn
 * in a given situation.
 */
public class PowerupSpawnerScript : MonoBehaviour
{
    public GameObject[] powerups;
    private Dictionary<string, int> puMapper;
    public new Camera camera;
    public PlayerController player;
    public float minSpawnTime = 30.0f;
    public float maxSpawnTime = 120.0f;
    private float spawnTime;
    private float timer = 0f;
    private bool gameStarted = false;
    private int powerUpSave = 1;
    private Dictionary<string, float> probabilities;
    public float maxInARow = 2f;
    private float instaSpawnedInRow = 0f;
    private float shootSpawnedInRow = 0f;
    private float invincSpawnInRow = 0f;
    public float powerupSaves = 3f;

    // Start is called before the first frame update
    void Start()
    {
        CalculateNextSpawnTime();

        // construct default cdf
        probabilities = new Dictionary<string, float> {

            {"Insta-Kill", 0f},
            {"Shooting", 0f},
            {"Invincible", 0f},
            {"Health", 1f}

        };

        // set id mappings
        puMapper = new Dictionary<string, int>
        {
            {"Insta-Kill", 0},
            {"Shooting", 1},
            {"Invincible", 2}
        };

    }

    /**
     * Called when the game is started.
     */
    public void StartGame()
    {
        gameStarted = true;
    }

    /**
     * Spawns the given powerup returned by the CalculatePowerupToSpawn function.
     * Parameters passed to said function are dictated by which powerup was most
     * recently spawned since the CalculatePowerupToSpawn function works on a cdf.
     */
    private void SpawnPowerup()
    {
        int powerup;

        if (shootSpawnedInRow >= 1)
        {
            powerup = CalculatePowerupToSpawn("Shooting", shootSpawnedInRow, "Insta-Kill", instaSpawnedInRow, "Invincible", invincSpawnInRow);
        }

        else if(invincSpawnInRow >= 1)
        {
            powerup = CalculatePowerupToSpawn("Invincible", invincSpawnInRow, "Insta-Kill", instaSpawnedInRow, "Shooting", shootSpawnedInRow);
        }

        else
        {
            powerup = CalculatePowerupToSpawn("Insta-Kill", instaSpawnedInRow, "Shooting", shootSpawnedInRow, "Invincible", invincSpawnInRow);
        }

        float spawnX = Random.Range(0.05f, 0.95f);
        float spawnY = Random.Range(0.05f, 0.95f);

        Vector3 pos = camera.ViewportToWorldPoint(
            new Vector3(spawnX, spawnY, 0f));

        pos.z = 0;

        Instantiate(powerups[powerup], pos, Quaternion.identity);
        player.am.PlayPoweurpSpawn();
    }

    /**
     * Calculates the next powerup to be spawned using a cumulative density function.
     * The baseline probabilites are 33% for all three powerups, with the heart powerup
     * having a 1% chance of spawning. Thus, .33 + .33 + .33 + .01 = 1. However, the
     * player is granted 3 guaranteed heart powerup spawns a game in order to keep the game
     * going. When the player drops to 1 heart or lower, the heart powerup has a 100%
     * chance to spawn while they still have a guaranteed save active.
     * 
     * Additionally, as the player has less health, the baseline probability of a heart
     * spawning increases, but the chances always remain fairly low.
     * 
     * Each powerup is only able to spawn twice in a row before a different powerup has to spawn.
     * This keeps the gameplay more interesting with different powerups spawning regularly.
     * If a powerup just spawned, its spawning probability drops to about half of what it normally is,
     * with the probability of the other two powerups spawning equally increasing.
     * 
     * This function takes parameters because it works with a cdf and the most recent powerup spawned
     * must be calculated first, otherwise the function produces errors with different powerups
     * sharing an overlapping probability range.
     * 
     * @param pu1 The first powerup to be calculated.
     * @parm row1 How many times in a row this powerup has been spawned
     * 
     * -- repeat for pu2, row2, pu3, row3
     */
    private int CalculatePowerupToSpawn(string pu1, float row1, string pu2, float row2, string pu3, float row3)
    {
        // does the player need a heart save?
        if (player.lives <= 1 && powerupSaves > 0)
        {
            powerUpSave = 0;
        }

        // cdf
        probabilities[pu1] = ((((maxInARow - row1)) / (powerups.Length * maxInARow)) + 
            ((player.lives / 3) * 0.08f) * ((maxInARow - row1) / 2)) * powerUpSave;


        probabilities[pu2] = ((((probabilities[pu1] + Mathf.Abs(probabilities[pu1] - 0.33f) / 2) * ((maxInARow - row2) / 2)))
            + ((((maxInARow - row2)) / (powerups.Length * maxInARow)) +
            ((player.lives / 3) * 0.08f) * ((maxInARow - row2) / 2))) * powerUpSave;


        probabilities[pu3] = ((((probabilities[pu2] + Mathf.Abs(probabilities[pu2] - 0.66f)) * ((maxInARow - row3)/ 2))) + 
            ((((maxInARow - row3)) / (powerups.Length * maxInARow)) + 
            ((player.lives / 3) * 0.08f) * ((maxInARow - row3) / 2))) * powerUpSave;

        
        // random probability chance
        float chance = Random.Range(0.0f, 1.0f);

        // set flag counter and return the int id of the powerup to spawn
        if (chance <= probabilities[pu1])
        {
            SetProbabilityFlags(pu1);
            return puMapper[pu1];
        }


        else if (chance <= probabilities[pu2])
        {
            SetProbabilityFlags(pu2);
            return puMapper[pu2];
        }

        else if (chance <= probabilities[pu3])
        {
            SetProbabilityFlags(pu3);
            return puMapper[pu3];
        }

        else
        {
            if (powerUpSave == 0)
            {
                powerUpSave = 1;
                powerupSaves--;
            }
            SetProbabilityFlags("Health");
            return 3;
        }
        
    }

    /**
     * Sets the counter for which probability was most recently spawned
     * 
     * @param powerup The id of the powerup just spawned.
     */
    private void SetProbabilityFlags(string powerup)
    {
        if (powerup == "Insta-Kill")
        {
            shootSpawnedInRow = 0f;
            invincSpawnInRow = 0f;
            instaSpawnedInRow++;
        }
        else if (powerup == "Shooting")
        {
            instaSpawnedInRow = 0f;
            invincSpawnInRow = 0f;
            shootSpawnedInRow++;
        }
        else if (powerup == "Invincible")
        {
            instaSpawnedInRow = 0f;
            shootSpawnedInRow = 0f;
            invincSpawnInRow++;
        }
        else
        {
            instaSpawnedInRow = 0f;
            shootSpawnedInRow = 0f;
            invincSpawnInRow = 0f;
        }
    }

    /**
     * Calculates the next time to spawn a powerup.
     */
    private void CalculateNextSpawnTime()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;
            if (timer >= spawnTime)
            {
                SpawnPowerup();
                CalculateNextSpawnTime();
                timer = 0;
            }
        }
        
    }
}
