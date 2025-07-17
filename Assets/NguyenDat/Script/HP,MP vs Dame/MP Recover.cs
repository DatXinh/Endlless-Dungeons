using UnityEngine;

public class MPRecover : MonoBehaviour
{
    public int manaRecoverAmount = 20;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PLayerMP playerMP = collision.GetComponent<PLayerMP>();
        if (playerMP != null)
        {
            playerMP.RecoverMP(manaRecoverAmount);
            Destroy(gameObject);
        }
    }
}
