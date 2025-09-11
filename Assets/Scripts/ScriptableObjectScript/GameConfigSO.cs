using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private int totalCollectibles = 20;
    [SerializeField] private float TotalTime = 300f;


    public int GetTotalCollectibles()
    {
        return totalCollectibles;
    }
    public float GetTotalTime()
    {
        return TotalTime;
    }
}
