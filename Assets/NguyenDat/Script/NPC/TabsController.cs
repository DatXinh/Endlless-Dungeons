using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] Pages;

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabIndex)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            Pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }

        Pages[tabIndex].SetActive(true);
        tabImages[tabIndex].color = Color.white;

        // ✅ Khắc phục lỗi không hiện layout
        var layouts = Pages[tabIndex].GetComponentsInChildren<RectTransform>(true);
        foreach (var layout in layouts)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }
    }
}
