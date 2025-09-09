using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance; 
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [SerializeField] private float startTime = 300f; 
    private float timeRemaining;
    private bool timerRunning = true;

    public event Action<float> OnTimeChanged;
    public event Action OnTimerEnded;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timeRemaining = startTime;
    }

    void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        if (!timerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = Mathf.RoundToInt(timeRemaining).ToString();
            if (timeRemaining < 0) timeRemaining = 0;

            // Notify subscribers
            OnTimeChanged?.Invoke(timeRemaining);

            if (timeRemaining <= 0)
            {
                timerRunning = false;
                OnTimerEnded?.Invoke();
            }
        }
    }
}
