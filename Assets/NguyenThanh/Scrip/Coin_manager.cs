using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Coin_manager : MonoBehaviour
{
    public static Coin_manager Instance;

    public int totalCoins = 0;
    public TextMeshProUGUI coinText;

    private void Awake()
    {
        // Tạo Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Hàm cộng coin
    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        coinText.text = "coin : " + totalCoins.ToString();
    }
    public bool SpendCoin(int amount)
{
    if (totalCoins >= amount)
    {
        totalCoins -= amount;
        UpdateCoinUI();
        return true; // Mua thành công
    }
    else
    {
        Debug.Log("Không đủ tiền!");
        return false; // Mua thất bại
    }
}

}
