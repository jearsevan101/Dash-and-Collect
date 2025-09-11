using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerManager : MonoBehaviour
{
    [SerializeField] private AudioLibrarySO library;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private MusicTrack currentTrack = (MusicTrack)(-1);

    private void Start()
    {
        PlayMusic(MusicTrack.MainTheme);
    }

    private void OnEnable()
    {
        GameEvents.onMusicTrackChanged += PlayMusic;
        GameEvents.onSFXChanged += PlaySFX;
    }

    private void OnDisable()
    {
        GameEvents.onMusicTrackChanged -= PlayMusic;
        GameEvents.onSFXChanged -= PlaySFX;
    }


    public void PlayMusic(MusicTrack track)
    {
        if (track == currentTrack)
        {
            return;
        }
        var data = library.GetMusic(track);
        if (data == null) return;

        musicSource.clip = data.clip;
        musicSource.volume = data.volume;
        musicSource.loop = data.loop;
        musicSource.Play();
        currentTrack = track;
    }

    public void PlaySFX(SFXType type)
    {
        var data = library.GetSFX(type);
        if (data == null) return;

        sfxSource.PlayOneShot(data.clip, data.volume);
    }



}
