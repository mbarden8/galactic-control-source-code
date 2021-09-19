using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles how the enemy bullets behave and interact with the player.
 */
public class EnemyLaserScript : MonoBehaviour
{
    public float lifespan = 4f;
    public float bulletSpeed = 300f;
    public float damage = 1.5f;
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
     * Checks for when this bullet collides with the player.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject other = collision.gameObject;
            other.GetComponent<PlayerController>().StartDamage(damage);
            Destroy(this.gameObject);
        }
    }

    /**
     * Returns how much damage this laser does. To be used by the player
     * to tell how much damage to take.
     */
    public float GetDamage()
    {
        return damage;
    }
}
