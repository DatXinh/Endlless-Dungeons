using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    public List<Room> nearbyRooms = new List<Room>(); // Danh sách phòng lân cận
    public float maxDistance = 20f;
    public int nearRoom;

    public void Start()
    {
        FindNearbyRooms();
    }

    public void Generate(int width, int height)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                if (x == -1 || y == -1 || x == width || y == height)
                    wallTilemap.SetTile(tilePos, wallTile); // Tường ngoài
                else
                    floorTilemap.SetTile(tilePos, floorTile); // Sàn
            }
        }
    }

    public Vector2 GetWorldCenter()
    {
        // Lấy tâm của Tilemap Ground trong phòng này
        Tilemap ground = GetComponentInChildren<Tilemap>();
        if (ground != null)
        {
            Bounds bounds = ground.localBounds;
            return bounds.center;
        }
        return Vector2.zero;
    }

    void FindNearbyRooms()
    {
        nearbyRooms.Clear(); // Đảm bảo danh sách luôn được cập nhật
        GameObject[] roomObjects = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject roomObj in roomObjects)
        {
            Room room = roomObj.GetComponent<Room>();
            if (room == null || room == this) continue; // Bỏ qua nếu không phải phòng hợp lệ

            float distance = Vector2.Distance(transform.position, room.transform.position);

            if (distance <= maxDistance)
            {
                nearbyRooms.Add(room);
            }
        }
        nearRoom = nearbyRooms.Count; // Cập nhật số lượng phòng lân cận
    }

}
