using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayDeadAnimation : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    [Header("Parameter Name")]
    public string parameterName;
    [Header("BossAI")]
    public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();
    private EnemySelfDestroy enemySelfDestroy;
    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (enemySelfDestroy == null)
        {
            enemySelfDestroy = GetComponent<EnemySelfDestroy>();
        }
    }
    public void DisableScripts()
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            enemySelfDestroy.SelfDestroy();
        }
        else
        {
            animator.SetTrigger(parameterName);
            foreach (var script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

    }

}
