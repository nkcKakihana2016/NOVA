using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRot : MonoBehaviour
{
    // マテリアルの確認用の球が回るだけのスクリプトです

    Material mat;
    bool tFlg;

	void Start ()
    {
        mat = GetComponent<Renderer>().material;
	}

	void Update ()
    {
        float t = mat.GetFloat("_Threshold");

        if (t <= 0.1f)
        {
            mat.SetFloat("_Threshold", 0.1f);
            tFlg = true;
        }
        else if(t >= 0.9f)
        {
            mat.SetFloat("_Threshold", 0.9f);
            tFlg = false;
        }

        t += (tFlg) ? 0.01f : -0.01f;
        mat.SetFloat("_Threshold", t);

        transform.eulerAngles += new Vector3(0.0f, 60.0f * Time.deltaTime, 0.0f);
    }
}
