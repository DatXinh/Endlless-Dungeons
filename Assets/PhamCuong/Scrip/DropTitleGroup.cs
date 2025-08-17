using UnityEngine;

public class DropTitleGroup : MonoBehaviour
{
    public float delayStep = 1.0f; // mỗi chữ cách nhau 1 giây

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform letter = transform.GetChild(i);
            DropText drop = letter.GetComponent<DropText>();
            if (drop != null)
            {
                drop.delayBeforeDrop = i * delayStep;
            }
        }
    }
}
