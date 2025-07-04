using UnityEngine;

public class ChuyenMan : MonoBehaviour
{
    private GameObject PortalPrefab;
    private DungeonGenerate DungeonGenerate;
    private ToSecens ToSecens;
    public string SceneName;


    private void Awake()
    {
        DungeonGenerate = GetComponent<DungeonGenerate>();
        PortalPrefab = DungeonGenerate.portalPrefab;
        ToSecens = PortalPrefab.GetComponent<ToSecens>();
    }
    void Start()
    {
        GanSecens();
    }
    public void GanSecens()
    {
        ToSecens.sceneName = SceneName;
    }
}
