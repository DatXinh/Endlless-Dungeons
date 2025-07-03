using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> normalEnemyPrefabs = new List<GameObject>();
    public List<GameObject> eliteEnemyPrefabs = new List<GameObject>();

    [Range(0f, 1f)]
    public float eliteEnemyChance = 0.1f; // 10% tỉ lệ sinh elite enemy

    public int minMonsterCount;
    public int maxMonsterCount;
    public Tilemap groundTilemap;
    public Transform enemyParent;

    public void SpawnAllMonsters(List<RectInt> rooms, Tilemap tilemap)
    {
        // Sinh quái ở tất cả các phòng trừ phòng đầu tiên (index 0)
        for (int i = 1; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            int monsterCount = Random.Range(minMonsterCount, maxMonsterCount);

            for (int j = 0; j < monsterCount; j++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(room.xMin + 1, room.xMax - 1),
                    Random.Range(room.yMin + 1, room.yMax - 1)
                );

                Vector3 world = tilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0);
                GameObject monsterPrefab = null;

                // 10% tỉ lệ sinh elite enemy, 90% sinh normal enemy
                if (Random.Range(0f, 1f) < eliteEnemyChance && eliteEnemyPrefabs.Count > 0)
                {
                    int idx = Random.Range(0, eliteEnemyPrefabs.Count);
                    monsterPrefab = eliteEnemyPrefabs[idx];
                }
                else if (normalEnemyPrefabs.Count > 0)
                {
                    int idx = Random.Range(0, normalEnemyPrefabs.Count);
                    monsterPrefab = normalEnemyPrefabs[idx];
                }

                if (monsterPrefab != null)
                {
                    Instantiate(monsterPrefab, world, Quaternion.identity, enemyParent);
                }
            }
        }
    }
}
