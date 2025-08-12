using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenev2 : MonoBehaviour
{
    public void OnClickLoadScene()
    {
        // Gán tên scene muốn chuyển đến
        SceneManager.LoadScene("Home");

    }

}
