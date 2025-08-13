using UnityEngine;

public class DestroyableProjectile : MonoBehaviour
{
    public bool Destroyable = true;
    public void Destroy()
    {
        if (Destroyable)
        {
            Destroy(gameObject);
        }
    }
}
