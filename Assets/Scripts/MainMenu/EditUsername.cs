using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditUsername : MonoBehaviour
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button editButton;
    [SerializeField] private TMP_InputField inputText;

    private void Start()
    {
        cancelButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });

        editButton.onClick.AddListener(() =>
        {
            string newUsername = inputText.text.Trim();
            if (!string.IsNullOrEmpty(newUsername))
            {
                UpdateUsername(newUsername);
                this.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Username cannot be empty!");
            }
        });
    }

    private async void UpdateUsername(string newUsername)
    {
        if (NakamaManager.Instance != null)
        {
            bool success = await NakamaManager.Instance.UpdateUsername(newUsername);
            if (success)
            {
                Debug.Log($"Username successfully changed to: {newUsername}");
                NakamaManager.Instance.OnUpdateUsername(newUsername);
            }
            else
            {
                Debug.LogError("Failed to update username");
            }
        }
        else
        {
            Debug.LogError("NakamaManager instance not found");
        }
    }
}