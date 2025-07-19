using UnityEngine;

public class HPInteracable : MonoBehaviour, IInteractable
{
    public int healAmount = 20;
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
}
