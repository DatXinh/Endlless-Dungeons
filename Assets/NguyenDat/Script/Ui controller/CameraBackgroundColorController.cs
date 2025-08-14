using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBackgroundColorController : MonoBehaviour
{
    public Camera mainCamera;

    [Header("Scene Background Colors")]
    public List<string> Tier1Scenes;
    public Color tier1;

    public List<string> Tier2Scenes;
    public Color tier2;

    public List<string> Tier3Scenes;
    public Color tier3;

    public Color defaultColor = Color.black;

    private void Start()
    {
        SceneManager.sceneLoaded += UpdateCameraBackgroundColor;
    }

    private void UpdateCameraBackgroundColor(Scene arg0, LoadSceneMode arg1)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (Tier1Scenes.Contains(currentScene))
        {
            mainCamera.backgroundColor = tier1;
        }
        else if (Tier2Scenes.Contains(currentScene))
        {
            mainCamera.backgroundColor = tier2;
        }
        else if (Tier3Scenes.Contains(currentScene))
        {
            mainCamera.backgroundColor = tier3;
        }
        else
        {
            mainCamera.backgroundColor = defaultColor;
        }
    }
}
