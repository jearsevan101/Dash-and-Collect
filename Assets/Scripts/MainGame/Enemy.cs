using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 5; 

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 8f;   // how far enemy can see player
    [SerializeField] private float chaseSpeed = 3.5f;     // faster movement when chasing
    [SerializeField] private Transform player;            // assign player transform in inspector
    [SerializeField] private float avoidDistance = 1f;    // how far to check for wall

    private Vector3 targetPos;
    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        PickNewDestination();
    }

    void Update()
    {
        if (player != null)
        {
            CheckForPlayer();
        }

        if (currentState == EnemyState.Patrol)
            Patrol();
        else if (currentState == EnemyState.Chase)
            Chase();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.PlayerDamaged(damage);
        }
    }

    // DETECTION
    void CheckForPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            Vector3 eye = transform.position + Vector3.up * 1.5f;
            Vector3 targetEye = player.position + Vector3.up * 1.5f;

            if (!Physics.Linecast(eye, targetEye, obstacleMask))
            {
                currentState = EnemyState.Chase;
                return;
            }
        }

        if (currentState == EnemyState.Chase)
        {
            currentState = EnemyState.Patrol;
            PickNewDestination();
        }
    }

    // PATROL
    void Patrol()
    {
        if (Vector3.Distance(transform.position, targetPos) <= stoppingDistance)
        {
            PickNewDestination();
        }
        else
        {
            MoveTowards(targetPos, moveSpeed);
        }
    }

    void PickNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y,
                transform.position.z + randomCircle.y
            );

            if (!Physics.Linecast(transform.position, candidate, obstacleMask))
            {
                targetPos = candidate;
                return;
            }
        }

        targetPos = transform.position;
    }

    // CHASE
    void Chase()
    {
        Vector3 eye = transform.position + Vector3.up * 1.5f;
        Vector3 targetEye = player.position + Vector3.up * 1.5f;

        if (Physics.Linecast(eye, targetEye, obstacleMask))
        {
            currentState = EnemyState.Patrol;
            PickNewDestination();
            return;
        }

        Vector3 dir = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, avoidDistance, obstacleMask))
        {
            Vector3 avoidDir = Vector3.Cross(Vector3.up, hit.normal).normalized;
            dir = Vector3.Lerp(dir, avoidDir, 0.7f);
        }

        MoveTowards(transform.position + dir, chaseSpeed);
    }

    // MOVEMENT
    void MoveTowards(Vector3 destination, float speed)
    {
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 5f);
    }
}
