using UnityEngine;

public class MPInteractable : MonoBehaviour, IInteractable
{
    public int ManaAmount = 20;
    public GameObject Tooltip;

    private void Awake()
    {
        Tooltip.SetActive(false); // Ẩn tooltip ban đầu
    }
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Destroy(gameObject);
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
