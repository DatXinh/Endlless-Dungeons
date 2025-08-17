using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTelepot : MonoBehaviour
{
    public string targetScene; // Scene A

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
