using UnityEngine;


/**
 * Handles the behavior of the laser that takes up a section
 * of the screen, prohibiting the player from entering that 
 * part of the screen and forcing them into a more confined area.
 */
public class SpaceLaserScript : MonoBehaviour
{
    public float windUpTime = 2f;
    public float activeTime = 3f;
    public float dmg = 1.5f;

    private Animator anim;
    private bool isActive = false;
    private new BoxCollider2D collider;
    private GameObject player;
    private CircleCollider2D otherCollider;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        collider = this.GetComponent<BoxCollider2D>();
        
        Invoke("SetActive", windUpTime);
    }

    /**
     * Sets the target player object.
     * 
     * @param player The target player object.
     */
    public void SetPlayerObject(GameObject player)
    {
        if (player != null)
        {
            this.player = player;
            otherCollider = player.GetComponent<CircleCollider2D>();
        }
    }

    /**
     * Checks to see if this sprite is overlapping with the player sprite.
     * If it is, we damage the player.
     */
    private void CheckIntersectingBounds()
    {
        if (isActive && player != null && collider.bounds.Intersects(otherCollider.bounds))
        {
            player.GetComponent<PlayerController>().StartDamage(dmg);
        }
    }

    /**
     * Sets the laser to activation mode, where it can now
     * damage and hurt the player.
     */
    private void SetActive()
    {
        anim.SetTrigger("activate");
        isActive = true;
        Destroy(this.gameObject, activeTime);
    }

    private void Update()
    {
        CheckIntersectingBounds();
    }

}
