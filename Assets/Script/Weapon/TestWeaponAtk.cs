using UnityEngine;

public class TestWeaponAtk : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        animator.SetTrigger("Atk");
    }
}
