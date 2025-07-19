using UnityEngine;

public class PortalInteractble : MonoBehaviour, IInteractable
{
    public ToSecens toSecens;
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        toSecens = GetComponent<ToSecens>();
        if (toSecens != null)
        {
            toSecens.toScene();
        }
        else
        {
            Debug.LogWarning("ToSecens component không được tìm thấy trên đối tượng này.");
        }
    }
}
