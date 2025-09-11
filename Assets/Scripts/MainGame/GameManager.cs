using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;
    [SerializeField] private GameConfigSO gameConfig;
    [SerializeField] private GameUIManager gameUICanvas;
    [SerializeField] private GameEndUIManager gameEndUICanvas;


    private int chasingCount = 0;

    private bool gameEnded = false;

    public static GameManager Instance { get; private set; }
    public bool IsGameStopped { get; private set; }

    private float timeRemaining;
    private bool timerRunning = true;

    private void Start()
    {
        playerStats.ResetStats();
        timeRemaining = gameConfig.GetTotalTime();
    }

    private void Update()
    {
        if (IsGameStopped || gameEnded) return;

        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;

            GameEvents.TimeChanged(timeRemaining);

            if (timeRemaining <= 0)
            {
                timerRunning = false;
                CheckGameEnd();
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void EnemyStartedChase()
    {
        chasingCount++;
        if (chasingCount == 1)
            GameEvents.PlayBGM(MusicTrack.EnemyChasing);
    }

    public void EnemyStoppedChase()
    {
        chasingCount--;
        if (chasingCount <= 0) 
        {
            chasingCount = 0;
            GameEvents.PlayBGM(MusicTrack.MainTheme);
        }
    }

    public void StopGame()
    {
        GameEvents.PauseGame(true);
        IsGameStopped = true;
    }

    public void ResumeGame()
    {
        GameEvents.PauseGame(false);
        IsGameStopped = false;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDamaged += HandleDamage;
        GameEvents.OnItemCollected += HandleItemCollected;
        GameEvents.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDamaged -= HandleDamage;
        GameEvents.OnItemCollected -= HandleItemCollected;
        GameEvents.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleDamage(int dmg)
    {
        GameEvents.CameraShaking(0.5f, 0.2f);
        GameEvents.PlaySFX(SFXType.PlayerHit);
        playerStats.TakeDamage(dmg);
        CheckGameEnd();
    }

    private void HandleItemCollected(GameEvents.CollectibleEventArgs e)
    {
        GameEvents.PlaySFX(SFXType.ItemPickup);
        playerStats.collectedItems = e.currentCollected;
        CheckGameEnd();
    }

    private void HandleTimeChanged(float remaining)
    {
        timeRemaining = remaining;
        if (timeRemaining <= 0) CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        if (gameEnded) return;
        if (playerStats.collectedItems >= gameConfig.GetTotalCollectibles())
        {
            GameEvents.PlayBGM(MusicTrack.Victory);
            EndGame();
        } else if (playerStats.currentHealth <= 0 || timeRemaining <= 0)
        {
            GameEvents.PlayBGM(MusicTrack.GameOver);
            EndGame();
        }
    }
    private int CalculateOverallScore() 
    {
        float normalizedTime = timeRemaining / gameConfig.GetTotalTime();     
        float normalizedHealth = playerStats.currentHealth / playerStats.maxHealth;  
        float normalizedItems = playerStats.collectedItems / gameConfig.GetTotalCollectibles(); 

        float score =
            (normalizedTime * 0.3f +
             normalizedHealth * 0.3f +
             normalizedItems * 0.4f) * 1000f;

        return (int)score;
    }
    private void EndGame()
    {
        VisibilityCanvas(true);
        gameEnded = true;
        int overallScore = CalculateOverallScore();
        GameEvents.GameEnded(
            playerStats.collectedItems,
            timeRemaining,
            playerStats.currentHealth,
            overallScore

        );
        StopGame();
        Debug.Log("GAME ENDED!");
        Debug.Log($"GAME ENDED: COLLECTED ITEMS : {playerStats.collectedItems}, TIME REMAINING : {timeRemaining}, CURRENT HEALTH : {playerStats.currentHealth},");
    }

    private void VisibilityCanvas(bool isGameEnded)
    {
        if (isGameEnded)
        {
            gameUICanvas.VisibilityUI(false);
            gameEndUICanvas.VisibilityUI(true);
        }
        else
        {
            gameUICanvas.VisibilityUI(true);
            gameEndUICanvas.VisibilityUI(false);
        }
    }

    public int GetTotalCollectible()
    {
        return gameConfig.GetTotalCollectibles();
    }
    public float GetTimerDuration()
    {
        return gameConfig.GetTotalTime();
    }
    public int GetCurrentHealth()
    {
        return playerStats.currentHealth;
    }
    public int GetTotalHealth()
    {
        return playerStats.maxHealth;
    }
}
