using UnityEngine;
using UnityEngine.Tilemaps;

public class GateTileChanger : MonoBehaviour
{
    public Tilemap tilemap;          // Kéo Tilemap vào đây trong Inspector
    public TileBase newTile;         // Kéo Tile muốn thay thế vào đây

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeGateTile()
    {
        Vector3Int tilePosition = new Vector3Int(0, 0, 0); // Vị trí ô cần thay
        tilemap.SetTile(tilePosition, newTile);            // Thay tile tại vị trí
    }
}
