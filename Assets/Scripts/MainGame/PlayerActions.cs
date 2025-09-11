using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;

    [SerializeField] private CollectibleSpawner collectiblesParent; 

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;

    private int currentCollectedItems = 0;


    void Update()
    {
        if (GameManager.Instance.IsGameStopped) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryCollectClosest();
        }
    }

    private void TryCollectClosest()
    {
        if (collectiblesParent == null) return;

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform child in collectiblesParent.transform)
        {
            float dist = Vector3.Distance(player.transform.position, child.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = child;
            }
        }

        if (closest != null && closestDistance <= interactRange)
        {
            Debug.Log("Collected: " + closest.name);
            Destroy(closest.gameObject);
            currentCollectedItems++;
            GameEvents.ItemCollected(GameManager.Instance.GetTotalCollectible(), currentCollectedItems);
        }
        else
        {
            Debug.Log("No collectible in range!");
        }
    }
}
