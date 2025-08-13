using UnityEngine;

public class WoodenBoxLoot : MonoBehaviour
{
    public GameObject lootPrefab1; // Prefab of the loot to drop
    public GameObject lootPrefab2; // Another prefab of the loot to drop
    public GameObject Toxictrap;

    public void spawLoot()
    {
        if(Random.Range(0, 2) == 0)
        {
            if (lootPrefab2 != null)
            {
                Instantiate(lootPrefab2, transform.position, Quaternion.identity);
                MPRecover mPRecover = lootPrefab2.GetComponent<MPRecover>();
                mPRecover.manaRecoverAmount = 5;
            }
        }
        else
        {
            if (lootPrefab1 != null)
            {
                Instantiate(lootPrefab1, transform.position, Quaternion.identity);
                HPRecover hPRecover = lootPrefab1.GetComponent<HPRecover>();
                if (hPRecover != null)
                {
                    hPRecover.healAmount = 5;
                }
            }
        }
    }
    public void spawToxictrap()
    {
        if (Toxictrap != null)
        {
            Instantiate(Toxictrap, transform.position, Quaternion.identity);
        }
    }

}
