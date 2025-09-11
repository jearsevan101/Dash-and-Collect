using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Game/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Combat Settings")]
    public int damage = 1;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float stoppingDistance = 0.5f;
    public LayerMask obstacleMask;

    [Header("Chase Settings")]
    public float detectionRange = 20f;      // how far enemy can see player
    public float senseRange = 4f;          // "feel" range (no LoS needed)
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
}
