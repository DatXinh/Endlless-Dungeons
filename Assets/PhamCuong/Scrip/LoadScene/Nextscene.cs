using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Nextscene : MonoBehaviour
{
    public string nextScene; // Scene B
    public float delay = 5f; // Thời gian chờ

    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextScene);
    }
}
