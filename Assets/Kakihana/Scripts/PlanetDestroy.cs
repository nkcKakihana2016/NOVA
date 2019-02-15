using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDestroy : MonoBehaviour
{
    // 惑星が持つスクリプト 30秒経つと消える
    [SerializeField] PlanetSpawner spawner;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 30.0f)
        {
            // 30秒経つとにスポーンクラスに消滅情報を送る
            FindObjectOfType<PlanetSpawner>().PlanetDestroy();
            Destroy(this.gameObject);
        }
    }
}
