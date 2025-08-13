using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackgroundController : MonoBehaviour
{
    [Header("Background Sprites")]
    public Sprite tier1;
    public Sprite tier2;
    public Sprite tier3;

    [Header("UI Component")]
    public Image image;
    public GameObject Image;

    [Header("References")]
    public Transform Player;

    [Header("Scene Tiers")]
    public List<string> SceneTier1;
    public List<string> SceneTier2;
    public List<string> SceneTier3;
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (Player == null) return;

        // Di chuyển background theo player (chỉ trục X và Y nếu là 2D)
        Vector2 newPos = new Vector2(Player.position.x, Player.position.y);
        transform.position = newPos;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBackgroundImage();
    }
    private void UpdateBackgroundImage()
    {
        Image.SetActive(true);
        string currentScene = SceneManager.GetActiveScene().name;
        if (SceneTier1.Contains(currentScene))
        {
            image.sprite = tier1;
        }
        else if (SceneTier2.Contains(currentScene))
        {
            image.sprite = tier2;
        }
        else if (SceneTier3.Contains(currentScene))
        {
            image.sprite = tier3;
        }
        else
        {
            //Debug.LogWarning("Scene không nằm trong bất kỳ tier nào: " + currentScene);
            Image.SetActive (false);
        }
    }
}
