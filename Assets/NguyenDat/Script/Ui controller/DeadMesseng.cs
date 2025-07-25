using UnityEngine;

public class DeadMesseng : MonoBehaviour
{
    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;

    public PlayerHP playerHP;
    private void Awake()
    {
        playerHP = GetComponentInParent<PlayerHP>();
    }
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

    }
}
