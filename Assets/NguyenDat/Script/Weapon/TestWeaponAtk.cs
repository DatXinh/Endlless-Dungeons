using UnityEngine;

public class TestWeaponAtk : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        if (animator == null)
        {
            return;
        }
        animator.SetTrigger("Atk");
    }
}
