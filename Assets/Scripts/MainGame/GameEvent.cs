using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnPlayerDamaged;
    public static event Action<float> OnTimeChanged;
    public static event Action OnGameOver;
    public static event Action OnGameFinished;
    public static event Action<bool> OnGamePaused;
    public static event Action<MusicTrack> onMusicTrackChanged;
    public static event Action<SFXType> onSFXChanged;
    public static event Action<float, float> OnDashCooldownChanged;
    public static event Action<float, float> OnCameraShaking;
    
    public class CollectibleEventArgs : EventArgs
    {
        public int totalCollectibles;
        public int currentCollected;

        public CollectibleEventArgs(int total, int current)
        {
            totalCollectibles = total;
            currentCollected = current;
        }
    }
    public static event Action<CollectibleEventArgs> OnItemCollected;

    public class GameStatusEventArgs : EventArgs
    {
        public int collectedItems;
        public float timeRemaining;
        public int health;
        public int overallScore;

        public GameStatusEventArgs(int collectedItems, float timeRemaining, int health, int overallScore)
        {
            this.collectedItems = collectedItems;
            this.timeRemaining = timeRemaining;
            this.health = health;
            this.overallScore = overallScore;
        }
    }
    public static event Action<GameStatusEventArgs> OnGameEnded;

    public static void CameraShaking(float duration, float magnitude)
    {
        OnCameraShaking?.Invoke(duration, magnitude);
    }

    public static void DashCooldownChanged(float remaining, float max)
    {
        OnDashCooldownChanged?.Invoke(remaining, max);
    }

    public static void PlayBGM(MusicTrack track)
    {
        onMusicTrackChanged?.Invoke(track);
    }

    public static void PlaySFX(SFXType type)
    {
        onSFXChanged?.Invoke(type);
    }

    public static void GameEnded(int collectedItems, float timeRemaining, int health, int overallScore)
    {
        OnGameEnded?.Invoke(new GameStatusEventArgs(collectedItems, timeRemaining, health, overallScore));
    }

    public static void PauseGame(bool isPaused)
    {
        OnGamePaused?.Invoke(isPaused);
    }
    public static void PlayerDamaged(int damage)
    {
        OnPlayerDamaged?.Invoke(damage);
    }

    public static void ItemCollected(int total, int current)
    {
        OnItemCollected?.Invoke(new CollectibleEventArgs(total, current));
    }

    public static void TimeChanged(float remainingTime)
    {
        OnTimeChanged?.Invoke(remainingTime);
    }
}
