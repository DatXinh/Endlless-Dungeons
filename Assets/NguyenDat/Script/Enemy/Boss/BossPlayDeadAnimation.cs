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
    public void DisableScripts()
    {
        animator.SetTrigger(parameterName);
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }
    }
    
}
