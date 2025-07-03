using UnityEngine;

public class SpawnAtDestroy : MonoBehaviour
{
    public GameObject spawnPrefab;

    private void OnDestroy()
    {
        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
        }
    }
}
