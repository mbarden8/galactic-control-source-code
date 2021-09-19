
using UnityEngine;

/**
 * Keeps the player within the screen, preventing them from disappearing off the edge.
 */
public class ScreenColliderSpawner : MonoBehaviour
{

    public Camera MainCamera;

    // adjusts the bounds the player can go, from min to max
    public Vector2 viewportClamp = new Vector2(0.1f, 0.9f);


    /**
     * Called once per frame, calculates the player's position relative to the
     * camera viewport and clamps the player's position according to the
     * bounds set by viewportClamp. Forcing the object to have a viewport coordinate
     * between (viewportClamp.x, viewportClamp.y), and therefore remain
     * on the screen.
     */
    void Update()
    {
        Vector3 pos = MainCamera.WorldToViewportPoint(transform.position);

        pos.x = Mathf.Clamp(pos.x, viewportClamp.x, viewportClamp.y);
        pos.y = Mathf.Clamp(pos.y, viewportClamp.x, viewportClamp.y);

        transform.position = MainCamera.ViewportToWorldPoint(pos);
    }
}
