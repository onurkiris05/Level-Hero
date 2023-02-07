using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ElephantSDK;
using TMPro;
using UnityEngine;
using VP.Nest.CameraControl;
using VP.Nest.Haptic;
using VP.Nest.SceneManagement;
using VP.Nest.UI;
using VP.Nest.UI.InGame;

public class Player : Singleton<Player>
{
    [Header("General Components")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject follower;
    [SerializeField] private Transform particleHolder;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private RectTransform levelHeader;
    [SerializeField] private LevelController levelController;
    [SerializeField] private InGameUI inGameUI;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject[] fractureClone;

    [Header("Player Settings")]
    public int CurrentLevel;
    public bool isActive = true;
    public bool isBeingFollowed;
    [SerializeField] private float impactForce;
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private AudioClip eatSound, beatenSound;

    [Header("Header Text Settings")]
    [SerializeField] private Vector3 growHeaderTextPerCollect;
    [SerializeField] private float headerTextUpPerCollect;

    [Header("Grow Settings")]
    [SerializeField] private Vector3 camOffsetPerCollect;
    [SerializeField] private float growPerLevel;
    [SerializeField] private float growTrailPerCollect;
    [SerializeField] private float growParticlePerCollect;
    [SerializeField] private float jumpOffsetPerLevel;

    [Header("VFX Settings")]
    [SerializeField] private ParticleSystem growParticle;
    [SerializeField] private ParticleSystem winParticle;
    [SerializeField] private ParticleSystem collectableParticle;

    private TextMeshProUGUI levelText;
    private TextMeshProUGUI headerText;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private int guid;
    private bool isWinner;
    private bool isLooser;
    private float jumpOffset = 6f;
    private int index = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        levelText = levelHeader.GetChild(0).GetComponent<TextMeshProUGUI>();
        headerText = levelHeader.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        enemyManager.SetEnemyTextsAndMaterials(CurrentLevel);

        SetMeUp();
    }

    public void PlaySuccessParticle()
    {
        growParticle.Play();
    }

    public void PlayCollectableParticle()
    {
        collectableParticle.Play();
    }

    public void MakeCollision(Vector3 enemyPos)
    {
        StartCoroutine(ProcessMakeCollision(enemyPos));
    }

    IEnumerator ProcessMakeCollision(Vector3 enemyPos)
    {
        isActive = false;

        playerMovement.enabled = false;

        AudioSource.PlayClipAtPoint(beatenSound, Camera.main.transform.position);

        Vector3 dir = (transform.position - enemyPos).normalized;
        dir.y = 0f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * impactForce, ForceMode.VelocityChange);

        StartCoroutine(ProcessDie());
        //DecrementSizeAndLevel();

        yield return new WaitForSeconds(1f);

