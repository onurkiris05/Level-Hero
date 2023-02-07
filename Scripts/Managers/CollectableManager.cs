using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectableManager : MonoBehaviour
{
    [Header("Collectable Prefab")] [SerializeField]
    private GameObject prefab;

    [SerializeField] private Material[] collectableMaterials;

    [Header("Spawn Area")] [SerializeField]
    private Collider spawnArea;

    [Header("Spawn Values")] [Range(0, 10f)] [SerializeField]
    private float noiseValueX;

    [Range(0, 10f)] [SerializeField] private float noiseValueZ;
    [SerializeField] private int divideCountX;
    [SerializeField] private int divideCountZ;

    [Header("Re Spawn Duration")] [SerializeField]
    private float reSpawnDuration;

    [SerializeField] private float spawnDelay;

    private const float SpawnedObjectYValue = 0.5f;

    private List<GameObject> _spawnedList = new();

    //TODO: Must be editor script
    private void Start()
    {
        //SpawnCollectables();
        StartCoroutine(CheckCollectable());
    }

    private IEnumerator CheckCollectable()
    {
        yield return reSpawnDuration;

        while (true)
        {
            var l = _spawnedList.Where(t => !t.activeSelf);
            foreach (var t in l)
            {
                t.SetActive(true);

                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitForSeconds(reSpawnDuration);
        }

        yield return null;
    }


#if UNITY_EDITOR
    [ContextMenu("Spawn")]
    public void SpawnCollectables()
    {
        _spawnedList.Clear();

        var bounds = spawnArea.bounds;

        var maxX = bounds.max.x;
        var maxZ = bounds.max.z;

        var minX = bounds.min.x;
        var minZ = bounds.min.z;

        var stepCountX = (maxX - minX) / divideCountX;
        var stepCountZ = (maxZ - minZ) / divideCountZ;

        var targetX = maxX;
        var targetZ = maxZ;

        for (var i = 0; i < divideCountX - 1; i++)
        {
            targetX -= stepCountX;

            for (var j = 0; j < divideCountZ - 1; j++)
            {
                targetZ -= stepCountZ;

                var randomValueX = Random.Range(-noiseValueX, noiseValueX);
                var randomValueZ = Random.Range(-noiseValueZ, noiseValueZ);

                var targetPosition =
                    Vector3.right * (targetX + randomValueX) +
                    Vector3.up * SpawnedObjectYValue +
                    Vector3.forward * (targetZ + randomValueZ);

                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                obj.transform.position = targetPosition;

                int randomIndex = Random.Range(0, collectableMaterials.Length);
                obj.GetComponent<MeshRenderer>().material = collectableMaterials[randomIndex];

                _spawnedList.Add(obj);
            }

            targetZ = maxZ;
        }

        // StartCoroutine(CheckCollectable());
    }
#endif
}