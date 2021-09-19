using UnityEngine;

/**
 * Controls the enemy pathfinding ai
 */
public class PathFinderAI : MonoBehaviour
{

    private Transform target;
    public float followSpeed = 2f;

    /**
     * Sets the target player transform.
     * 
     * @param player The target player object.
     */
    public void SetPlayerObject(GameObject player)
    {
        if (player != null)
        {
            this.target = player.transform;
        }
    }

    /**
     * Returns the player object.
     */
    public GameObject GetPlayer()
    {
        return target.gameObject;
    }

    public void SetFollowSpeed(float followSpeed)
    {
        this.followSpeed = followSpeed;
    }


    private void Update()
    {
        if (target == null) return;
        float step = followSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);
    }
}
