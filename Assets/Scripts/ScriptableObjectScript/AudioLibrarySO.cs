using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Library")]
public class AudioLibrarySO : ScriptableObject
{
    public MusicData[] musicClips;
    public SFXData[] sfxClips;

    public MusicData GetMusic(MusicTrack track)
    {
        foreach (var m in musicClips)
            if (m.trackName == track) return m;
        return null;
    }

    public SFXData GetSFX(SFXType type)
    {
        foreach (var s in sfxClips)
            if (s.sfxName == type) return s;
        return null;
    }
}
