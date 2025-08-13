using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerResetOnHome : MonoBehaviour
{
    [Header("Giá trị mặc định")]
    public int defaultHP = 200;
    public int defaultMP = 200;
    public GameObject defaultWeapon; // để None nếu không có vũ khí mặc định
    public Color defaultColor = Color.white;

    private PlayerHP playerHP;
    private PLayerMP playerMP;
    private Renderer playerRenderer;

    private void Awake()
    {
        playerHP = GetComponent<PlayerHP>();
        playerMP = GetComponent<PLayerMP>();
        playerRenderer = GetComponentInChildren<Renderer>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Home")
        {
            ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        if (playerHP != null)
        {
            playerHP.currentHP = defaultHP;
            playerHP.maxHP = defaultHP;
        }

        if (playerMP != null)
        {
            playerMP.currentMP = defaultMP;
            playerMP.maxMP = defaultMP;
        }

        // Reset màu
        if (playerRenderer != null)
        {
            playerRenderer.material.color = defaultColor;
        }

        Debug.Log("[PlayerResetOnHome] Player đã reset về mặc định ở Home.");
    }
}
