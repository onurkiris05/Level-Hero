using System;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("General Components")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform minBorder, maxBorder;

    [Header("Enemy Settings")]
    [SerializeField] private float baseEnemySpeed;
    [SerializeField] private int enemyCount;
    [SerializeField] private int highLevelEnemyCount;
    [SerializeField] private int enemyMaxLevel;

    [Header("Enemy Size Settings")]
    [SerializeField] private float growSizePerLevel;
    [SerializeField] private float growRadiusPerLevel;
    [SerializeField] private float growTrailPerLevel;
    [SerializeField] private float growColliderPerLevel;
    [SerializeField] private float reduceSpeedPerLevel;
    [SerializeField] private float reduceRotationPerLevel;

    [Header("Enemy UI Settings")]
    [SerializeField] private float growHeaderTextPerLevel;
    [SerializeField] private float headerTextUpPerLevel;

    public static List<Enemy> spawnedEnemies = new List<Enemy>();

    void Awake()
    {
        // baseEnemySpeed = RemoteConfigManager.EnemySpeed;
        // enemyCount = RemoteConfigManager.EnemyCount;
        // highLevelEnemyCount = RemoteConfigManager.HighLevelEnemyCount;
        // enemyMaxLevel = RemoteConfigManager.EnemyMaxLevels[(PlayerPrefKeys.CurrentLevel - 1) % 4];

        //CreateEnemies();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartAllEnemies();
        }
    }

    private void OnDisable()
    {
        spawnedEnemies.Clear();
    }

    public Vector3 GetRandomPos()
    {
        float randomX = Random.Range(minBorder.position.x, maxBorder.position.x);
        float randomZ = Random.Range(minBorder.position.z, maxBorder.position.z);

        Vector3 targetPos = new Vector3(randomX, 0f, randomZ);

        return targetPos;
    }

    public void SetEnemyTextsAndMaterials(int playerLevel)
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            EnemyUI enemyUI = enemy.GetComponentInChildren<EnemyUI>();

            if (enemyUI != null)
                enemyUI.SetHeaderText(playerLevel, enemy.CurrentLevel);

            enemy.SetMaterial(playerLevel);
        }
    }

    public void EliminateAllEnemies()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.Die();
        }
    }

    public void StartAllEnemies()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    public void SetAllEnemies(Player player, bool canAttack)
    {
        if (canAttack)
        {
            foreach (Enemy enemy in spawnedEnemies)
            {
                EnemyAI enemyAI = enemy.gameObject.GetComponent<EnemyAI>();
                enemyAI.ChasePlayer(player);
            }
        }
        else
        {
            foreach (Enemy enemy in spawnedEnemies)
            {
                EnemyAI enemyAI = enemy.gameObject.GetComponent<EnemyAI>();
                enemyAI.ProcessWalkAround();
            }
        }
    }

    void CreateEnemies()
    {
        spawnedEnemies.Clear();

        int highLevelIndex = 0;
        int randomLevel;

        for (int i = 0; i < enemyCount; i++)
        {
            if (highLevelIndex < highLevelEnemyCount)
            {
                highLevelIndex++;
                randomLevel = Random.Range(enemyMaxLevel, enemyMaxLevel + 100);
            }
            else
            {
                randomLevel = Random.Range(1, enemyMaxLevel);
            }

            GameObject enemyGo = Instantiate(enemyPrefab, transform);
            Enemy enemy = enemyGo.GetComponent<Enemy>();
            EnemyAI enemyAI = enemyGo.GetComponent<EnemyAI>();
            EnemyUI enemyUI = enemyGo.GetComponentInChildren<EnemyUI>();
            Rotator rotator = enemyGo.GetComponentInChildren<Rotator>();
            enemy.name = $"Enemy{i}";

            //enemy.SetUp(GetRandomPos(), randomLevel, growSizePerLevel, growTrailPerLevel);
            enemyAI.SetUp(Math.Clamp(baseEnemySpeed - (reduceSpeedPerLevel * randomLevel), 1f, 100f),
                growColliderPerLevel * randomLevel, growRadiusPerLevel * randomLevel);
            enemyUI.SetUp(randomLevel, growHeaderTextPerLevel, headerTextUpPerLevel);
            rotator.SetUp(reduceRotationPerLevel * randomLevel);

            spawnedEnemies.Add(enemy);
        }
    }
}