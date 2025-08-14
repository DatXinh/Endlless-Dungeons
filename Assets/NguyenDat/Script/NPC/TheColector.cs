using UnityEngine;

public class TheColector : MonoBehaviour , IInteractable
{
    public Canvas canvas;
    public Canvas Text;
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }
    private void Start()
    {
        canvas.enabled = false; // Ẩn canvas ban đầu
        Text.enabled = false; // Ẩn Text ban đầu
    }
    public void Interact()
    {
        canvas.enabled = true; // Hiển thị canvas khi tương tác
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hiển thị canvas khi người chơi vào vùng tương tác
            Text.enabled = true; // Hiển thị Text khi vào vùng tương tác
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Ẩn canvas khi người chơi rời khỏi vùng tương tác
            Text.enabled = false; // Ẩn Text khi rời khỏi vùng tương tác
        }
    }
}
