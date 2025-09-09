using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnPlayerDamaged;
    public static event Action<int> OnItemCollected;
    public static event Action<float> OnTimeChanged;

    public static void PlayerDamaged(int damage)
    {
        OnPlayerDamaged?.Invoke(damage);
    }

    public static void ItemCollected(int count)
    {
        OnItemCollected?.Invoke(count);
    }

    public static void TimeChanged(float remainingTime)
    {
        OnTimeChanged?.Invoke(remainingTime);
    }
}
