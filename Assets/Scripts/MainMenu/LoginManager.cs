using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private RegisterManager registerManager;
    [SerializeField] private GameObject mainPanel;


    private void Start()
    {
        if (NakamaManager.Instance != null)
            NakamaManager.Instance.OnErrorMessage += HandleErrorMessage;

        loginButton.onClick.AddListener(() =>
        {
            string email = emailInput.text.Trim();
            string password = passwordInput.text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                errorMessageText.text = "Email and password cannot be empty!";
                Debug.LogWarning("Email and password cannot be empty!");
                return;
            }

            Login(email, password);
        });

        registerButton.onClick.AddListener(() =>
        {
            SetVisibility(false);
            registerManager.SetVisibility(true);
        });
    }
    private void OnDestroy()
    {
        if (NakamaManager.Instance != null)
            NakamaManager.Instance.OnErrorMessage -= HandleErrorMessage;
    }
    private void HandleErrorMessage(string message)
    {
        errorMessageText.text = message;
    }
    private async void Login(string email, string password)
    {
        if (NakamaManager.Instance != null)
        {
            bool success = await NakamaManager.Instance.LoginEmail(email, password);
            if (success)
            {
                SetVisibility(false);
                mainPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Login failed");
            }
        }
        else
        {
            Debug.LogError("NakamaManager instance not found");
        }
    }

    public void SetVisibility(bool visible)
    {
        this.gameObject.SetActive(visible);
    }
}
