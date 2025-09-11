using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI collectedItemText;
    [SerializeField] private Image dashBarCooldown;

    private void OnEnable()
    {
        GameEvents.OnItemCollected += HandleItemCollected;
        GameEvents.OnPlayerDamaged += HandleHealthUI;
        GameEvents.OnTimeChanged += HandleTimerUI;
        GameEvents.OnDashCooldownChanged += UpdateDashCooldownBar;
    }

    private void OnDisable()
    {
        GameEvents.OnItemCollected -= HandleItemCollected;
        GameEvents.OnPlayerDamaged -= HandleHealthUI;
        GameEvents.OnTimeChanged -= HandleTimerUI;
        GameEvents.OnDashCooldownChanged -= UpdateDashCooldownBar;
    }
    void Start()
    {
        collectedItemText.text = $"Collected: 0 / {GameManager.Instance.GetTotalCollectible()}";
    }

    private void HandleTimerUI(float remainingTime)
    {
        timerText.text = Mathf.RoundToInt(remainingTime).ToString();
    }
    private void HandleHealthUI(int damage)
    {
        healthText.text = $"Health: {GameManager.Instance.GetCurrentHealth()}/{GameManager.Instance.GetTotalHealth()}";
    }
    private void HandleItemCollected(GameEvents.CollectibleEventArgs e)
    {
        collectedItemText.text = $"Collected: {e.currentCollected} / {e.totalCollectibles}";
    }

    public void VisibilityUI(bool isVisible)
    {
        this.gameObject.SetActive(isVisible);
    }

    public void UpdateDashCooldownBar(float remainingTime, float max)
    {
        dashBarCooldown.fillAmount = 1 - (remainingTime / max);
    }
}
