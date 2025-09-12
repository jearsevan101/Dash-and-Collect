using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntryLeaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hitText;

    public void SetData(string rank, string name, string score, string hits)
    {
        rankText.text = rank;
        nameText.text = string.IsNullOrEmpty(name) ? "Anonymous" : name;
        scoreText.text = score;
        hitText.text = hits;
    }
}
