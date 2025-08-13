using UnityEngine;

public class ShopInteractable : MonoBehaviour , IInteractable
{
    public GameObject[] weaponPrefabs; // Mảng chứa các prefab vũ khí
    public GameObject[] saleSlot;
    public GameObject[] Potion;

    public Canvas shopCanvas;

    public int interactionCount = 0; // Số lần tương tác với shop

    private void Awake()
    {
        RandomizeSaleSlots(interactionCount);
        shopCanvas.enabled = false; // Ẩn canvas cửa hàng ban đầu
    }

    public string GetInteractionPrompt()
    {
        return "Nhấn để xem cửa hàng";
    }

    public void Interact()
    {
        interactionCount++;

        // RandomizeSaleSlots sẽ tăng giá cho các weapon được sinh ra từ lần thứ 2 trở đi
        RandomizeSaleSlots(interactionCount);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Nếu người chơi vào vùng tương tác, hiển thị canvas cửa hàng
            shopCanvas.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        shopCanvas.enabled = false; // Ẩn canvas cửa hàng khi người chơi rời khỏi vùng tương tác
    }
    private void RandomizeSaleSlots(int interactionCount)
    {
        for (int i = 0; i < saleSlot.Length; i++)
        {
            // Xóa vật phẩm cũ nếu có
            foreach (Transform child in saleSlot[i].transform)
            {
                Destroy(child.gameObject);
            }

            GameObject prefabToSpawn = null;
            bool isWeapon = false;

            // Nếu cả 2 mảng đều rỗng → bỏ qua slot (bắt buộc)
            if (weaponPrefabs.Length == 0 && Potion.Length == 0)
                continue;

            // Ưu tiên 80% là weapon (nếu có)
            float rand = Random.value;
            if (rand < 0.8f && weaponPrefabs.Length > 0)
            {
                int weaponIndex = Random.Range(0, weaponPrefabs.Length);
                prefabToSpawn = weaponPrefabs[weaponIndex];
                isWeapon = true;
            }
            else if (Potion.Length > 0)
            {
                int potionIndex = Random.Range(0, Potion.Length);
                prefabToSpawn = Potion[potionIndex];
                isWeapon = false;
            }
            else if (weaponPrefabs.Length > 0)
            {
                // Nếu Potion rỗng nhưng Weapon vẫn còn → fallback
                int weaponIndex = Random.Range(0, weaponPrefabs.Length);
                prefabToSpawn = weaponPrefabs[weaponIndex];
                isWeapon = true;
            }

            // Kiểm tra lần cuối
            if (prefabToSpawn == null)
                continue;

            // Sinh ra vật phẩm
            GameObject spawned = Instantiate(prefabToSpawn, saleSlot[i].transform.position, Quaternion.identity, saleSlot[i].transform);

            if (isWeapon)
            {
                WeaponInteractable weaponInteractable = spawned.GetComponent<WeaponInteractable>();
                WeaponData weaponData = spawned.GetComponent<WeaponData>();
                if (weaponInteractable != null)
                {
                    weaponInteractable.isSale = true;

                    if (weaponData != null)
                    {
                        int basePrice = weaponData.WeaponPrice;
                        int newPrice = interactionCount > 1
                            ? Mathf.CeilToInt(basePrice * Mathf.Pow(1.1f, interactionCount))
                            : basePrice;

                        weaponData.WeaponPrice = newPrice;
                    }
                }
            }
            else
            {
                MPInteractable mpInteractable = spawned.GetComponent<MPInteractable>();
                HPInteracable hpInteractable = spawned.GetComponent<HPInteracable>();

                if (mpInteractable != null)
                {
                    mpInteractable.isSale = true;
                    mpInteractable.setCoinText();
                }
                else if (hpInteractable != null)
                {
                    hpInteractable.isSale = true;
                    hpInteractable.setCoinText();
                }
            }
        }
    }

}
