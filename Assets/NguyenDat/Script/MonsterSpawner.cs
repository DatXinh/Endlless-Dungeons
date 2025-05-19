using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject normalSlimePrefab;
    public GameObject rareSlimePrefab;

    public float rareSlimeChance = 0.1f; // 10% tỉ lệ

    public int minMonsterCount;
    public int maxMonsterCount;
    public Tilemap groundTilemap;
    public Transform enemyParent;

    public void SpawnAllMonsters(List<RectInt> rooms, Tilemap tilemap)
    {
        int specialRoomIndex = Random.Range(1, rooms.Count); // Tránh phòng 0
        RectInt specialRoom = rooms[specialRoomIndex];

        // Sinh các quái thường trong các phòng còn lại
        for (int i = 1; i < rooms.Count; i++)
        {
            if (i == specialRoomIndex) continue; // bỏ qua phòng đặc biệt

            RectInt room = rooms[i];
            int monsterCount = Random.Range(minMonsterCount, maxMonsterCount);

            for (int j = 0; j < monsterCount; j++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(room.xMin + 1, room.xMax - 1),
                    Random.Range(room.yMin + 1, room.yMax - 1)
                );

                Vector3 world = tilemap.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0);
                GameObject monsterPrefab = Random.Range(0f, 1f) < rareSlimeChance ? rareSlimePrefab : normalSlimePrefab;
                Instantiate(monsterPrefab, world, Quaternion.identity, enemyParent);
            }
        }
    }

}
