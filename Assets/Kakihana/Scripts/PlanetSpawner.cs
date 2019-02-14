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
    [SerializeField] private int hotSpotMax;                    // ボス周辺エリアの最大スポーン数
    [SerializeField] private int count;                         // 現在のスポーン数
    [SerializeField] private int planetObjNum;                  // スポーンするプレハブの種類
    [SerializeField] private float bossRadius;                  // ボスオブジェクトの円周

    [SerializeField] private Transform bossObjTrans;            // ボスオブジェクトのトランスフォーム
    [SerializeField] private GameObject[] planetPre;            // スポーンする惑星をここに格納
    [SerializeField] private List<GameObject> pooledObjList;    // キャッシュ後の惑星をここに格納
    [SerializeField] private Transform[] planetPreTrans;        // スポーンする惑星のトランスフォーム


    int interval = 1;
    // Start is called before the first frame update
    void Start()
    {
        bossRadius = bossObjTrans.localScale.x * Mathf.PI; // ボスオブジェクトの円周を求める

        // 2秒ごとに実行
        Observable.IntervalFrame(120)
            .Do(_ => Debug.Log("PlanetCreate")).Subscribe(_ =>
            {
                if(count < planetMaxnum)
                {
                    if (count < hotSpotMax)
                    {
                        // 惑星は一定値になるまでボス周辺エリアにスポーンする
                        HotSpotCreate();
                        Debug.Log("HotSpotSpawn");
                    }
                    else
                    {
                        // 一定値を超えると最大スポーン数まですべての範囲でスポーンする
                        PlanetCreate();
                        Debug.Log("NormalSpawn");
                    }
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

    // 通常スポーン用
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
    // ボス周辺エリア専用スポーン
    private void HotSpotCreate()
    {
        if (count == hotSpotMax) return;
        planetObjNum = Random.Range(0, planetPre.Length); // 生成したい惑星を取得
        float x = Random.Range(-bossRadius, bossRadius); // 生成座標の設定
        float y = 0.0f;
        float z = Random.Range(-bossRadius, bossRadius);
        Vector3 spawnPos = new Vector3(x, y, z);
        RaycastHit hit;
        var radius = planetPreTrans[planetObjNum].localScale.x * 0.5f;
        if(Physics.SphereCast(spawnPos,radius,Vector3.forward,out hit))
        {
            Debug.DrawRay(spawnPos, hit.point, Color.red);
            Debug.Log("設置不可");
        }
        else
        {
            Debug.DrawRay(spawnPos, hit.point, Color.red);
            Debug.Log("設置");
            Instantiate(planetPre[planetObjNum], spawnPos, Quaternion.identity);
        }
        // 惑星生成
        //Instantiate(planetPre[planetObjNum], spawnPos, Quaternion.identity);
        count++;
    }

    public void PlanetDestroy()
    {
        count--;
    }

    // デバッグ用ボス周辺エリアを可視化する
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawSphere(bossObjTrans.position, bossRadius);
    }
}
