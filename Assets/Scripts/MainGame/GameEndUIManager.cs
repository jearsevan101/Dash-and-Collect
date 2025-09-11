using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameEvents;

public class GameEndUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI collectedItemText;
    [SerializeField] private TextMeshProUGUI overallScoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button continueToMainMenu;

    private void OnEnable()
    {
        GameEvents.OnGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        GameEvents.OnGameEnded -= HandleGameEnded;
    }
    private void Start()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainGame);
        });
        continueToMainMenu.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        });
    }

    private void HandleGameEnded(GameStatusEventArgs e)
    {
        SetPopUpValue((int)e.timeRemaining, e.health, GameManager.Instance.GetTotalHealth(), e.collectedItems, GameManager.Instance.GetTotalCollectible(), e.overallScore);
    }

    public void SetPopUpValue(int currentTime, int currentHealth, int maxHealth, int collectedItem, int totalItem, int overallScore)
    {
        timerText.text = $"Time Remaining: {currentTime} Second";
        healthText.text = $"Health: {currentHealth}/{maxHealth}";
        collectedItemText.text = $"Collected: {collectedItem}/{totalItem}";
        overallScoreText.text = $"Overall Score \n {overallScore}";
    }
    public void VisibilityUI(bool isVisible)
    {
        this.gameObject.SetActive(isVisible);
    }
}
