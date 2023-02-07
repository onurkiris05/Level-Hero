using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject destinationPrefab;
    [SerializeField] private float enemySpeed;

    [Header("Detection Settings")]
    [SerializeField] private float detectionDistance;

    public bool CanAttack;

    private Enemy enemy;
    private EnemyManager enemyManager;
    private GameObject destinationSwitcher;
    private Coroutine chasePlayer;
    private NavMeshAgent navMeshAgent;
    private SphereCollider sphereCollider;
    private bool isChasing;
    private Player player;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyManager = FindObjectOfType<EnemyManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.radius += 0.002f * enemy.CurrentLevel;

        if (CanAttack)
        {
            StartCoroutine(ProcessChasePlayer(player));
        }
        else
        {
            ProcessWalkAround();
        }
    }

    public void SetUp(float enemySpeed, float colliderSize, float radiusSize)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();

        detectionDistance += colliderSize;
        sphereCollider.radius = detectionDistance;
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.radius += radiusSize;
    }

    public void ProcessWalkAround()
    {
        if (chasePlayer != null)
        {
            StopCoroutine(chasePlayer);
        }

        Vector3 randomPos = enemyManager.GetRandomPos();

        if (destinationSwitcher != null)
        {
            Destroy(destinationSwitcher);
        }

        destinationSwitcher = Instantiate(destinationPrefab, randomPos, Quaternion.identity);

        navMeshAgent.velocity = Vector3.zero;

        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(randomPos);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Player player = other.GetComponentInParent<Player>();
    //
    //         if (!player.isBeingFollowed)
    //         {
    //             player.isBeingFollowed = true;
    //             chasePlayer = StartCoroutine(ProcessChasePlayer(player));
    //         }
    //     }
    //     else if (other.gameObject == destinationSwitcher)
    //     {
    //         ProcessWalkAround();
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Player player = other.GetComponentInParent<Player>();
    //
    //         ProcessWalkAround();
    //
    //         if (player.isBeingFollowed)
    //         {
    //             player.isBeingFollowed = false;
    //         }
    //     }
    // }

    public void ChasePlayer(Player player)
    {
        CanAttack = true;
        chasePlayer = StartCoroutine(ProcessChasePlayer(player));
    }

    public void StopChasing()
    {
        if (chasePlayer != null)
        {
            StopCoroutine(chasePlayer);
        }

        CanAttack = false;
    }

    IEnumerator ProcessChasePlayer(Player player)
    {
        if (player != null)
        {
            isChasing = true;

            while (isChasing)
            {
                if (enemy.CurrentLevel > player.CurrentLevel && player.isActive)
                {
                    if (destinationSwitcher != null)
                    {
                        Destroy(destinationSwitcher);
                    }

                    navMeshAgent.SetDestination(player.transform.position);
                }
                else
                {
                    isChasing = false;
                    ProcessWalkAround();
                    yield break;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}