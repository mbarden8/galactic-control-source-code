using UnityEngine.Rendering;
using UnityEngine;

/**
 * The health powerup grants one life to the player.
 */
public class HealthPowerup : MonoBehaviour
{
    public float healing = 1f;
    

    private void OnEnable()
    {
        this.GetComponent<PowerupScript>().GetPlayer().GetComponent<PlayerController>().IncreaseHealth(healing);
        this.GetComponentInChildren<Animator>().SetTrigger("pickup");
    }

}
