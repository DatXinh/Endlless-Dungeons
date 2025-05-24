using UnityEngine;

public class TestWeaponLockEnemy : MonoBehaviour
{
    // Enemy gần nhất sẽ được truyền từ TestPlayerMove
    [HideInInspector]
    public Transform nearestEnemy;

    public float searchRadius = 10f;      // Bán kính tìm enemy (không còn dùng để tìm enemy ở đây)
    public LayerMask enemyLayer;          // Layer của enemy, set trong Inspector

    void Update()
    {
        if (nearestEnemy != null)
        {
            Vector3 direction = nearestEnemy.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Clamp góc quay Z trong [-90, 90]
            angle = Mathf.Clamp(angle,-360,360);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    // Vẽ bán kính tìm enemy trong Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
