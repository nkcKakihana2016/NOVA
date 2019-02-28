﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;

[System.Serializable]
public class PlanetPool : ObjectPool<PlanetDestroy>
{

    /* 
     【オブジェクトプールクラス】
     Unity標準機能のInstantiateとDestroyでは負荷が大きいのでオブジェクトが必要でなければ非表示にし
     必要な時に初期化して再び表示させる 
    */
    public readonly PlanetDestroy planetObj; // プールしたいプレファブ
    private Transform myTrans;               // プールしたオブジェクトをまとめるオブジェクトの座標

    // コンストラクタ
    public PlanetPool(Transform trans,PlanetDestroy planetPre)
    {
        myTrans = trans;
        planetObj = planetPre;
    }

    // 惑星をスポーンさせる
    protected override PlanetDestroy CreateInstance()
    {
        var e = GameObject.Instantiate(planetObj);
        e.transform.SetParent(myTrans);

        return e;
    }
}
