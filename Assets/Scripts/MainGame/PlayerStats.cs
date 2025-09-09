using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Items")]
    [SerializeField] private int collectedItems = 0;

    [Header("Time (from GameManager)")]
    [SerializeField] private float timeRemaining;



    private void OnEnable()
    {
        GameEvents.OnPlayerDamaged += TakeDamage;
        GameEvents.OnItemCollected += AddItem;
        GameEvents.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDamaged -= TakeDamage;
        GameEvents.OnItemCollected -= AddItem;
        GameEvents.OnTimeChanged -= HandleTimeChanged;
    }


    private void AddItem(int count)
    {
        collectedItems += count;
        Debug.Log($"Collected item. Total items = {collectedItems}");
    }

    private void HandleTimeChanged(float remaining)
    {
        Debug.Log($"Time remaining: {remaining}");
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.OnTimeChanged += UpdateTime;
            GameUIManager.Instance.OnTimerEnded += HandleTimeOver;
        }
    }

    void OnDestroy()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.OnTimeChanged -= UpdateTime;
            GameUIManager.Instance.OnTimerEnded -= HandleTimeOver;
        }
    }

    // HEALTH
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }

    // TIME
    private void UpdateTime(float newTime)
    {
        timeRemaining = newTime;

    }

    private void HandleTimeOver()
    {
        //TODO 
    }
}
