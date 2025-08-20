using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;

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

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        // Nếu đã có user đăng nhập trước đó -> vào game luôn
        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
            Debug.Log($"🔑 Tự động đăng nhập: {currentUser.Email}");

            // Load dữ liệu user
            FirebaseUserDataManager.Instance.LoadUserData(currentUser.UserId);

            // 👉 Chuyển qua LoadScene để tới Home
            SceneLoadManager.nextSceneName = "Home";
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
        }
        else
        {
            ShowLoginUI();
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
            Debug.LogError("❌ Email hoặc mật khẩu trống!");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogError("❌ Mật khẩu nhập lại không khớp!");
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
            Debug.LogError("❌ Email hoặc mật khẩu trống!");
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
                    Debug.LogError("❌ Đăng ký thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                Debug.Log($"✅ Đăng ký thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");

                // Lưu user mới
                FirebaseUserDataManager.Instance.SaveNewUser(currentUser);

                // Đăng nhập luôn
                Login(email, password);
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
                    Debug.LogError("❌ Đăng nhập thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                Debug.Log($"✅ Đăng nhập thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");

                // Load dữ liệu user
                FirebaseUserDataManager.Instance.LoadUserData(currentUser.UserId);

                // 👉 Chuyển qua LoadScene để tới Home
                SceneLoadManager.nextSceneName = "Home";
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
            });
    }

    // Đăng xuất
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("🚪 Đã đăng xuất.");
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
}
