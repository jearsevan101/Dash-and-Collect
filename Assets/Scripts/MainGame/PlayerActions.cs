using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player; // Player reference

    [SerializeField] private Transform collectiblesParent; // Parent containing all collectibles

    [Header("Interaction Settings")]
    public float interactRange = 3f;


    void Update()
    {
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

        // Loop through children of collectiblesParent
        foreach (Transform child in collectiblesParent)
        {
            float dist = Vector3.Distance(player.transform.position, child.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = child;
            }
        }

        // If something is within range, collect it
        if (closest != null && closestDistance <= interactRange)
        {
            Debug.Log("Collected: " + closest.name);
            Destroy(closest.gameObject);

            GameEvents.ItemCollected(1);
        }
        else
        {
            Debug.Log("No collectible in range!");
        }
    }
}
