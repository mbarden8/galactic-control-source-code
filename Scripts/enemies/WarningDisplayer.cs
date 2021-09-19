using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Displays the warning icon that indicates where the enemies are going to
 * be spawning off-screen.
 */
public class WarningDisplayer : MonoBehaviour
{

    public float destroyTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
