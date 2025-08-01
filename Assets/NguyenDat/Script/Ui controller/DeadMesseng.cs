using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMesseng : MonoBehaviour
{
    public int reviveTime = 2;
    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;
    public GameObject pauseButton;

    public PlayerHP playerHP;
    public PlayerInteractor playerInteractor;

    public  void Revive()
    {
        if(reviveTime > 0)
        {
            playerHP.ResumeGame();
            reviveTime--;
            rightPanel.SetActive(true);
            leftPanel.SetActive(true);
            pauseButton.SetActive(true);
            deadMesseng.SetActive(false);
            playerHP.currentHP = playerHP.maxHP;
            playerHP.UpdateHealthUI();
        }
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
