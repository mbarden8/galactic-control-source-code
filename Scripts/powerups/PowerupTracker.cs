using UnityEngine;

/**
 * PowerupTracker assists in tracking what powerups are currently active
 * in the game.
 */
public class PowerupTracker
{

    private GameObject powerup;
    private bool active;

    /**
     * Creates a new PowerupTracker object.
     * 
     * @param powerup The powerup GameObject this is currently holding.
     * @param active Whether the powerup is currently active or not.
     */
    public PowerupTracker(GameObject powerup, bool active)
    {
        this.powerup = powerup;
        this.active = active;
    }

    /**
     * Returns the powerup GameObject.
     */
    public GameObject GetPowerup()
    {
        return this.powerup;
    }

    /**
     * Returns whether the powerup is active or not.
     */
    public bool IsActive()
    {
        return this.active;
    }

    /**
     * Sets the powerup active flag based on the parameter.
     * 
     * @param active Whether the powerup is active or not.
     */
    public void SetActive(bool active)
    {
        this.active = active;
    }

    /**
     * Sets the powerup variable to hold a new powerup GameObject.
     * 
     * @param powerup The new powerup GO to be tracked.
     */
    public void SetPowerup(GameObject powerup)
    {
        this.powerup = powerup;
    }

}
