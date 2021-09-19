using UnityEngine;

/**
 * The invinciblity powerup causes the player to take no damage for its duration.
 * The powerup also causes enemies to be destroyed upon collision with the player.
 */
public class InvincPowerup : MonoBehaviour
{
    private GameObject player;
    private PlayerController cont;
    private Animator anim;

    /**
     * Called when this script is enabled.
     */
    private void OnEnable()
    {
        player = this.GetComponent<PowerupScript>().GetPlayer();
        cont = player.GetComponent<PlayerController>();
        anim = player.GetComponentInChildren<Animator>();
        anim.SetTrigger("powered");

        cont.SetInvinc(true);
    }

    /**
     * Starts the deactivation process for the powerup.
     */
    public void StartDeactivation()
    {
        anim.SetTrigger("power-fade");
        Invoke("AlmostExpired", 2.75f);
    }

    /**
     * Triggers animation that speeds up the blinking effect because
     * I was too lazy to code a solution with the different sprites.
     */
    private void AlmostExpired()
    {
        anim.SetTrigger("power-gone");
    }

    /**
     * Called when this script is destroyed.
     */
    private void OnDestroy()
    {
        // if cont is null then this script was never enabled, thus we
        // can not call ondestroy
        if (cont != null)
        {
            anim.SetTrigger("stop-powered");
            cont.SetInvinc(false);
        }
        
    }
}
