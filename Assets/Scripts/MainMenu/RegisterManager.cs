using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private GameObject mainPanel;

    

    private void Start()
    {
        if (NakamaManager.Instance != null && NakamaManager.Instance.IsLoggedIn)
        {
            SetVisibility(false);
            mainPanel.SetActive(true); // show main panel instead
            return;
        }

        if (NakamaManager.Instance != null)
            NakamaManager.Instance.OnErrorMessage += HandleErrorMessage;

        registerButton.onClick.AddListener(() =>
        {
            string email = emailInput.text.Trim();
            string password = passwordInput.text.Trim();
            string username = usernameInput.text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                errorMessageText.text = "Email and password cannot be empty!";
                return;
            }

            Register(email, password, username);
        });

        loginButton.onClick.AddListener(() =>
        {
            SetVisibility(false);
            loginManager.SetVisibility(true);
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

    private async void Register(string email, string password, string username)
    {
        if (NakamaManager.Instance != null)
        {
            bool success = await NakamaManager.Instance.RegisterEmail(email, password, username);
            if (success)
            {
                Debug.Log($"Registered successfully as {username}");
                SetVisibility(false);
                mainPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Registration failed");
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
