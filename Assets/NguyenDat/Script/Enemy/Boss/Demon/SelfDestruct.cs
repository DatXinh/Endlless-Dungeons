using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public SpawnBoss SpawnBoss;

    private void Start()
    {
        SpawnBoss = GetComponentInParent<SpawnBoss>();
    }
    public void Selfdestroy()
    {
        if (transform.parent != null)
        {
            if (SpawnBoss != null)
            {
                SpawnBoss.enablePortal();
            }
            Destroy(transform.parent.gameObject);
        }
        else
        {
            if (SpawnBoss != null)
            {
                SpawnBoss.enablePortal();
            }
            Destroy(gameObject);
        }
    }
}
