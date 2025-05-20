using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomRomGeneratev1 : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public RuleTile groundTile;
    public RuleTile wallTile;

    public int minRoomCount = 5;
    public int maxRoomCount = 10;
    public Vector2Int roomSizeMinMax = new Vector2Int(5, 10);
    public int mapRange = 30;

    private Dictionary<Vector2Int, int> map = new();
    private List<RectInt> rooms = new();
    public MonsterSpawner monsterSpawner;

    void Start()
    {
        GenerateRooms();
        DrawMap();
    }

    void GenerateRooms()
    {
        map.Clear();
        rooms.Clear();

        RectInt startRoom = new RectInt(-5, -5, 10, 10);
        rooms.Add(startRoom);
        CarveRoom(startRoom);

        int roomCount = Random.Range(minRoomCount, maxRoomCount + 1);

        for (int i = 1; i < roomCount; i++)
        {
            RectInt newRoom;
            int tries = 0;

            do
            {
                tries++;
                int width = Random.Range(roomSizeMinMax.x, roomSizeMinMax.y + 1);
                int height = Random.Range(roomSizeMinMax.x, roomSizeMinMax.y + 1);
                Vector2Int pos = new Vector2Int(
                    Random.Range(-mapRange, mapRange),
                    Random.Range(-mapRange, mapRange)
                );
                newRoom = new RectInt(pos, new Vector2Int(width, height));
            }
            while (RoomOverlaps(newRoom) && tries < 10);

            if (tries >= 10) continue;

            rooms.Add(newRoom);
            CarveRoom(newRoom);
        }

        List<(float dist, int a, int b)> connections = new();

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                float distance = Vector2Int.Distance(Vector2Int.RoundToInt(rooms[i].center), Vector2Int.RoundToInt(rooms[j].center));
                connections.Add((distance, i, j));
            }
        }

        connections.Sort((x, y) => x.dist.CompareTo(y.dist));

        int[] parent = new int[rooms.Count];
        for (int i = 0; i < parent.Length; i++) parent[i] = i;

        int Find(int x)
        {
            if (parent[x] != x) parent[x] = Find(parent[x]);
            return parent[x];
        }

        void Union(int x, int y)
        {
            int px = Find(x);
            int py = Find(y);
            if (px != py) parent[px] = py;
        }

        foreach (var (dist, a, b) in connections)
        {
            if (Find(a) != Find(b))
            {
                Union(a, b);
                ConnectRooms(Vector2Int.RoundToInt(rooms[a].center), Vector2Int.RoundToInt(rooms[b].center));
            }
        }

        FillWalls();
        monsterSpawner.SpawnAllMonsters(rooms,groundTilemap);
    }

    void CarveRoom(RectInt room)
    {
        foreach (Vector2Int pos in room.allPositionsWithin)
        {
            map[pos] = 0;
        }
    }

    // Thêm khoảng đệm 1 tile xung quanh để tránh phòng quá sát nhau
    bool RoomOverlaps(RectInt newRoom)
    {
        RectInt paddedNewRoom = new RectInt(
            newRoom.xMin - 1, newRoom.yMin - 1,
            newRoom.width + 2, newRoom.height + 2
        );

        foreach (var room in rooms)
        {
            if (room.Overlaps(paddedNewRoom))
                return true;
        }
        return false;
    }

    void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        Vector2Int current = a;

        while (current.x != b.x)
        {
            current.x += (b.x > current.x) ? 1 : -1;
            CarveCorridorTile(current, Vector2Int.right); // đi ngang => mở rộng theo dọc
        }

        while (current.y != b.y)
        {
            current.y += (b.y > current.y) ? 1 : -1;
            CarveCorridorTile(current, Vector2Int.up); // đi dọc => mở rộng theo ngang
        }
    }

    // Hành lang rộng 3x3
    void CarveCorridorTile(Vector2Int pos, Vector2Int direction)
    {
        map[pos] = 0;

        // Nếu đi ngang thì mở rộng theo chiều dọc
        if (direction.x != 0)
        {
            map[pos + Vector2Int.up] = 0;
            map[pos + Vector2Int.down] = 0;
        }
        // Nếu đi dọc thì mở rộng theo chiều ngang
        else if (direction.y != 0)
        {
            map[pos + Vector2Int.left] = 0;
            map[pos + Vector2Int.right] = 0;
        }
    }


    void FillWalls()
    {
        HashSet<Vector2Int> toCheck = new(map.Keys);

        foreach (var pos in map.Keys)
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    toCheck.Add(pos + new Vector2Int(x, y));
        }

        foreach (var pos in toCheck)
        {
            if (!map.ContainsKey(pos))
            {
                map[pos] = 1;
            }
        }
    }

    void DrawMap()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var pair in map)
        {
            if (pair.Value == 0)
                groundTilemap.SetTile((Vector3Int)pair.Key, groundTile);
            else if (pair.Value == 1)
                wallTilemap.SetTile((Vector3Int)pair.Key, wallTile);
        }
    }
}
