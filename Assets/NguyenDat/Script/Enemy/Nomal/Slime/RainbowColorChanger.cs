using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowColorChanger : MonoBehaviour
{
    public float speed = 1f;
    private SpriteRenderer sr;
    private float hue = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        hue += Time.deltaTime * speed;
        if (hue > 1f) hue -= 1f;

        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        sr.color = rainbowColor;
    }
}
