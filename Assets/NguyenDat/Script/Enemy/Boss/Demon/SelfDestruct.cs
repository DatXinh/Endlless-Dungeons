using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public GameObject[] loot;
    public GameObject LoreItem;
    public SpawnBoss SpawnBoss;
    public float dropRadius = 2f; // Bán kính xung quanh để rơi loot

    private void Start()
    {
        SpawnBoss = GetComponentInParent<SpawnBoss>();
    }

    public void Selfdestroy()
    {
        // Gọi hàm spawn loot trước khi hủy
        SpawnRandomLoot();

        if (transform.parent != null)
        {
            if (SpawnBoss != null)
                SpawnBoss.enablePortal();

            Destroy(transform.parent.gameObject);
        }
        else
        {
            if (SpawnBoss != null)
                SpawnBoss.enablePortal();

            Destroy(gameObject);
        }
    }

    private void SpawnRandomLoot()
    {
        if (loot == null || loot.Length == 0) return;

        // Chọn số lượng loot ngẫu nhiên từ 3 đến 5 (giới hạn không vượt quá số lượng loot có sẵn)
        int lootCount = Random.Range(1, Mathf.Min(3, loot.Length + 1));

        // Tạo danh sách chỉ số ngẫu nhiên không trùng
        List<int> indices = new System.Collections.Generic.List<int>();
        while (indices.Count < lootCount)
        {
            int rand = Random.Range(0, loot.Length);
            if (!indices.Contains(rand))
                indices.Add(rand);
        }

        // Sinh loot tại các vị trí ngẫu nhiên xung quanh
        foreach (int index in indices)
        {
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            Instantiate(loot[index], spawnPos, Quaternion.identity);
        }
        if (LoreItem != null)
        {
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            Instantiate(LoreItem, spawnPos, Quaternion.identity);
        }
    }
}
