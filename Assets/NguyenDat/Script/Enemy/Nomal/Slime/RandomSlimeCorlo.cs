using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSlimeCorlo : MonoBehaviour
{

    public Color[] possibleColors = new Color[]
{
    new Color(0.3f, 1.0f, 0.3f),   // Xanh lá tươi (Green Slime)
    new Color(0.2f, 0.6f, 1.0f),   // Xanh dương (Water Slime)
    new Color(1.0f, 0.4f, 0.4f),   // Đỏ (Fire Slime)
    new Color(1.0f, 1.0f, 0.3f),   // Vàng (Electric Slime)
    new Color(0.8f, 0.5f, 1.0f),   // Tím (Magic Slime)
    new Color(0.6f, 0.6f, 0.6f),   // Xám (Stone Slime)
    new Color(0f, 0f, 0f),         // Đen (Dark Slime)
    new Color(1f, 1f, 1f)          // Trắng (Holy Slime)
};
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = possibleColors[Random.Range(0, possibleColors.Length)];
    }

}
