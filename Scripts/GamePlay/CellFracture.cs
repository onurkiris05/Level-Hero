using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFracture : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetMaterial;
    [SerializeField] private MeshRenderer[] fractureMaterials;

    private void OnEnable()
    {
        for (int i = 0; i < fractureMaterials.Length; i++)
        {
            fractureMaterials[i].material = targetMaterial.material;
        }
    }
}