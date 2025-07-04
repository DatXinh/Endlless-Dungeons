using UnityEngine;

public class SpawnToxic : MonoBehaviour
{
    public GameObject spawnPrefab;
    private EnemyHealth enemyHealth;
    private bool hasSpawned = false;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (!hasSpawned && enemyHealth.currentHP <= 0)
        {
            hasSpawned = true;

            GameObject clone = Instantiate(spawnPrefab, transform.position, transform.rotation);

            // Gán tên giống object cũ nếu cần debug
            clone.name = spawnPrefab.name;

            // Sau khi spawn prefab, tự xóa object (giống hành vi gốc)
            Destroy(gameObject);
        }
    }
}
