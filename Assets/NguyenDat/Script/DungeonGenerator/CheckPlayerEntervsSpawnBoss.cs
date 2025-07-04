using UnityEngine;

public class CheckPlayerEntervsSpawnBoss : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToEnableOnEnter;      // GameObject sẽ được bật khi Player chạm vào
    public GameObject[] monsterPrefabs;           // Danh sách prefab quái vật sẽ spawn
    public Transform spawnPosition;               // Vị trí spawn quái vật
    public GameObject objectToEnableOnMonsterDeath; // GameObject sẽ được bật khi quái vật chết

    private Collider2D myCollider;
    private GameObject spawnedMonster;
    private bool hasTriggered = false;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            // Enable đối tượng chỉ định
            if (objectToEnableOnEnter != null)
                objectToEnableOnEnter.SetActive(true);

            // Tắt collider của chính nó
            if (myCollider != null)
                myCollider.enabled = false;

            // Spawn quái vật ngẫu nhiên
            if (monsterPrefabs != null && monsterPrefabs.Length > 0 && spawnPosition != null)
            {
                int randomIndex = Random.Range(0, monsterPrefabs.Length);
                GameObject selectedPrefab = monsterPrefabs[randomIndex];
                spawnedMonster = Instantiate(selectedPrefab, spawnPosition.position, spawnPosition.rotation);
                // Đăng ký sự kiện chết của quái vật
                MonsterDeathHandler deathHandler = spawnedMonster.AddComponent<MonsterDeathHandler>();
                deathHandler.onDeathCallback = OnMonsterDeath;
            }
        }
    }

    // Hàm callback khi quái vật chết
    private void OnMonsterDeath()
    {
        if (objectToEnableOnMonsterDeath != null)
            objectToEnableOnMonsterDeath.SetActive(true);
    }
}

// Script phụ để bắt sự kiện chết của quái vật
public class MonsterDeathHandler : MonoBehaviour
{
    public System.Action onDeathCallback;

    // Gọi hàm này khi quái vật chết (ví dụ từ script máu của quái vật)
    public void Die()
    {
        if (onDeathCallback != null)
            onDeathCallback.Invoke();
        Destroy(this); // Xoá component này sau khi gọi callback
    }
}
