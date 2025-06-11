using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckPlayerEnterRoom : MonoBehaviour
{
    public GameObject gateObject;    // Kéo GameObject "Gate" từ Hierarchy vào đây
    public TileBase newTile;         // Gán tile mới muốn thay thế

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Check"))
        {
            if (gateObject != null)
            {
                // 1. Thay đổi tile trong Tilemap
                Tilemap gateTilemap = gateObject.GetComponent<Tilemap>();
                if (gateTilemap != null)
                {
                    Vector3Int position = new Vector3Int(0, 0, 0); // vị trí tile cần đổi
                    gateTilemap.SetTile(position, newTile);
                }

                // 2. Tắt trigger của Collider
                Collider2D gateCollider = gateObject.GetComponent<BoxCollider2D>();
                if (gateCollider != null)
                {
                    gateCollider.isTrigger = false;
                }
            }
            else
            {
                Debug.LogWarning("Chưa gán GameObject Gate trong Inspector!");
            }
            Debug.Log("Player đã vào phòng và gate đã được xử lý.");
        }
    }
}
