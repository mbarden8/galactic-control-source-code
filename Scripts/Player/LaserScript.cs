
using UnityEngine;

/**
 * Handles how the player's bullets behave after it has been spawned in.
 */

public class LaserScript : MonoBehaviour
{
    public float lifespan = 4f;
    public float bulletSpeed = 300f;
    private float damage = 5f;
    private Rigidbody2D rb;

    /**
     * Called before the first update frame.
     */
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, lifespan);
        rb.AddForce(transform.up * bulletSpeed);
    }

    /**
     * Sets the damage this bullet will do, set by the player
     * and used to change bullet damage depending on powerup states.
     * 
     * @param dmg The amount of damage this bullet will do.
     */
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    /**
     * Returns how much damage this laser does. To be used by the enemy scripts
     * when they are taking damage.
     */
    public float GetDamage()
    {
        return damage;
    }

}
