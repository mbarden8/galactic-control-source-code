using System.Collections.Generic;
using UnityEngine;

/**
 * Handles calculating what enemy needs to be spawned next based off
 * probability formulas and cumulative density function.
 */
public class EnemySpawnProbabilities: MonoBehaviour
{

    public int laserSpawnStart = 4;
    public int shipSpawnStart = 6;
    private int lasersToSpawn = 0;
    private int shipsToSpawn = 0;
    private int baseShipsToSpawn = 0;
    private int baseLasersToSpawn = 0;
    private int beauSpawnChained = 0;
    private Dictionary<string, float> probabilities;

    private void Start()
    {
        probabilities = new Dictionary<string, float>
        {

            {"beaufort", 1f },
            {"ship", 0f},
            {"laser", 0f }

        };
    }

    /** 
     * Calculates the next enemy to be spawned based on how many
     * ships and lasers need to be spawned in this wave, as well
     * as the number of beauforts who have been spawned in a row.
     * 
     * @param remainingEnemies The number of enemies left to be spawned.
     * @param currentWave The current wave.
     * @return The id of the enemy to spawn.
     */
    public int EnemyToSpawn(int remainingEnemies, int currentWave)
    {
        // basic cumulative density function
        probabilities["ship"] = (shipsToSpawn * (beauSpawnChained / 2)) / remainingEnemies; // (2/16)
        probabilities["laser"] = 
            probabilities["ship"] + ((lasersToSpawn * (beauSpawnChained / 2)) / remainingEnemies); // (2/16) + (1/16) = (3/16)

        // beaufort = (1 - (3/16))

        // probability to be compared w/ our cdf
        float chance = Random.Range(0.0f, 1.0f);

        if (chance <= probabilities["ship"])
        {
            shipsToSpawn--;
            if (shipsToSpawn > lasersToSpawn)
            {
                beauSpawnChained = shipsToSpawn;
            }
            else
            {
                beauSpawnChained = lasersToSpawn;
            }
            
            return 2;
        }
        else if (chance <= probabilities["laser"])
        {
            lasersToSpawn--;
            if (shipsToSpawn > lasersToSpawn)
            {
                beauSpawnChained = shipsToSpawn;
            }
            else
            {
                beauSpawnChained = lasersToSpawn;
            }
            return 1;
        }
        else
        {
            if (currentWave >= laserSpawnStart)
            {
                beauSpawnChained++;
            }

            return 0;
        }

    }

    /**
     * Increases the number of "special" enemies that need to be
     * spawned in the upcoming wave.
     * 
     * @param currentWave The new wave that is being started.
     */
    public void IncrementSpecialEnemies(int currentWave)
    {
        if (currentWave >= laserSpawnStart)
        {
            baseLasersToSpawn++;
            lasersToSpawn = baseLasersToSpawn;
        }

        if (currentWave >= shipSpawnStart)
        {
            baseShipsToSpawn++;
            shipsToSpawn = baseShipsToSpawn;
            
        }

    }


}
