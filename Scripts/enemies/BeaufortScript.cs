
using UnityEngine;

/**
 * Beaufort is really special. He was created in three minutes by the greatest
 * pixel artist of all time (me, of course). Beaufort is an enemy who chases
 * the player around, trying to deal damage to the player. When Beaufort hits
 * the player, he bounces off him and gives the player some i-frames, allowing the player
 * a chance to destroy Beaufort before another fatal run.
 */
public class BeaufortScript : MonoBehaviour
{

    public float health = 100f;
    public float damage = 1f;
    public float moveDelay = 2f;
    private bool canTakeDmg = false;
    private float hitFlag = 0f;
    public float hitDelay = 1.5f;
    public float dmgThrust = 2f;
    public float moveAfterHit = 1f;
    private Animator anim;
    private Rigidbody2D rb;
    private PathFinderAI ai;
    private bool isDead = false;
    private float followSpeed = 2f;
    private GameObject player;
    private AudioManager am;

    private void Start()
    {
        Invoke("EnableMovement", moveDelay);
        anim = this.GetComponentInChildren<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        ai = this.GetComponent<PathFinderAI>();
    }

    /**
     * Sets target player object and movement speed.
     */
    public void Init(GameObject player, float followSpeed, AudioManager am)
    {
        if (player != null)
        {
            this.player = player;
        }
        this.am = am;
        this.followSpeed = followSpeed;
    }

    /**
     * Enables the pathfinding component of this enemy.
     */
    private void EnableMovement()
    {
        ai.enabled = true;
        ai.SetPlayerObject(player);
        ai.SetFollowSpeed(this.followSpeed);
        canTakeDmg = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead)
        {
            if (collision.tag == "PlayerBullet")
            {
                this.TakeDamage(collision.GetComponent<LaserScript>().GetDamage());
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Beaufort-hit") && hitFlag == 0)
                {
                    anim.SetTrigger("hit");
                    hitFlag = 1;
                }

                Destroy(collision.gameObject);
            }
            else if (collision.tag == "Player")
            {

                PlayerController other = collision.gameObject.GetComponent<PlayerController>();
                if (other.InvincActive())
                {
                    this.TakeDamage(100000000f);
                }
                else
                {
                    other.StartDamage(damage);
                    
                }
                this.HitPlayer(other.gameObject.transform);


            }
        }
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
        if (canTakeDmg)
        {
            this.health -= dmg;
            if (this.health <= 0 && !isDead)
            {
                isDead = true;
                anim.SetTrigger("die");
                ai.GetPlayer().GetComponent<PlayerController>().UpdateSpawner();
                Destroy(this.gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
                Invoke("PlayDeathSound", 0.25f);
                
            }
        }

        return this.health;

    }

    private void PlayDeathSound()
    {
        am.PlayBeaufortExplosion();
    }

    /**
     * Makes Beaufort bounce off the player once he hits the player.
     * This allows the player to have time to orient themselves
     * and prepare for another onslaught of beaufort.
     * 
     * @param other The transform of the player
     */
    private void HitPlayer(Transform other)
    {
        Vector3 playerPos = other.position;
        Vector2 posDiff = new Vector2(
            this.transform.position.x - playerPos.x, 
            this.transform.position.y - playerPos.y);

        // add force relative to beaufort's position from the player
        rb.AddRelativeForce(posDiff * dmgThrust);

        // temporarily disable ai pathfinding
        ai.enabled = false;
        Invoke("EnableMovement", moveAfterHit);
    }

    private void Update()
    {
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

}
