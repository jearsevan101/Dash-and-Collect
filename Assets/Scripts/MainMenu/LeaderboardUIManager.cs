using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform contentParent; 
    [SerializeField] private GameObject entryPrefab;

    private List<GameObject> entries = new List<GameObject>();

    public void ClearEntries()
    {
        foreach (var entry in entries)
            Destroy(entry);
        entries.Clear();
    }

    public void ShowLeaderboard(Nakama.IApiLeaderboardRecordList records)
    {
        ClearEntries();

        foreach (var record in records.Records)
        {
            LeaderboardMetadata metadata = null;

            if (!string.IsNullOrEmpty(record.Metadata))
            {
                try
                {
                    metadata = JsonUtility.FromJson<LeaderboardMetadata>(record.Metadata);
                }
                catch
                {
                    Debug.LogWarning($"Failed to parse metadata for {record.Username}: {record.Metadata}");
                }
            }

            var entryObj = Instantiate(entryPrefab, contentParent);
            var entry = entryObj.GetComponent<EntryLeaderboard>();
            entry.SetData(
                record.Rank.ToString(),
                record.Username,
                record.Score.ToString(),
                metadata?.hitsTaken.ToString() ?? "0"
            );
            entries.Add(entryObj);
        }
    }

}
[System.Serializable]
public class LeaderboardMetadata
{
    public int hitsTaken;
    public string version;
}
