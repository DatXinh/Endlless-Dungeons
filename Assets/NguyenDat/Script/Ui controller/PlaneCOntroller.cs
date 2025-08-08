using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneCOntroller : MonoBehaviour
{
    [Header("Planes")]
    public GameObject Plane1;
    public GameObject Plane2;
    public GameObject Plane3;

    [Header("Scene tier")]
    public List<string> Tier1Scenes;

    public List<string> Tier2Scenes;

    public List<string> Tier3Scenes;

    [Tooltip("Object mà đối tượng này sẽ theo dõi")]
    public Transform target;

    [Tooltip("Khoảng cách offset giữa đối tượng và target")]
    public Vector3 offset = Vector3.zero;

    [Tooltip("Có nên cập nhật vị trí mỗi khung hình không?")]
    public bool followInLateUpdate = false;

    private void Start()
    {
        // Ensure the planes are set to inactive at the start
        Plane1.SetActive(false);
        Plane2.SetActive(false);
        Plane3.SetActive(false);
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += UpdatePlaneVisibility;
    }
    void Update()
    {
        if (!followInLateUpdate)
            Follow();
    }

    void LateUpdate()
    {
        if (followInLateUpdate)
            Follow();
    }

    void Follow()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    private void UpdatePlaneVisibility(Scene arg0, LoadSceneMode arg1)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (Tier1Scenes.Contains(currentScene))
        {
            Plane1.SetActive(true);
            Plane2.SetActive(false);
            Plane3.SetActive(false);
        }
        else if (Tier2Scenes.Contains(currentScene))
        {
            Plane1.SetActive(false);
            Plane2.SetActive(true);
            Plane3.SetActive(false);
        }
        else if (Tier3Scenes.Contains(currentScene))
        {
            Plane1.SetActive(false);
            Plane2.SetActive(false);
            Plane3.SetActive(true);
        }
        else
        {
            Plane1.SetActive(false);
            Plane2.SetActive(false);
            Plane3.SetActive(false);
        }
    }
}
