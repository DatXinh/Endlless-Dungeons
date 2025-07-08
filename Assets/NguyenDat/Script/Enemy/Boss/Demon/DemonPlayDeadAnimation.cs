using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DemonPlayDeadAnimation : MonoBehaviour
{
    [Header("Health")]
    public EnemyHealth enemyHealth;
    [Header("Animator")]
    public Animator animator;
    [Header("Parameter Name")]
    public string parameterName;
    [Header("BossAI")]
    public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (enemyHealth != null && enemyHealth.currentHP == 0)
        {
            if (animator != null)
            {
                DisableScripts();
                animator.SetTrigger(parameterName);
            }
        }
    }
    public void DisableScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }
    }
    
}
