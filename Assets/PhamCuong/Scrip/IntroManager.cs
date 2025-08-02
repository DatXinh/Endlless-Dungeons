using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public float waitTime = 2f; // Chờ sau khi rơi xong
    private float timer = 0f;
    private bool allLettersDone = false;

    void Update()
    {
        if (!allLettersDone)
        {
            if (AllLettersStopped())
            {
                allLettersDone = true;
                timer = waitTime;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SceneManager.LoadScene("Start Game");
            }
        }
    }

    bool AllLettersStopped()
    {
        var letters = FindObjectsOfType<DropText>();
        foreach (var letter in letters)
        {
            if (letter.enabled) return false;
        }
        return true;
    }
}
