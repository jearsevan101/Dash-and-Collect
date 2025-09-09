using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasManager : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private Button MyRecordButton;
    [SerializeField] private Button Leaderboard;

    private void Start()
    {
        StartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainGame);
        });

        MyRecordButton.onClick.AddListener(() =>
        {
            // TODO show record
        });

        Leaderboard.onClick.AddListener(() =>
        {
            // TODO show leaderboard
        });
    }
}
