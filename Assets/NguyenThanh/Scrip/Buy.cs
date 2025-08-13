using UnityEngine;

public class Buy : MonoBehaviour
{
    public int price = 5;

    public void OnBuyButtonClick()
    {
        bool bought = Coin_manager.Instance.SpendCoin(price);

        if (bought)
        {
            Debug.Log("Mua thành công!");
            Destroy(gameObject); // Xoá chính nút này sau khi mua
        }
        else
        {
            Debug.Log("Không đủ tiền!");
        }
    }
}
