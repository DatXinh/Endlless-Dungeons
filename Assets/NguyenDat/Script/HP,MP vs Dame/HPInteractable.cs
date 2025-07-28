using TMPro;
using UnityEngine;

public class HPInteracable : MonoBehaviour, IInteractable
{
    public int healAmount = 20;
    public int CoinCost = 30;
    public GameObject Tooltip;
    public bool isSale = false; // Biến kiểm tra xem có phải là bán hay không
    public TextMeshProUGUI coinText;
    private void Awake()
    {
        Tooltip.SetActive(false); // Ẩn tooltip ban đầu
        setCoinText(); // Gọi hàm để thiết lập văn bản hiển thị số lượng coin
    }
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
    public void setCoinText()
    {
        if (isSale)
        {
            coinText.text = CoinCost.ToString();
        }
        else
        {
            coinText.text = "0";
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hiển thị tooltip khi người chơi vào vùng tương tác
            Tooltip.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Ẩn tooltip khi người chơi rời khỏi vùng tương tác
            Tooltip.SetActive(false);
        }
    }
}
