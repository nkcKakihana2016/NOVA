﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

public class PlanetSpawner : MonoBehaviour
{
    // 惑星自動生成スクリプト

    [Header("シーン毎に設定が必要な変数")]

    [SerializeField] private int planetMaxnum;                  // 最大スポーン数
    [SerializeField] private int hotSpotMax;                    // ボス周辺エリアの最大スポーン数

    [SerializeField] private float hotSpotRadiusMax;            // ボス周辺エリアスポーン範囲の最大半径
    [SerializeField] private float hotSpotRadiusMin;            // ボス周辺エリアスポーン範囲の最小半径

    [Header("シーン毎に設定が必要なコンポーネント")]
    [SerializeField] private Transform bossObjTrans;            // ボスオブジェクトのトランスフォーム
    [SerializeField] private GameObject[] planetPre;            // スポーンする惑星をここに格納
    [SerializeField] private Transform[] planetPreTrans;        // スポーンする惑星のトランスフォーム

    [Header("自動可動し、設定する必要がないもの")]
    [SerializeField] private int count;                         // 現在のスポーン数
    [SerializeField] private int planetObjNum;                  // スポーンするプレハブの種類
    [SerializeField] private float bossRadius;                  // ボスオブジェクトの円周
    [SerializeField] private float xAbs, zAbs;                  // スポーン先座標の絶対値
    [SerializeField] private float maxR, minR;                  // ボス周辺エリアスポーンの最小範囲と最大範囲を2乗したもの

    [SerializeField] private Vector3 spawnPos;                  // スポーンする惑星の座標
    // Start is called before the first frame update
    void Start()
    {
        bossRadius = bossObjTrans.localScale.x * Mathf.PI; // ボスオブジェクトの円周を求める

        // ボス周辺エリアの半径を2乗する
        maxR = Mathf.Pow(hotSpotRadiusMax, 2);
        minR = Mathf.Pow(hotSpotRadiusMin, 2);

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
            }).AddTo(this.gameObject);
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

        spawnPos.x = Random.Range(-20.0f, 20.0f); // 生成座標の設定
        spawnPos.y = 0.0f;
        spawnPos.z = Random.Range(-20.0f, 20.0f);

        Instantiate(planetPre[planetObjNum], spawnPos, Quaternion.identity);
        count++;
    }
    // ボス周辺エリア専用スポーン
    private void HotSpotCreate()
    {
        if (count == hotSpotMax) return;                  // 設定されてる値を超えたらこのメソッドは起動しない
        planetObjNum = Random.Range(0, planetPre.Length); // 生成したい惑星を取得
        RaycastHit hit;                                   // 惑星重なり防止用Rayの当たり判定

        // スポーン予定の惑星の半径を取得
        var radius = planetPreTrans[planetObjNum].localScale.x * 0.5f;

        // スポーン座標をランダムで生成
        spawnPos.x = Random.Range(-hotSpotRadiusMax, hotSpotRadiusMax);
        spawnPos.z = Random.Range(-hotSpotRadiusMax, hotSpotRadiusMax);

        xAbs = Mathf.Abs(Mathf.Pow(spawnPos.x, 2));
        zAbs = Mathf.Abs(Mathf.Pow(spawnPos.z, 2));
        // 惑星をスポーンする前にスポーンしたい惑星の大きさと同じ球型Rayを飛ばす
        if (Physics.SphereCast(spawnPos,radius,Vector3.down,out hit))
        {
            // 既に惑星がいる場合はスポーン不可
            Debug.DrawRay(spawnPos, hit.point, Color.red,5);
            Debug.Log("スポーン不可");
        }
        else
        {
            // スポーン可能な場合、スポーン先座標が半径の2乗以内であればスポーンする
            if (maxR > xAbs + zAbs && zAbs + zAbs > minR)
            {
                Debug.DrawRay(spawnPos, hit.point, Color.red);
                Debug.Log("惑星スポーン");
                // 惑星生成
                Instantiate(planetPre[planetObjNum], spawnPos + bossObjTrans.position, Quaternion.identity);
                count++;
            }
            else
            {
                Debug.Log("スポーン範囲外");
            }
        }
    }

    // 惑星が消滅するときに呼び出される
    public void PlanetDestroy()
    {
        // 現在の生成数を減らす
        count--;
    }

    // デバッグ用ボス周辺エリアを可視化する
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawSphere(bossObjTrans.position, bossRadius);
    }
}
