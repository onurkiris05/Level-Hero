using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private Vector2 scrollingSpeed;

    private Material material;

    void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        material.mainTextureOffset += scrollingSpeed * Time.deltaTime;
    }
}
