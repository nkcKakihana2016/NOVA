using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

public class PlanetSpawner : MonoBehaviour
{
    // 惑星自動生成スクリプト

    [SerializeField] private int planetMaxnum;                  // 最大スポーン数
    [SerializeField] private int count;                         // 現在のスポーン数
    [SerializeField] private int planetObjNum;                  // スポーンするプレハブの種類

    [SerializeField] private GameObject[] planetPre;            // スポーンする惑星をここに格納
    [SerializeField] private List<GameObject> pooledObjList;    // キャッシュ後の惑星をここに格納
    [SerializeField] private Transform hierarchyTrans;


    int interval = 1;
    // Start is called before the first frame update
    void Start()
    {
        // 2秒ごとに実行
        Observable.IntervalFrame(120)
            .Do(_ => Debug.Log("PlanetCreate")).Subscribe(_ =>
            {
                if(count < planetMaxnum)
                {
                    PlanetCreate();
                }
                //planetObjNum = Random.Range(0, planetPre.Length);
                //planetPool._planetsPre = planetPre[planetObjNum];
                //var pos = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));
                //var planet = planetPool.Rent();
            });
        //InvokeRepeating("PlanetCreate", 3, 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlanetCreate()
    {
        if (count == planetMaxnum) return; // 30回生成されたらこのメソッドは起動しない
        planetObjNum = Random.Range(0, planetPre.Length); // 生成したい惑星を取得
        float x = Random.Range(-20.0f, 20.0f); // 生成座標の設定
        float y = 0.0f;
        float z = Random.Range(-20.0f, 20.0f);
        Vector3 spawnPos = new Vector3(x, y, z);
        // 惑星生成
        Instantiate(planetPre[planetObjNum], spawnPos, Quaternion.identity);
        count++;
    }
    }
