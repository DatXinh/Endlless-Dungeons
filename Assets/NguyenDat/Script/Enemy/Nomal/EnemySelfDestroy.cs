using UnityEngine;

public class EnemySelfDestroy : MonoBehaviour
{
    public Transform parent;
    public GameObject[] weaponLoot;
    public GameObject[] potionLoot;
    public float dropRadius = 2f;

    public void SelfDestroy()
    {
        SpawnRandomLoot();
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void SpawnRandomLoot()
    {
        float rand = Random.value;

        // 10%: sinh ra một món trong danh sách potionLoot
        if (rand < 0.10f && potionLoot != null && potionLoot.Length > 0)
        {
            int index = Random.Range(0, potionLoot.Length);
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            Instantiate(potionLoot[index], spawnPos, Quaternion.identity);
        }
        // 2.5%: sinh ra một món trong danh sách weaponLoot
        else if (rand < 0.125f && weaponLoot != null && weaponLoot.Length > 0)
        {
            int index = Random.Range(0, weaponLoot.Length);
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            Instantiate(weaponLoot[index], spawnPos, Quaternion.identity);
        }
        // Các trường hợp còn lại không sinh ra gì
    }
}
