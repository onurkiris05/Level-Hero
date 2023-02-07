using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject levelCanvas;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem redNovaParticle, greenNovaParticle;

    public int CurrentLevel;
    public bool isIndicated;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        SetUp(CurrentLevel, 0.005f, 0.1f);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetUp( /*Vector3 pos, */ int level, float growPerLevel, float growTrailPerLevel)
    {
        //transform.position = pos;
        CurrentLevel = level;

        body.localScale = new Vector3(body.localScale.x + (growPerLevel * level),
            body.localScale.y + (growPerLevel * level),
            body.localScale.z + (growPerLevel * level));

        trailRenderer.startWidth += growTrailPerLevel;
        trailRenderer.time += growTrailPerLevel / 8f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(meshRenderer.material.color, 0.0f),
                new GradientColorKey(meshRenderer.material.color, 1.0f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0.2f, 1.0f), new GradientAlphaKey(1f, 0.0f) }
        );
        trailRenderer.colorGradient = gradient;

        EnemyManager.spawnedEnemies.Add(this);
    }

    public void SetMaterial(int playerLevel)
    {
        if (CurrentLevel > playerLevel)
        {
            meshRenderer.material.color = Color.red;
        }
        else
        {
            meshRenderer.material.color = Color.green;
        }

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(meshRenderer.material.color, 0.0f),
                new GradientColorKey(meshRenderer.material.color, 1.0f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0.2f, 1.0f), new GradientAlphaKey(1f, 0.0f) }
        );
        trailRenderer.colorGradient = gradient;
    }

    public void Die()
    {
        StartCoroutine(ProcessDie());
    }

    public void PlaySuccessParticle()
    {
        redNovaParticle.Play();
    }

    IEnumerator ProcessDie()
    {
        greenNovaParticle.Play();
        levelCanvas.SetActive(false);
        body.gameObject.SetActive(false);
        navMeshAgent.isStopped = true;
        meshRenderer.enabled = false;
        trailRenderer.enabled = false;

        yield return new WaitForSeconds(greenNovaParticle.main.duration);

        EnemyManager.spawnedEnemies.Remove(this);
        Destroy(gameObject);
    }
}