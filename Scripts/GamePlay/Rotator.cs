using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rawRotationSpeed;

    //FOR DEBUG
    public float rotationSpeed;
    //

    public void SetUp(float value)
    {
        rotationSpeed = rawRotationSpeed - value;
        rotationSpeed = Math.Clamp(rotationSpeed, 1f, 100f);
        transform.DOLocalRotate(Vector3.right * rotationSpeed, 0.1f).SetLoops(-1, LoopType.Incremental);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}