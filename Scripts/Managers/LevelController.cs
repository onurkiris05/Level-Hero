using System;
using System.Collections;
using System.Collections.Generic;
using ElephantSDK;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public int TargetLevel;

    private void Awake()
    {
        TargetLevel = RemoteConfigManager.TargetLevels[(PlayerPrefKeys.CurrentLevel - 1) % 4];
    }
}