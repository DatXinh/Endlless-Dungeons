using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Auth;

public class EndSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;          // Tiêu đề: WIN / LOSE
    public TMP_Text sceneText;
    public TMP_Text timeText;
    public TMP_Text coinText;
    public TMP_Text weapon1Text;
    public TMP_Text weapon2Text;

    public GameObject saveWinButton;    // Nút "Save"
    public GameObject homeButton;       // Nút "Về Home"

    [Header("Extra UI")]
    public TMP_Text winRewardText;      // 👉 Text thông báo khi WIN

    private CurrentRunData lastRun;
    private string computedStatus = "Unknown";

    void Start()
    {
        Time.timeScale = 1f;

        string json = PlayerPrefs.GetString("LastEndRun", "");
        Debug.Log("📥 EndScene Loaded LastEndRun JSON = " + json);

        if (!string.IsNullOrEmpty(json))
        {
            lastRun = JsonUtility.FromJson<CurrentRunData>(json);
        }

        if (lastRun != null)
        {
            // 👉 Tự tính trạng thái
            computedStatus = (lastRun.hp <= 0) ? "LOSE" : "WIN";

            if (titleText) titleText.text = computedStatus;
            if (sceneText) sceneText.text = $"Màn chơi: {lastRun.scene}";
            if (timeText) timeText.text = $"Thời gian: {FormatTime(lastRun.time)}";
            if (coinText) coinText.text = $"Coin: {lastRun.coin}";

            // Vũ khí
            if (weapon1Text)
                weapon1Text.text = $"Vũ khí 1: {(lastRun.weapons != null && lastRun.weapons.Length > 0 ? lastRun.weapons[0] : "None")}";
            if (weapon2Text)
                weapon2Text.text = $"Vũ khí 2: {(lastRun.weapons != null && lastRun.weapons.Length > 1 ? lastRun.weapons[1] : "None")}";

            // 👉 Nút Save & Text Reward chỉ hiện nếu Win
            if (saveWinButton) saveWinButton.SetActive(computedStatus == "WIN");
            if (winRewardText)
            {
                winRewardText.gameObject.SetActive(computedStatus == "WIN");
                if (computedStatus == "WIN")
                    winRewardText.text = "🎉 Bạn đã chiến thắng! Hãy lưu lại tiến trình.";
            }

            if (homeButton) homeButton.SetActive(true);
        }
        else
        {
            // 👉 Khi không có dữ liệu
            if (titleText) titleText.text = "No Data";
            if (sceneText) sceneText.text = "";
            if (timeText) timeText.text = "";
            if (coinText) coinText.text = "";
            if (weapon1Text) weapon1Text.text = "";
            if (weapon2Text) weapon2Text.text = "";

            if (saveWinButton) saveWinButton.SetActive(false);
            if (winRewardText) winRewardText.gameObject.SetActive(false);
            if (homeButton) homeButton.SetActive(true);
        }
    }

    private string FormatTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2}:{s:D2}";
    }

    // Nút Về Home
    public void OnHomeButton()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            // 👉 Nếu chết (LOSE) thì xoá log tạm
            if (computedStatus == "LOSE")
            {
                Debug.Log("🗑 Xóa lastEndLog vì trạng thái LOSE");
                FirebaseUserDataManager.Instance.ClearLastEndLog(user);
            }

            // 👉 Dù WIN hay LOSE cũng clear currentRun để lần sau là 1 run mới
            FirebaseUserDataManager.Instance.ClearCurrentRun(user);
        }

        // 👉 Xóa cache tạm ở PlayerPrefs
        PlayerPrefs.DeleteKey("LastEndRun");
        PlayerPrefs.Save();

        // Load về Home
        SceneManager.LoadScene("Home");
    }

    // Nút Save (chỉ khi Win)
    public void OnSaveWinButton()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null && lastRun != null)
        {
            Debug.Log("💾 Save EndGameLog với trạng thái WIN");
            FirebaseUserDataManager.Instance.SaveEndGameLog(user, lastRun, "Win");

            // 👉 Sau khi save thì clear currentRun luôn
            FirebaseUserDataManager.Instance.ClearCurrentRun(user);
        }

        PlayerPrefs.DeleteKey("LastEndRun");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Home");
    }
}
