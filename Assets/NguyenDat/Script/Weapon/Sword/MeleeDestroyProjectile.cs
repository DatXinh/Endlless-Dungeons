using UnityEngine;

public class MeleeDestroyProjectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroyableProjectile destroyableProjectile = collision.GetComponent<DestroyableProjectile>();
        if (collision.CompareTag("EnemyProjectile"))
        {
            destroyableProjectile.Destroy();
        }
    }
}
