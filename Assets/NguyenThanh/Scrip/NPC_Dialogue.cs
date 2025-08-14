using UnityEngine;
using TMPro;

public class NPC_Dialogue : MonoBehaviour
{
    public GameObject dialogueCanvas; // Canvas chứa text
    public TextMeshProUGUI dialogueText; // TMP component
    public string message = "Welcome, traveler!"; // Nội dung lời thoại
    public float displayTime = 3f;

    private float timer;

    void Start()
    {
        dialogueCanvas.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueText.text = message;
            dialogueCanvas.SetActive(true);
            timer = displayTime;
        }
    }

    void Update()
    {
        if (dialogueCanvas.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                dialogueCanvas.SetActive(false);
        }
    }
}
