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
    public void Attack()
    {
        if (animator == null)
        {
            return;
        }
        //animator.SetTrigger("Atk");
        animator.SetBool("IsAtk",true);
    }
    public void AttackEnd()
    {
        if (animator == null)
        {
            return;
        }
        //animator.SetTrigger("AtkEnd");
        animator.SetBool("IsAtk", false);
    }
    public void resetWeapon()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }
}
