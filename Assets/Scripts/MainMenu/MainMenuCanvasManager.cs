using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

public class MainMenuCanvasManager : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button LeaderboardButton;
    [SerializeField] private Button SubmitScoreButton;
    [SerializeField] private Button editUsernameButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private LeaderboardUIManager leaderboardManager;
    [SerializeField] private EditUsername editUsername;

    public long completionTimeMs => Random.Range(10000, 40000); // between 10s and 40s
    public int hitsTaken => Random.Range(0, 5);

    private bool isEditUsernameActive = false;
    private bool isLeaderboardActive = false;


    private void OnEnable()
    {
        if (NakamaManager.Instance != null)
        {
            NakamaManager.Instance.OnConnectionStatusChanged += HandleConnectionStatus;
            NakamaManager.Instance.OnUsernameUpdated += HandleUsernameUpdated;
        }
    }

    private void OnDisable()
    {
        if (NakamaManager.Instance != null)
        {
            NakamaManager.Instance.OnConnectionStatusChanged -= HandleConnectionStatus;
            NakamaManager.Instance.OnUsernameUpdated -= HandleUsernameUpdated;
        }
    }

    private void Start()
    {
        if (NakamaManager.Instance != null && NakamaManager.Instance.IsLoggedIn)
        {
            usernameText.text = NakamaManager.Instance.CurrentUsername;
        }
        StartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainGame);
        });

        LeaderboardButton.onClick.AddListener(() => {
            isLeaderboardActive = !isLeaderboardActive;
            leaderboardManager.gameObject.SetActive(isLeaderboardActive);
            OnLeaderboardClicked();
        });

        SubmitScoreButton.onClick.AddListener(() =>
        {
            OnSubmitScore(completionTimeMs, hitsTaken);
        });
        editUsernameButton.onClick.AddListener(() =>
        {
            isEditUsernameActive = !isEditUsernameActive;
            editUsername.gameObject.SetActive(isEditUsernameActive);
        });
        logoutButton.onClick.AddListener(() =>
        {
            if (NakamaManager.Instance != null)
            {
                NakamaManager.Instance.Logout();

                Loader.Load(Loader.Scene.MainMenu); 
            }
        });
    }

    private void HandleUsernameUpdated(string username)
    {
        usernameText.text = username;
    }
    private void HandleConnectionStatus(NakamaConnectionStatus status)
    {
        switch (status)
        {
            case NakamaConnectionStatus.Connecting:
                statusText.text = "Connecting to server...";
                break;
            case NakamaConnectionStatus.Connected:
                statusText.text = "Connected!";
                break;
            case NakamaConnectionStatus.Reconnecting:
                statusText.text = "Reconnecting...";
                break;
            case NakamaConnectionStatus.Disconnected:
                statusText.text = "Disconnected.";
                break;
            case NakamaConnectionStatus.Failed:
                statusText.text = "Failed to connect!";
                break;
        }
    }

    private async void OnLeaderboardClicked()
    {
        LeaderboardButton.interactable = false;
        await Task.Delay(2000);
        var records = await NakamaManager.Instance.GetTopLeaderboard();

        
        if (records != null)
        {
            leaderboardManager.ShowLeaderboard(records);
            foreach (var record in records.Records)
            {
                Debug.Log($"{record.Rank}. {record.Username} - {record.Score}ms");
            }
        }
        else
        {
            Debug.LogError("Failed to get leaderboard");    
        }

        LeaderboardButton.interactable = true;
    }

    private async void OnSubmitScore(long completionTimeMs, int hitsTaken)
    {
        SubmitScoreButton.interactable = false;

        bool success = await NakamaManager.Instance.SubmitRun(completionTimeMs, hitsTaken);
        if (success)
            Debug.Log($"Score submitted successfully! completion Time: {completionTimeMs} hits taken: {hitsTaken}");
        else
            Debug.LogError("Failed to submit score");

        SubmitScoreButton.interactable = true;
    }

}
