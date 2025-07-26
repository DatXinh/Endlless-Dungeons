using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [Header("References")]
    public GameObject Gate;      // GameObject sẽ được bật khi Player chạm vào
    public GameObject[] monsterPrefabs;           // Danh sách prefab quái vật sẽ spawn
    public Transform spawnPosition;               // Vị trí spawn quái vật
    public GameObject Portal; // GameObject sẽ được bật khi quái vật chết

    private Collider2D myCollider;
    private GameObject spawnedMonster;
    private bool hasTriggered = false;
    public Transform BossParent;

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
            if (Gate != null)
                Gate.SetActive(true);

            // Tắt collider của chính nó
            if (myCollider != null)
                myCollider.enabled = false;

            // Spawn quái vật ngẫu nhiên
            if (monsterPrefabs != null && monsterPrefabs.Length > 0 && spawnPosition != null)
            {
                int randomIndex = Random.Range(0, monsterPrefabs.Length);
                GameObject selectedPrefab = monsterPrefabs[randomIndex];
                spawnedMonster = Instantiate(selectedPrefab, spawnPosition.position, spawnPosition.rotation);
                if (BossParent != null)
                {
                    spawnedMonster.transform.SetParent(BossParent);
                }
            }
        }
    }
    public void enablePortal()
    {
        if (Portal != null)
        {
            Portal.SetActive(true);
        }
    }
}
