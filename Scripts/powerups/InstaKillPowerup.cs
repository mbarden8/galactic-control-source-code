using UnityEngine;
using UnityEngine.Rendering;

/**
 * The insta-kill powerup gives the player a one-shot kill with their lasers.
 * In order to indicate the powerup, the post-processing is amplified on-screen
 * and while blink away when the powerup will deactivate soon.
 */
public class InstaKillPowerup : MonoBehaviour
{

    private PlayerController playerCont;
    private float startBulletDmg;
    private float startVolume;
    private Volume volume;

    public float minIntensity = 0.6f;
    private float maxIntensity = 1f;
    private float t = 0f;
    public float blinkSpeed = 0.01f;
    private bool deactivation = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCont = this.GetPlayerController();
        
        startBulletDmg = playerCont.GetBulletDamage();
        // set to large number to insta-kill enemies
        if (playerCont != null)
        {
            playerCont.SetBulletDamage(10000f);
        }
        volume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Volume>();
        startVolume = volume.weight;
        volume.weight = 1f;
    }
    
    /**
     * Time wave that the powerup blinks to show player it is deactivating.
     */
    private float PowerupBlinkGraph(float speed)
    {
        return speed / 2;
    }

    /**
     * Starts the deactivation process (animating the screen with blinking).
     */
    public void StartDeactivation()
    {
        deactivation = true;
    }

    /**
     * Retrieves the playercontroller script attached to the player
     * gameobject. Allows this script to manipulate bullet dmg.
     * 
     * @return The playercontroller script.
     */
    private PlayerController GetPlayerController()
    {
        return this.GetComponent<PowerupScript>().GetPlayer().
            GetComponent<PlayerController>();
    }

    /**
     * Revert bullet damage back to original state when this object
     * is destroyed.
     */
    private void OnDestroy()
    {
        if (playerCont != null)
        {
            playerCont.SetBulletDamage(startBulletDmg);
            volume.weight = startVolume;
        }
        
        
    }

    private void Update()
    {
        if (deactivation)
        {
            blinkSpeed += PowerupBlinkGraph(blinkSpeed) * Time.deltaTime;

            volume.weight = Mathf.Lerp(maxIntensity, minIntensity, t);
            t += blinkSpeed;

            if (t > 1f)
            {
                float temp = maxIntensity;
                maxIntensity = minIntensity;
                minIntensity = temp;
                t = 0f;
            }
        }
        
    }
}
