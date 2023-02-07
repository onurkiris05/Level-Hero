using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VP.Nest.Haptic;

public class EnemyAttacker : MonoBehaviour
{
    private Enemy enemy;
    private EnemyAI enemyAI;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() != null)
        {
            Player player = other.GetComponentInParent<Player>();

            if (player.isActive)
            {
                if (enemy.CurrentLevel > player.CurrentLevel)
                {
                    HapticManager.Haptic(HapticType.HeavyImpact);
                    enemy.PlaySuccessParticle();
                    player.MakeCollision(transform.position);
                    enemyAI.ProcessWalkAround();
                }
                else if (enemy.CurrentLevel < player.CurrentLevel)
                {
                    HapticManager.Haptic(HapticType.HeavyImpact);
                    player.PlaySuccessParticle();
                    enemy.Die();
                    player.IncreaseSizeAndLevel(enemy.CurrentLevel);
                }
            }
        }
    }
}