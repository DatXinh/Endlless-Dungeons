using UnityEngine;

public class AutoDestroyExplosion : MonoBehaviour
{
    public float destroyDelay = 2f; // Time in seconds before the explosion is destroyed

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
