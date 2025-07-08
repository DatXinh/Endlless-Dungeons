using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public void Selfdestroy()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
