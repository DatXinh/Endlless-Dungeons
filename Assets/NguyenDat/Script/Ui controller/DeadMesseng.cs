using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMesseng : MonoBehaviour
{
    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;

    public PlayerHP playerHP;
    public PlayerInteractor playerInteractor;
    public  void Revive()
    {
        rightPanel.SetActive(true);
        leftPanel.SetActive(true);
        deadMesseng.SetActive(false);
        playerHP.currentHP = playerHP.maxHP;
        Time.timeScale = 1;
        playerHP.UpdateHealthUI();
        playerHP.ResumeGame();
    }
    public void Giveup()
    {
        playerInteractor.Coins = 50;
        playerInteractor.setCoinNumber();
        playerInteractor.RemoveAllWeapons();
        SceneLoadManager.nextSceneName = "Home";
        SceneManager.LoadScene("LoadScene");
    }
}
