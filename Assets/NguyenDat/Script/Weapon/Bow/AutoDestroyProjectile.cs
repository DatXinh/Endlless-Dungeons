using UnityEngine;

public class AutoDestroyProjectile : MonoBehaviour
{
    [Header("Projectile Identity")]
    public bool IsPlayer = false;
    public bool IsEnemy = false;

    [Header("Projectile Behavior")]
    public bool canPierce = false;

    [Header("LifeTime")]
    public float lifeTime = 1f;

    private SpawnAtDestroy spawnAtDestroy;

    private void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    // Tự hủy khi chạm vào Wall, Enemy hoặc Boss
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (canPierce)
        {
            return;
        }

        if (IsPlayer && (collision.CompareTag("Enemy") || collision.CompareTag("Boss")))
        {
            Destroy(gameObject);
        }
        else if (IsEnemy && collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (spawnAtDestroy == null)
        {
            spawnAtDestroy = GetComponent<SpawnAtDestroy>();
            if (spawnAtDestroy == null)
            {
                return;
            }
            if (spawnAtDestroy != null)
            {
                spawnAtDestroy.TriggerDestroy();
            }
        }
    }
}
