using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneController : MonoBehaviour
{
    [Header("Planes (index = tier - 1)")]
    public List<GameObject> planes; // 0 = Tier1, 1 = Tier2, 2 = Tier3

    [Header("Scene tiers")]
    public List<string> tier1Scenes;
    public List<string> tier2Scenes;
    public List<string> tier3Scenes;

    [Tooltip("Target để theo dõi")]
    public Transform target;

    [Tooltip("Offset giữa object và target")]
    public Vector3 offset = Vector3.zero;

    [Tooltip("Theo dõi trong LateUpdate thay vì Update")]
    public bool followInLateUpdate = false;

    [Header("Smooth Follow Settings")]
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Tắt tất cả planes ban đầu
        SetActivePlane(-1);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!followInLateUpdate && target != null)
            FollowTarget();
    }

    private void LateUpdate()
    {
        if (followInLateUpdate && target != null)
            FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (tier1Scenes.Contains(sceneName))
            SetActivePlane(0);
        else if (tier2Scenes.Contains(sceneName))
            SetActivePlane(1);
        else if (tier3Scenes.Contains(sceneName))
            SetActivePlane(2);
        else
            SetActivePlane(-1);
    }

    private void SetActivePlane(int index)
    {
        for (int i = 0; i < planes.Count; i++)
        {
            planes[i].SetActive(i == index);
        }
    }
}
