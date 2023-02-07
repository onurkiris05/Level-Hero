using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextRotator : MonoBehaviour
{
    public bool isEnemy;

    private void LateUpdate()
    {
        if (isEnemy)
        {
            Quaternion camRot = Camera.main.transform.rotation;

            transform.LookAt(transform.position + camRot * Vector3.forward,
                camRot * Vector3.up);
        }
        else
        {
            Vector3 eulerAngle = Camera.main.transform.rotation.eulerAngles;
            eulerAngle -= new Vector3(50, 0f, 0f);

            transform.LookAt(transform.position + Quaternion.Euler(eulerAngle) * Vector3.forward,
                Quaternion.Euler(eulerAngle) * Vector3.up);
        }
    }
}