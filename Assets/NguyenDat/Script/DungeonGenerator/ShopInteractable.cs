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

            // Chọn ngẫu nhiên: 80% weapon, 20% potion
            float rand = Random.value;
            GameObject prefabToSpawn;
            bool isWeapon = false;
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
            }
            else
            {
                continue; // Không có prefab để sinh
            }

            // Sinh ra vật phẩm tại vị trí saleSlot
            GameObject spawned = Instantiate(prefabToSpawn, saleSlot[i].transform.position, Quaternion.identity, saleSlot[i].transform);

            // Nếu là weapon thì set biến isSale = true và tăng giá nếu cần
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
                        int newPrice = basePrice;
                        if (interactionCount > 1)
                        {
                            newPrice = Mathf.CeilToInt(basePrice * Mathf.Pow(1.1f, interactionCount));
                        }
                        weaponData.WeaponPrice = newPrice;
                    }
                }
            }
        }
    }
}
