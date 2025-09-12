using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum NakamaConnectionStatus
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Failed
}

public class NakamaManager : MonoBehaviour
{
    public static NakamaManager Instance { get; private set; }
    public event Action<NakamaConnectionStatus> OnConnectionStatusChanged;
    public event Action<string> OnUsernameUpdated;
    public event Action<String> OnErrorMessage;

    private IClient client;
    private ISession session;
    private ISocket socket;


    [Header("Nakama Config")]
    [SerializeField] private string scheme = "http";
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int port = 7350;
    [SerializeField] private string serverKey = "defaultkey";

    [Header("Game Info")]
    [SerializeField] private string clientVersion = "1.0.0";
    [SerializeField] private string leaderboardId = "completion_leaderboard";

    public bool IsConnected => socket != null && socket.IsConnected;

    public bool IsLoggedIn => session != null && !session.IsExpired;

    public string CurrentUsername => session != null ? session.Username : string.Empty;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await ConnectWithDevice();
    }


    private void ChangeStatus(NakamaConnectionStatus status)
    {
        Debug.Log($"[Nakama] Status: {status}");
        OnConnectionStatusChanged?.Invoke(status);
    }

    public void OnUpdateUsername(string newUsername)
    {
        OnUsernameUpdated?.Invoke(newUsername);
    }
    public void ErrorMessage(string errorMessage)
    {
        OnErrorMessage?.Invoke(errorMessage);
    }

    // -------------------
    //  CONNECTION
    // -------------------
    #region Device Authentication

    public async Task ConnectWithDevice()
    {
        ChangeStatus(NakamaConnectionStatus.Connecting);
        client = new Client(scheme, host, port, serverKey);

        try
        {
            if (PlayerPrefs.HasKey("nakama.session"))
            {
                var token = PlayerPrefs.GetString("nakama.session");
                session = Session.Restore(token);

                if (!session.IsExpired)
                {
                    Debug.Log("Restored existing session.");
                    if (!string.IsNullOrEmpty(session.Username))
                        OnUpdateUsername(session.Username);
                }
                else
                {
                    Debug.Log("Session expired, re-authenticating...");
                    session = await AuthenticateDevice();
                }
            }
            else
            {
                session = await AuthenticateDevice();
            }

            PlayerPrefs.SetString("nakama.session", session.AuthToken);

            socket = client.NewSocket();
            socket.Closed += () => ChangeStatus(NakamaConnectionStatus.Disconnected);
            await socket.ConnectAsync(session);

            ChangeStatus(NakamaConnectionStatus.Connected);

            if (!string.IsNullOrEmpty(session.Username))
                OnUpdateUsername(session.Username);
        }
        catch (Exception e)
        {
            Debug.LogError($"Nakama connect failed: {e}");
            ChangeStatus(NakamaConnectionStatus.Failed);
        }
    }

    private async Task<ISession> AuthenticateDevice()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        var session = await client.AuthenticateDeviceAsync(deviceId, create: true);

        if (!string.IsNullOrEmpty(session.Username))
            OnUpdateUsername(session.Username);

        return session;
    }

    #endregion

    #region Email/Password Authentication

    public async Task<bool> RegisterEmail(string email, string password, string username = null)
    {
        try
        {
            // Registers the account (requires unique email)
            session = await client.AuthenticateEmailAsync(email, password, username, create: true);
            PlayerPrefs.SetString("nakama.session", session.AuthToken);

            OnUpdateUsername(session.Username);
            await ConnectSocket();
            Debug.Log($"Registered and connected as {session.Username}");
            return true;
        }
        catch (Exception e)
        {
            ErrorMessage("RegisterEmail failed: " + e.Message);
            Debug.LogError("RegisterEmail failed: " + e.Message);
            return false;
        }
    }

    public async Task<bool> LoginEmail(string email, string password)
    {
        try
        {
            // Login existing account
            session = await client.AuthenticateEmailAsync(email, password);
            PlayerPrefs.SetString("nakama.session", session.AuthToken);

            OnUpdateUsername(session.Username);
            await ConnectSocket();
            Debug.Log($"Logged in as {session.Username}");
            return true;
        }
        catch (Exception e)
        {
            ErrorMessage("LoginEmail failed: " + e.Message);
            Debug.LogError("LoginEmail failed: " + e.Message);
            return false;
        }
    }

    private async Task ConnectSocket()
    {
        if (client == null || session == null) return;

        socket = client.NewSocket();
        socket.Closed += () => ChangeStatus(NakamaConnectionStatus.Disconnected);
        await socket.ConnectAsync(session);
        ChangeStatus(NakamaConnectionStatus.Connected);
    }

    #endregion

    #region Username Update

    public async Task<bool> UpdateUsername(string newUsername)
    {
        if (session == null)
        {
            ErrorMessage("No session. Cannot update username.");
            Debug.LogWarning("No session. Cannot update username.");
            return false;
        }

        try
        {
            await client.UpdateAccountAsync(session, username: newUsername);
            Debug.Log($"Username updated to: {newUsername}");
            OnUpdateUsername(newUsername);
            return true;
        }
        catch (Exception e)
        {
            ErrorMessage("UpdateUsername failed: " + e.Message);
            Debug.LogError("UpdateUsername failed: " + e.Message);
            return false;
        }
    }

    #endregion

    public void Logout()
    {
        // Clear session
        session = null;

        // Disconnect socket if connected
        if (socket != null && socket.IsConnected)
        {
            socket.CloseAsync();
            socket = null;
        }

        // Remove saved session from PlayerPrefs
        if (PlayerPrefs.HasKey("nakama.session"))
            PlayerPrefs.DeleteKey("nakama.session");

        Debug.Log("Logged out successfully!");
    }


    // -------------------
    //  RPC SUBMIT RUN
    // -------------------
    public async Task<bool> SubmitRun(long completionTimeMs, int hitsTaken)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Not connected. Cannot submit run.");
            return false;
        }

        try
        {
            var metadata = new LeaderboardMetadata
            {
                hitsTaken = hitsTaken,
                version = clientVersion
            };
            string metadataJson = JsonUtility.ToJson(metadata);

            var record = await client.WriteLeaderboardRecordAsync(
                session, leaderboardId, completionTimeMs, 0, metadataJson
            );
            Debug.Log($"Submitted score: {record.Score}, rank: {record.Rank}, username: {record.Username}, metadata: {record.Metadata}");
            Debug.Log($"Score submitted: {record.Score} | Rank: {record.Rank}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("SubmitRun failed: " + e.Message);
            return false;
        }
    }

    [Serializable]
    private class SubmitRunResult
    {
        public bool success;
        public string error;
    }

    // -------------------
    //  LEADERBOARDS
    // -------------------

    public async Task<IApiLeaderboardRecordList> GetTopLeaderboard(int limit = 10)
    {
        try
        {
            var records = await client.ListLeaderboardRecordsAsync(session, leaderboardId, null, null, limit);

            Debug.Log($"Top {limit} records on {leaderboardId}: {records.Records.ToList().Count}");

            foreach (var record in records.Records)
            {
                Debug.Log($"Rank {record.Rank}, Score {record.Score}, Username {record.Username}, Metadata {record.Metadata}");
            }

            return records;
        }
        catch (Exception e)
        {
            Debug.LogError("GetTopLeaderboard failed: " + e.Message);
            return null;
        }
    }

    public async Task<IApiLeaderboardRecordList> GetAroundMeLeaderboard(int limit = 7)
    {
        try
        {
            var records = await client.ListLeaderboardRecordsAroundOwnerAsync(session, leaderboardId, session.UserId, limit);
            int count = records.Records.Count(); // FIX: use LINQ Count()

            Debug.Log($"Records around me: {count}");

            foreach (var record in records.Records)
            {
                Debug.Log($"{record.Rank}. {record.Username} - Time: {record.Score}ms");
            }

            return records;
        }
        catch (Exception e)
        {
            Debug.LogError("GetAroundMeLeaderboard failed: " + e.Message);
            return null;
        }
    }
    [Serializable]
    private class SerializableDict
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();

        public SerializableDict(Dictionary<string, object> dict)
        {
            foreach (var kv in dict)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value.ToString());
            }
        }
    }
}
