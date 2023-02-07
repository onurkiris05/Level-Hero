using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private RectTransform headerTextRect;

    private Enemy enemy;

    private void OnEnable()
    {
        enemy = GetComponentInParent<Enemy>();

        SetUp(enemy.CurrentLevel, 0.002f, 0.006f);
    }

    public void SetUp(int level, float growHeaderTextPerLevel, float headerTextUpPerCollect)
    {
        headerText.text = $"Lv{level}";

        headerTextRect.localScale = new Vector3(headerTextRect.localScale.x + (growHeaderTextPerLevel * level),
            headerTextRect.localScale.y + (growHeaderTextPerLevel * level),
            headerTextRect.localScale.z + (growHeaderTextPerLevel * level));

        headerTextRect.anchoredPosition = new Vector2(headerTextRect.anchoredPosition.x,
            headerTextRect.anchoredPosition.y + (headerTextUpPerCollect * level));
    }

    public void SetHeaderText(int playerLevel, int enemyLevel)
    {
        if (enemyLevel > playerLevel)
        {
            headerText.color = Color.red;
        }
        else
        {
            headerText.color = Color.green;
        }
    }
}