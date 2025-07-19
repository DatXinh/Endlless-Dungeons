using UnityEngine;

public class MPInteractable : MonoBehaviour, IInteractable
{
    public int ManaAmount = 20;
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
}
