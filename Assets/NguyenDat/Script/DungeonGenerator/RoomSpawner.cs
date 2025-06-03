using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public Room roomPrefab;
    public Vector2Int roomSizeMinMax = new Vector2Int(5, 8);
    public Vector2Int roomSpacing = new Vector2Int(2, 2);
    public int numberOfRoomsToSpawn = 9;

    private Dictionary<Vector2Int, Room> spawnedRooms = new Dictionary<Vector2Int, Room>();
    private Vector2 worldOriginOffset = Vector2.zero;

    void Start()
    {
        GenerateRooms(numberOfRoomsToSpawn);
    }

    void GenerateRooms(int count)
    {
        Vector2Int currentGridPos = Vector2Int.zero;
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(currentGridPos);

        Room startRoom = SpawnRoom(currentGridPos);
        worldOriginOffset = -startRoom.GetWorldCenter();
        startRoom.transform.position += (Vector3)worldOriginOffset;

        int roomsSpawned = 1;

        while (roomsSpawned < count)
        {
            Vector2Int randomDirection = GetRandomDirection();
            Vector2Int nextGridPos = currentGridPos + randomDirection;

            if (visited.Contains(nextGridPos)) continue; // Tránh trùng lặp

            Room newRoom = SpawnRoom(nextGridPos);
            if (newRoom != null)
            {
                newRoom.transform.position += (Vector3)worldOriginOffset;
                visited.Add(nextGridPos);
                currentGridPos = nextGridPos; // Chuyển vị trí hiện tại sang phòng mới
                roomsSpawned++;
            }
        }
    }

    Room SpawnRoom(Vector2Int gridPos)
    {
        if (spawnedRooms.ContainsKey(gridPos)) return null;

        int size = Random.Range(roomSizeMinMax.x, roomSizeMinMax.y + 1);
        Vector2Int roomSize = new Vector2Int(size, size);

        Vector2 worldPos = new Vector2(
            gridPos.x * (roomSize.x + roomSpacing.x),
            gridPos.y * (roomSize.y + roomSpacing.y)
        );

        Room newRoom = Instantiate(roomPrefab, worldPos, Quaternion.identity, transform);
        newRoom.Generate(roomSize.x, roomSize.y);

        RoomData data = newRoom.GetComponent<RoomData>();
        data.gridPosition = gridPos;
        data.roomSize = roomSize;

        spawnedRooms.Add(gridPos, newRoom);

        return newRoom;
    }
    Vector2Int GetRandomDirection()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        return directions[Random.Range(0, directions.Count)];
    }
}
