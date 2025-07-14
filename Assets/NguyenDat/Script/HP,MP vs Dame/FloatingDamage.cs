using TMPro;
using UnityEngine;

public class FloatingDamage : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private TMP_Text DMValue;

    public float InitialYVelocity = 8f; // Initial velocity of the floating damage text
    public float InitialXVelocity = 3f; // Initial vertical velocity of the floating damage text
    public float Lifetime = 0.75f; // Lifetime of the floating damage text in seconds
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        DMValue = GetComponentInChildren<TMP_Text>();
    }
    void Start()
    {
        rb2d.linearVelocity = new Vector2(Random.Range(-InitialXVelocity,InitialXVelocity),InitialYVelocity);
        Destroy(gameObject, Lifetime); // Destroy the floating damage text after its lifetime
    }
    public void SetDamageValue(int value , Color color)
    {
        if (DMValue != null)
        {
            DMValue.text = value.ToString(); // Set the text to the damage value
            DMValue.color = color;
        }
    }
}
