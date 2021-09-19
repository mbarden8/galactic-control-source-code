using UnityEngine;
using System.Collections;

/**
 * The Shooting (Supercharged) powerup fires a continuous stream of lasers
 * from three sides of the player's ship while it is active.
 * Player still has the ability to control their ship.
 */
public class ShootingPowerup : MonoBehaviour
{
    private PlayerController cont;
    public float shootDelay = 0.1f;

    private void OnEnable()
    {
        cont = this.GetComponent<PowerupScript>().GetPlayer().GetComponent<PlayerController>();
        StartCoroutine(SpawnAllLasers());
    }

    private IEnumerator SpawnAllLasers()
    {
        while (true)
        {
            cont.PowerupShoot();
            yield return new WaitForSeconds(shootDelay);
        }
    }

}