        playerMovement.enabled = true;
    }

    public void IncreaseSizeAndLevel(int multiply)
    {
        if (isWinner /*|| !isActive*/) return;

        AudioSource.PlayClipAtPoint(eatSound, Camera.main.transform.position);
        growParticle.Play();

        for (int i = 0; i < multiply; i++)
        {
            CurrentLevel++;

            transform.DOComplete();
            transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f).SetEase(Ease.OutBack);

            IncrementValues();

            //Re-position camera
            CameraManager.Instance.SmoothIncreaseCamera(camOffsetPerCollect);

            // if (CurrentLevel >= levelController.TargetLevel)
            // {
            //     isWinner = true;
            //     StartCoroutine(ProcessWinSequence());
            //
            //     return;
            // }
        }

        PlayerPrefs.SetInt("PlayerLevel", CurrentLevel);

        enemyManager.SetEnemyTextsAndMaterials(CurrentLevel);
    }

    private void DecrementSizeAndLevel()
    {
        int count = Mathf.CeilToInt(CurrentLevel / 2f);

        for (int i = 0; i < count; i++)
        {
            CurrentLevel--;

            transform.DOComplete();
            transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f).SetEase(Ease.OutBack);

            DecrementValues();

            //Re-position camera
            CameraManager.Instance.SmoothDecreaseCamera(camOffsetPerCollect);

            if (CurrentLevel < 1)
            {
                StartCoroutine(ProcessDie());
                return;
            }
        }

        PlayerPrefs.SetInt("PlayerLevel", CurrentLevel);

        enemyManager.SetEnemyTextsAndMaterials(CurrentLevel);

        isActive = true;
    }

    IEnumerator ProcessWinSequence()
    {
        HapticManager.Haptic(HapticType.MediumImpact);
        PlayerPrefs.SetInt("PlayerLevel", 1);
        enemyManager.EliminateAllEnemies();
        playerMovement.Stop();
        levelHeader.gameObject.SetActive(false);
        winParticle.Play();

        transform.DOMoveY(jumpOffset, 0.3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        yield return new WaitForSeconds(3f);

        UIManager.Instance.SuccessGame();
    }

    IEnumerator ProcessDie()
    {
        isActive = false;
        follower.SetActive(false);
        meshRenderer.enabled = false;
        enemyManager.SetAllEnemies(this,false);

        fractureClone[index].SetActive(true);
        //PlayerPrefs.SetInt("PlayerLevel", 1);

        yield return new WaitForSeconds(3f);

        fractureClone[index].SetActive(false);
        index++;

        meshRenderer.enabled = true;
        follower.SetActive(true);

        yield return new WaitForSeconds(5f);

        isActive = true;
        enemyManager.SetAllEnemies(this,true);

        // gameObject.SetActive(false);
        //UIManager.Instance.FailGame();
    }

    private void SetMeUp()
    {
        //CurrentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        inGameUI.UpdateTargetUI(CurrentLevel);

        //playerMovement.MaxSpeed = RemoteConfigManager.PlayerSpeed;
        AdjustMaterial();

        for (int i = 0; i < CurrentLevel; i++)
        {
            IncrementValues();

            //Re-position camera
            CameraManager.Instance.RawAdjustCamera(camOffsetPerCollect);
        }
    }

    private void IncrementValues()
    {
        //Update header text
        levelText.text = $"Lv {CurrentLevel}";

        //Update fillbar
        inGameUI.UpdateTargetUI(CurrentLevel);

        //Update jump offset
        jumpOffset += jumpOffsetPerLevel;

        //Update particles size
        particleHolder.localScale = new Vector3(particleHolder.localScale.x + growParticlePerCollect, particleHolder.localScale.y + growParticlePerCollect, particleHolder.localScale.z + growParticlePerCollect);

        //Increase player max speed
        //playerMovement.MaxSpeed += RemoteConfigManager.PlayerSpeedIncrease;

        //Update own transform
        transform.localScale = new Vector3(transform.localScale.x + growPerLevel, transform.localScale.y + growPerLevel, transform.localScale.z + growPerLevel);

        //Move up header text
        levelHeader.anchoredPosition = new Vector2(levelHeader.anchoredPosition.x, levelHeader.anchoredPosition.y + headerTextUpPerCollect);

        //Resize header text
        levelHeader.localScale = new Vector3(levelHeader.localScale.x + growHeaderTextPerCollect.x, levelHeader.localScale.y + growHeaderTextPerCollect.y, levelHeader.localScale.z + growHeaderTextPerCollect.z);

        //Resize trail
        trailRenderer.startWidth += growTrailPerCollect;
    }

    private void DecrementValues()
    {
        //Update header text
        levelText.text = $"Lv {CurrentLevel}";

        //Update fillbar
        inGameUI.UpdateTargetUI(CurrentLevel);

        //Update jump offset
        jumpOffset -= jumpOffsetPerLevel;

        //Update particles size
        particleHolder.localScale = new Vector3(particleHolder.localScale.x - growParticlePerCollect, particleHolder.localScale.y - growParticlePerCollect, particleHolder.localScale.z - growParticlePerCollect);

        //Increase player max speed
        //playerMovement.MaxSpeed -= RemoteConfigManager.PlayerSpeedIncrease;

        //Update own transform
        transform.localScale = new Vector3(transform.localScale.x - growPerLevel, transform.localScale.y - growPerLevel, transform.localScale.z - growPerLevel);

        //Move up header text
        levelHeader.anchoredPosition = new Vector2(levelHeader.anchoredPosition.x, levelHeader.anchoredPosition.y - headerTextUpPerCollect);

        //Resize header text
        levelHeader.localScale = new Vector3(levelHeader.localScale.x - growHeaderTextPerCollect.x, levelHeader.localScale.y - growHeaderTextPerCollect.y, levelHeader.localScale.z - growHeaderTextPerCollect.z);

        //Resize trail
        trailRenderer.startWidth -= growTrailPerCollect;
    }

    private void AdjustMaterial()
    {
        //Re-adjust player material
        meshRenderer.material = playerMaterials[(PlayerPrefKeys.CurrentLevel - 1) % 4];

        //Adjust header text color
        headerText.color = meshRenderer.material.color;

        //Change color of trail
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[]
        {
            new GradientColorKey(meshRenderer.material.color, 0.0f),
            new GradientColorKey(meshRenderer.material.color, 1.0f)
        }, new GradientAlphaKey[] { new GradientAlphaKey(0.2f, 1.0f), new GradientAlphaKey(1f, 0.0f) });
        trailRenderer.colorGradient = gradient;
    }
}