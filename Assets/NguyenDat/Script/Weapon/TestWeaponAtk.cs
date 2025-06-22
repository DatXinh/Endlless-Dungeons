using UnityEngine;

public class TestWeaponAtk : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cách 1: Lấy Animator đầu tiên trong tất cả các con của GameObject
        animator = gameObject.GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("TestWeaponAtk: animator is NULL!");
        else
            Debug.Log("TestWeaponAtk: animator is assigned.");

        // Cách 2: Nếu biết đường dẫn tương đối từ GameObject đến con cụ thể (ví dụ: "WeaponParent")
        //Transform weaponParentTransform = gameObject.transform.Find("WeaponParent");
        //if (weaponParentTransform != null)
        //{
        //    animator = weaponParentTransform.GetComponent<Animator>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        animator.SetTrigger("Atk");
    }
    public void AttackEnd()
    {
        animator.ResetTrigger("Atk");
    }
}
