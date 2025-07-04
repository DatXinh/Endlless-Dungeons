using UnityEngine;

public class SpawnAtDestroy : MonoBehaviour
{
    public GameObject spawnPrefab;
    private bool shouldSpawn = false;

    public void TriggerDestroy()
    {
        shouldSpawn = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;

        if (shouldSpawn && spawnPrefab != null)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
        }
    }
}
