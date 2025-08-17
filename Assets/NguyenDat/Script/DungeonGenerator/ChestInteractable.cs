using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    public Collider2D chestCollider;
    public Animator chestAnimator;
    public GameObject coinPrefab;
    public GameObject[] potionPrefabs;
    public GameObject[] weaponPrefabs;

    public Transform spawnPoint; // Vị trí sinh vật phẩm
    public AudioSource audioSource;

    private void Awake()
    {
        chestCollider = GetComponent<Collider2D>();
        chestAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public string GetInteractionPrompt()
    {
        return "Nhấn để mở rương";
    }

    public void Interact()
    {
        // 1. Tắt collider
        if (chestCollider != null)
            chestCollider.enabled = false;

        // 2. Chạy animation trigger "Oppen"
        if (chestAnimator != null)
            chestAnimator.SetTrigger("Oppen");

        // 3. Sinh vật phẩm theo tỉ lệ
        float rand = Random.value;
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        if (rand < 0.25f)
        {
            SpawnCoins(2, spawnPos);
        }
        else if (rand < 0.5f)
        {
            SpawnCoins(4, spawnPos);
        }
        else if (rand < 0.75f)
        {
            SpawnCoins(6, spawnPos);
        }
        else if (rand < 0.8f)
        {
            SpawnCoins(8, spawnPos);
        }
        else if (rand < 0.85f)
        {
            SpawnCoins(10, spawnPos);
        }
        else if (rand < 0.95f && potionPrefabs.Length > 0)
        {
            int index = Random.Range(0, potionPrefabs.Length);
            Instantiate(potionPrefabs[index], spawnPos, Quaternion.identity);
        }
        else if (weaponPrefabs.Length > 0)
        {
            int index = Random.Range(0, weaponPrefabs.Length);
            Instantiate(weaponPrefabs[index], spawnPos, Quaternion.identity);
        }
    }

    private void SpawnCoins(int count, Vector3 position)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Instantiate(coinPrefab, position + offset, Quaternion.identity);
        }
    }
    void playAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
