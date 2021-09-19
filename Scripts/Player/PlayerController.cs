using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

/**
 * Handles all basic player functionality, including but not limited to:
 * movement, shooting, aiming, and tracking powerups.
 */
public class PlayerController : MonoBehaviour
{
    
    public Camera cam;
    public float force = 5f;
    public float maxPlayerForce = 100f;
    public GameObject laser;
    public float fireRate = 0.1f;
    public float lives = 3f;
    public float invincDuration = 3f;
    public InGameUIHandler ui;
    public EnemySpawning spawner;
    public float bulletDmg = 25f;
    public AudioManager am;

    private Animator anim;
    private Vector2 mousePos;
    private Vector2 lookDir;
    private Rigidbody2D rb;
    private float angle;
    private float timeSinceShot = 0f;
    private bool canTakeDmg = false;
    private Dictionary<string, PowerupTracker> activePowerups;
    private bool invincPowerup = false;
    public Transform[] laserSpawns;

    /**
     * Called before first frame update.
     */
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponentInChildren<Animator>();
        activePowerups = new Dictionary<string, PowerupTracker>();
    }

    /**
     * Handles the player shooting. Calculates time delay between shots,
     * as well as applying physics force to the player and pushing them
     * backward. Intensity of force can be adjust using the force variable
     * and the fire reate can be adjusted using the fireRate variable.
     */
    private void Shoot()
    {
        timeSinceShot += Time.deltaTime;

        if (Input.GetButton("Fire1") && timeSinceShot >= fireRate)
        {
            Vector2 pos = -transform.up;
            rb.AddForce(pos * force);
            GameObject leftB = Instantiate(laser, laserSpawns[0].position, laserSpawns[0].rotation);
            GameObject rightB = Instantiate(laser, laserSpawns[1].position, laserSpawns[1].rotation);

            // set the damage of our bullets
            leftB.GetComponent<LaserScript>().SetDamage(bulletDmg);
            rightB.GetComponent<LaserScript>().SetDamage(bulletDmg);
            am.PlayShootNoise();
            
            timeSinceShot = 0;
        }
    }

    public void PowerupShoot()
    { 
        for (int i = 2; i < laserSpawns.Length; i++)
        {
            GameObject l = Instantiate(laser, laserSpawns[i].position, laserSpawns[i].rotation);
            l.GetComponent<LaserScript>().SetDamage(bulletDmg);
        }
    }

    /**
     * Increments the player speed. Called when a new wave starts.
     */
    public void IncreasePlayerForce()
    {
        if (force < maxPlayerForce)
        {
            force++;
        }
    }

    /**
     * Sets the bulletDamage variable, used by powerup scripts to change
     * the amount of damage the player's bullets do.
     * 
     * @param dmg The amount of damage the bullets do.
     */
    public void SetBulletDamage(float dmg)
    {
        bulletDmg = dmg;
    }

    /**
     * Returns the amount of damage the bullets do.
     */
    public float GetBulletDamage()
    {
        return bulletDmg;
    }

    /**
     * Alerts the enemy spawner that an enemy has been killed.
     */
    public void UpdateSpawner()
    {
        spawner.SetEnemyCounter();
    }

    /**
     * Sets the invincibility status of the player. Used by powerups.
     */
    public bool SetInvinc(bool invinc)
    {
        invincPowerup = invinc;
        return invincPowerup;
    }

    /**
     * Checks to see if the invincibility powerup is activated.
     */
    public bool InvincActive()
    {
        return invincPowerup;
    }

    /**
     * Checks to see if the player just took damage or not.
     */
    private bool JustTookDamage()
    {
        return canTakeDmg;
    }

    /**
     * Handles the player losing a life. If the player is not currently invincible,
     * they lose a life and go invincible for invincDuration amount of time.
     * The ship will start blinking, signaling it can't take any damage.
     * Invinc duration can be adjusted using the invincDuration variable.
     * 
     * @param dmg The amount of damage being taken.
     */
    public IEnumerator TakeDamage(float dmg)
    {
        if (this.JustTookDamage() || invincPowerup)
        {
            // end the coroutine
            yield break;
        }

        

        else
        {
            canTakeDmg = true;
            anim.SetTrigger("invinc");
            lives -= dmg;
            ui.UpdateLives(lives);
            am.PlayPlayerHit();

            if (lives <= 0)
            {
                anim.SetTrigger("die");
                Invoke("PlayDeathSound", 0.25f);
                Destroy(this.gameObject, 0.5f);

                yield return new WaitForSeconds(0.4f);
                ui.EnableGameOver();
            }

            yield return new WaitForSeconds(invincDuration);

            canTakeDmg = false;
            anim.SetTrigger("stopinvinc");
        }
    }

    private void PlayDeathSound()
    {
        am.PlayPlayerDeath();
    }

    public void EnableUIPowerupText(string id)
    {
        ui.ShowPowerupText(id);
    }

    public void DisableUIPowerupText()
    {
        ui.HidePowerupText();
    }

    public void IncreaseHealth(float health)
    {
        lives += health;
        if (lives > 3f)
        {
            lives = 3f;
        }
        ui.UpdateLives(lives);
    }

    /**
     * Called when a powerup is picked up by the player. If this type of powerup
     * is being activated for the first time, the powerup id is added to the 
     * activePowerups dictionary and a new PowerupTracker object is stored as the 
     * id's value. If it already has been picked up, the PowerupTracker object
     * is updated to hold the new powerup GameObject.
     * 
     * @param id The string id of the powerup.
     */
    public void PowerupActivated(string id, GameObject powerup)
    {
        if (activePowerups.ContainsKey(id))
        {
            activePowerups[id].SetPowerup(powerup);
            activePowerups[id].SetActive(true);
        }
        else
        {
            activePowerups.Add(id, new PowerupTracker(powerup, true));
        }
    }

    /**
     * Called when a powerup is deactivated. Updates the activePowerups
     * dictionary accordingly.
     * 
     * @param id The string id of the powerup that was just deactivated.
     */
    public void PowerupDeactivated(string id)
    {
        Destroy(activePowerups[id].GetPowerup());
        activePowerups[id].SetActive(false);
    }

    /**
     * Determines if the given powerup is active within the scene.
     * 
     * @param id The id of the powerup we are checking for.
     * @return True if the powerup is active within the scene, false otherwise.
     */
    public bool PowerupIsActive(string id)
    {
        if (activePowerups.ContainsKey(id))
        {
            return activePowerups[id].IsActive();
        }

        return false;
    }

    /**
     * Starts the coroutine for damage. Called by enemies.
     * 
     * @param dmg The amount of damage to be taken.
     */
    public void StartDamage(float dmg)
    {
        StartCoroutine(TakeDamage(dmg));
    }

    /**
     * Called once per frame
     */
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    /**
     * Similar to update but to be used for physics calculations
     */
    private void FixedUpdate()
    {
        lookDir = mousePos - rb.position;
        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        Shoot();
           
        // for colliding with edges of screen just put collision tiles on edge
    }
}
