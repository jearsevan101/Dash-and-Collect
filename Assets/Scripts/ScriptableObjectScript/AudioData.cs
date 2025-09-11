using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicTrack
{
    None=-1,
    MainTheme,
    EnemyChasing,
    Victory,
    GameOver
}

public enum SFXType
{
    ItemPickup,
    PlayerHit,
    EnemyNearby,
    PlayerDash
}

[System.Serializable]
public class MusicData
{
    public MusicTrack trackName;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = true;
}

[System.Serializable]
public class SFXData
{
    public SFXType sfxName;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}
