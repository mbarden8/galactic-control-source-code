using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/**
 * The PowerupScript controls all basic functionality of the powerups.
 * Including: the amount of time before a powerup despawns, the amount of time
 * a powerup is activated for upon pickup, and knowing which powerup
 * was just picked up, in order to activate the script with it.
 */
public class PowerupScript : MonoBehaviour
{

    public float duration = 20f;
    public float availability = 10f;
    public float animDelay = 1f;
    public string id;
    public float deactivationStartPercentage = 0.1f;
    public float despawnStartPercentage = 0.25f;
    private GameObject player;
    private Animator anim;
    private Color color;
    private PlayerController cont;
    private Light2D lt;

    private float timeSinceSpawn = 0f;
    private float timeSinceActivated = 0f;
    private bool pickedUp = false;
    private new SpriteRenderer renderer;
    private bool deactivationStarted = false;

    public float minAlpha = 0.2f;
    private float maxAlpha = 1f;
    private float t = 0f;
    public float alphaChangeSpeed = 0.01f;

    private void Start()
    {
        renderer = this.GetComponentInChildren<SpriteRenderer>();
        anim = this.GetComponentInChildren<Animator>();
        color = renderer.color;

        StartCoroutine(PlayShineAnimation());
        deactivationStartPercentage = duration * deactivationStartPercentage; // 20 * 0.1 = 2
        despawnStartPercentage = availability * despawnStartPercentage;
        lt = this.GetComponentInChildren<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pickedUp && collision.tag == "Player")
        {
            player = collision.gameObject;
            if (id != "Health")
            {
                renderer.enabled = false;
            }
            
            pickedUp = true;
            lt.enabled = false;
            

            cont = player.GetComponent<PlayerController>();
            cont.EnableUIPowerupText(id);
            StartCoroutine(CheckOtherPowerups());
            cont.am.PlayPowerupPickup();
        }
    }

    private IEnumerator CheckOtherPowerups()
    {
        if (cont.PowerupIsActive(id))
        {
            cont.PowerupDeactivated(id);
            yield return new WaitForFixedUpdate();
        }
        cont.PowerupActivated(id, this.gameObject);
        ActivatePowerup();
    }

    /**
     * Returns the player gameobject.
     */
    public GameObject GetPlayer()
    {
        return player;
    }

    /**
     * Activates the powerup that this object is
     */
    private void ActivatePowerup()
    {
        if (id == "Insta-Kill")
        {
            this.GetComponent<InstaKillPowerup>().enabled = true;
        }
        else if (id == "Health")
        {
            this.GetComponent<HealthPowerup>().enabled = true;
        }
        else if (id == "Invincible")
        {
            this.GetComponent<InvincPowerup>().enabled = true;
        }
        else if (id == "Supercharged")
        {
            this.GetComponent<ShootingPowerup>().enabled = true;
        }
    }

    /**
     * Starts the deactivation process for the current active powerup.
     */
    private void DeactivatePowerup()
    {
        if (id == "Insta-Kill")
        {
            this.GetComponent<InstaKillPowerup>().StartDeactivation();
        }

        else if (id == "Invincible")
        {
            this.GetComponent<InvincPowerup>().StartDeactivation();
        }
    }

    /**
     * Plays the powerup "shine" animation. Pauses for animDelay before
     * each loop.
     */
    private IEnumerator PlayShineAnimation()
    {
        while (true)
        {
            anim.SetTrigger("play");
            yield return new WaitForSeconds(animDelay);

        }
    }

    /**
     * Equation for handling the blink speed of a powerup that is disappearing
     * from the scene if the player has not picked it up.
     * 
     * @param speed The speed of the blinking.
     */
    private float PowerupDespawnGraph(float speed)
    {
        return speed / 2;
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        // give indicator that powerup is despawning by adjusting the alpha levels
        // for a "blink" effect
        if (availability - timeSinceSpawn <= despawnStartPercentage && !pickedUp)
        {
            alphaChangeSpeed += PowerupDespawnGraph(alphaChangeSpeed) * Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, minAlpha, t);
            renderer.color = color;
            t += alphaChangeSpeed;
            if (t > 1f)
            {
                float temp = maxAlpha;
                maxAlpha = minAlpha;
                minAlpha = temp;
                t = 0f;
            }
        }

        // despawn powerup
        if (!pickedUp && timeSinceSpawn >= availability)
        {
            
            Destroy(this.gameObject);
        }

        else if (pickedUp)
        {
            color.a = 1f;
            renderer.color = color;
            timeSinceActivated += Time.deltaTime;
            
            // if powerup was picked up, start the deactivation sequence
            // once it has been active for timeSinceActivated time
            if (!deactivationStarted && 
                (duration - timeSinceActivated) <= deactivationStartPercentage)
            {
                DeactivatePowerup();
                deactivationStarted = true;
            }

            if (timeSinceActivated >= duration)
            {
                cont.PowerupDeactivated(id);
                cont.DisableUIPowerupText();
                Destroy(this.gameObject);
            }
        }

    }
}
