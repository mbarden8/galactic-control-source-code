using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Handles the behavior of the enemy ship. The enemy ship
 * will move away from the player if the player gets too close to it.
 * The enemy ship shoots at the player from a distance.
 */
public class EnemyShipScript : MonoBehaviour
{

    public float maxPlayerDist = 5f;
    public float moveSpeed = 2f;
    private float currentDist;
    public GameObject player;
    public GameObject enLaser;
    public Transform lSpawn;
    public Transform rSpawn;
    private bool isDead = false;
    public float health = 300f;
    public float startDelay = 1.5f;
    public float shootDelay = 0.25f;
    public float moveDelay = 2f;
    private bool onScreen = false;
    private bool canMove = false;
    public float followSpeed = 200f;
    public float hitDelay = 1.5f;

    public float collisionDamage = 0.5f;

    private new Camera camera;
    private Animator anim;
    private float hitFlag = 0f;

    private ScreenColliderSpawner screenCollider;
    private float willShoot;
    private Rigidbody2D rb;
    private float angle;
    private Vector2 lookDir;


    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        screenCollider = this.GetComponent<ScreenColliderSpawner>();
        anim = this.GetComponentInChildren<Animator>();

        InvokeRepeating("Shoot", startDelay, shootDelay);
        Invoke("EnableMovement", moveDelay);
    }

    /**
     * Sets the target player object and camera to reference.
     * 
     * @param player The target player object.
     * @param cam The camera to reference.
     */
    public void InitializeReferences(GameObject player, Camera cam)
    {
        if (player != null)
        {
            this.player = player;
        }

        this.camera = cam;
    }

    void EnableMovement()
    {
        canMove = true;
    }

    private void MoveIn()
    {
        if (canMove)
        {
            Vector3 pos = camera.WorldToViewportPoint(transform.position);

            if (pos.x < 0.1f || pos.x > 0.9f || pos.y < 0.1f || pos.y > 0.9f)
            {
                float step = followSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
            }

            else
            {
                screenCollider.enabled = true;
                screenCollider.MainCamera = camera;
                rb.AddForce(-transform.up * moveSpeed);
                onScreen = true;
            }
        }
    }

    /**
     * Calculates the distance of this enemy ship from the player.
     * 
     * @return The distance from the player.
     */
    private float CalculateDistanceFromPlayer()
    {
        Vector2 pos = this.transform.position;
        Vector2 other = player.transform.position;

        float dist = Mathf.Sqrt(Mathf.Pow(pos.x - other.x, 2) + Mathf.Pow(pos.y - other.y, 2));

        return dist;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (!isDead)
            {
                if (collision.tag == "PlayerBullet")
                {
                    this.TakeDamage(collision.GetComponent<LaserScript>().GetDamage());
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("E_Ship_Hit") && hitFlag == 0)
                    {
                        anim.SetTrigger("hit");
                        hitFlag = 1;
                    }

                    Destroy(collision.gameObject);
                }


                else if (collision.tag == "Player" && 
                    collision.gameObject.GetComponent<PlayerController>().InvincActive())
                {
                    this.HitPlayer(collision.gameObject.transform);
                    this.TakeDamage(1000000000f);
                }

                else if (collision.tag == "Player")
                {
                    canMove = false;
                    this.HitPlayer(collision.gameObject.transform);
                    collision.gameObject.GetComponent<PlayerController>().TakeDamage(collisionDamage);
                    Invoke("EnableMovement", 1.5f);
                }
                
            }
        }
    }

    /**
     * Gives effect that enemy ship is bouncing off player ship
     * after a collision.
     * 
     * @param other The transform of the player
     */
    private void HitPlayer(Transform other)
    {
        Vector3 playerPos = other.position;
        Vector2 posDiff = new Vector2(
            this.transform.position.x - playerPos.x,
            this.transform.position.y - playerPos.y);

        rb.AddRelativeForce(posDiff * 250);

    }

    /**
     * Takes damage, subtracts the current beaufort health by the amount
     * of damage received from the player ship.
     * 
     * @param dmg The amount of damage to be taken.
     * @return The current remaining health
     */
    private float TakeDamage(float dmg)
    {

        this.health -= dmg;
        if (this.health <= 0 && !isDead)
        {
            isDead = true;
            player.GetComponent<PlayerController>().UpdateSpawner();
            anim.SetTrigger("die");
            Destroy(this.gameObject, 0.75f);
            Invoke("PlayDeathSound", 0.25f);
        }

        return this.health;

    }

    private void PlayDeathSound()
    {
        player.GetComponent<PlayerController>().am.PlayEnemyShipExplode();
    }

    /**
     * Handles the enemy ship shooting. There is always a chance
     * the enemy does not shoot when this function is called.
     */
    private void Shoot()
    {
        if (player == null) return;
        willShoot = Random.Range(0.0f, 1.0f);
        if (!isDead && willShoot <= 0.75f)
        {
            Instantiate(enLaser, lSpawn.position, lSpawn.rotation);
            Instantiate(enLaser, rSpawn.position, rSpawn.rotation);
        }
        
    }

    private void Update()
    {

        if (player == null) return;

        currentDist = CalculateDistanceFromPlayer();

        if (onScreen)
        {

            if (currentDist < maxPlayerDist)
            {
                Vector2 pos = -transform.up;
                rb.AddForce(pos * moveSpeed);
            }
        }
        else
        {
            MoveIn();
        }

        // keep track of how long to delay before allowing the hit effect
        if (hitFlag >= 1 && hitFlag < hitDelay)
        {
            hitFlag += Time.deltaTime;
            if (hitFlag >= hitDelay)
            {
                hitFlag = 0;
            }
        }


    }

    private void FixedUpdate()
    {
        if (player == null) return;
  
        lookDir = player.transform.position - this.transform.position;
        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        
        
    }

}
