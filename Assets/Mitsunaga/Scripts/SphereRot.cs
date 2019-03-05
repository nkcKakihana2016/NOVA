﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRot : MonoBehaviour
{
    // マテリアルの確認用の球が回るだけのスクリプトです
    [SerializeField, Header("回転速度")]
    float rotationSpeed = 30.0f;
    [SerializeField,Header("自作シェーダーか否か")]
    bool isSphere = true;

    Material mat;
    bool tFlg;

	void Start ()
    {
        if (isSphere)
        {
            mat = GetComponent<Renderer>().material;
            StartCoroutine(matCor());
        }
	}

	void FixedUpdate ()
    {
        transform.eulerAngles += new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
    }

    IEnumerator matCor()
    {
        float t = mat.GetFloat("_Threshold");

        while (true)
        {
            if (t <= 0.1f)
            {
                mat.SetFloat("_Threshold", 0.1f);
                tFlg = true;
            }
            else if (t >= 0.9f)
            {
                mat.SetFloat("_Threshold", 0.9f);
                tFlg = false;
            }

            t += (tFlg) ? 0.01f : -0.01f;
            mat.SetFloat("_Threshold", t);

            yield return null;
        }
    }
}
