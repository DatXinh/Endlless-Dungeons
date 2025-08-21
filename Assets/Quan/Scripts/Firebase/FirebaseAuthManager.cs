using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    public FirebaseUser currentUser;

    [Header("Register UI")]
    public TMP_InputField regEmailInput;
    public TMP_InputField regPasswordInput;
    public TMP_InputField regConfirmPasswordInput;
    public GameObject registerCanvas;
    public GameObject loginCanvas;

    [Header("Login UI")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    [Header("Message UI")]
    public GameObject messagePanel;
    public TMP_Text messageText;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        messagePanel.SetActive(false);

        // Nếu đã có user đăng nhập trước đó -> vào game luôn
        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
            ShowMessage($"Tự động đăng nhập: {currentUser.Email}", Color.green, 3f);

            FirebaseUserDataManager.Instance.LoadUserData(currentUser.UserId);

            SceneLoadManager.nextSceneName = "Home";
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
        }
        else
        {
            ShowLoginUI();
            ShowMessage("❌ Chưa có tài khoản!", Color.red, 3f);
        }
    }

    // Nút Đăng ký
    public void OnRegisterButton()
    {
        string email = regEmailInput.text.Trim();
        string password = regPasswordInput.text.Trim();
        string confirmPassword = regConfirmPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("❌ Email hoặc mật khẩu trống!", Color.red, 3f);
            return;
        }

        if (password != confirmPassword)
        {
            ShowMessage("❌ Mật khẩu nhập lại không khớp!", Color.red, 3f);
            return;
        }

        Register(email, password);
    }

    // Nút Đăng nhập
    public void OnLoginButton()
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("❌ Email hoặc mật khẩu trống!", Color.red, 3f);
            return;
        }

        Login(email, password);
    }

    // Đăng ký
    private void Register(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowMessage("❌ Đăng ký thất bại!", Color.red, 3f);
                    return;
                }

                currentUser = task.Result.User;
                ShowMessage("✅ Đăng ký thành công! Quay lại đăng nhập.", Color.green, 3f);

                FirebaseUserDataManager.Instance.SaveNewUser(currentUser);
            });
    }

    // Đăng nhập
    private void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowMessage("❌ Đăng nhập thất bại!", Color.red, 3f);
                    return;
                }

                currentUser = task.Result.User;
                ShowMessage("✅ Đăng nhập thành công!", Color.green, 2f);

                FirebaseUserDataManager.Instance.LoadUserData(currentUser.UserId);

                SceneLoadManager.nextSceneName = "Home";
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
            });
    }

    // Đăng xuất
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        ShowMessage("🚪 Đã đăng xuất.", Color.red, 3f);
        ShowLoginUI();
    }

    // Chuyển UI
    public void ShowRegisterUI()
    {
        loginCanvas.SetActive(false);
        registerCanvas.SetActive(true);
    }

    public void ShowLoginUI()
    {
        registerCanvas.SetActive(false);
        loginCanvas.SetActive(true);
    }

    // Hiển thị thông báo
    private void ShowMessage(string msg, Color color, float duration = 2f)
    {
        StopAllCoroutines();
        messagePanel.SetActive(true);
        messageText.text = msg;
        messageText.color = color;
        StartCoroutine(HideMessageAfter(duration));
    }

    private IEnumerator HideMessageAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messagePanel.SetActive(false);
    }
}
