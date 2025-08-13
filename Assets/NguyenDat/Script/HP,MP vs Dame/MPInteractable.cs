using UnityEngine;
using TMPro;

public class MPInteractable : MonoBehaviour, IInteractable
{
    public int ManaAmount = 20;
    public int CoinCost = 30;
    public GameObject Tooltip;
    public bool isSale = false; // Biến kiểm tra xem có phải là bán hay không
    public TextMeshProUGUI coinText; // Tham chiếu đến TextMeshProUGUI để hiển thị số lượng mana

    private void Awake()
    {
        Tooltip.SetActive(false);
        setCoinText();
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
