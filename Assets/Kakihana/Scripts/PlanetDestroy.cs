﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;

public class PlanetDestroy : MonoBehaviour
{
    
    [SerializeField] PlanetSpawner spawner; // 惑星が持つスクリプト 30秒経つと消える
    public Transform myTrans;               // 自身のトランスフォーム
    string sceneName;                       // スポーン時点でのシーン名
    float time;                             // スポーンしてからの経過時間

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
        // スポーン時にシーン名を取得
        sceneName = PlanetSpawner.Instance.NowSceneName();
        // シーンが切り替わってもDestroyしない問題を解決
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            Destroy(this.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間をカウントし格納
        //time += Time.deltaTime;
        //if (time > 30.0f)
        //{
        //    // 30秒経つとにスポーンクラスに消滅情報を送る
        //    PlanetSpawner.Instance.PlanetDestroy();
        //    Destroy(this.gameObject);
        //}
    }

    // 惑星スポーンの座標設定
    public void PlanetSpawn(Vector3 pos)
    {
        transform.position = pos;

        //return Observable.TakeUntil(this.OnDisableAsObservable(Stop())).Subscribe().AddTo(this.gameObject);
    }

    // 消滅情報をスポーンクラスに送る
    public void Stop()
    {
        PlanetSpawner.Instance.PlanetDestroy();
        this.gameObject.SetActive(false);
    }

    // 惑星非表示メソッド
    //public IObservable<Unit> Stop()
    //{
    //    var stop = Observable.EveryUpdate().Where(_ => this.OnDiasbleAsObservable());
    //    var r = Observable.EveryUpdate().Where(this.OnEnableAsObservable()).TakeUntil(this.OnDisableAsObservable().Subscribe(_ =>
    //    {
    //        PlanetSpawner.Instance.PlanetDestroy();
    //        this.gameObject.SetActive(false);
    //    }).AddTo(this.gameObject));
    //    30秒経つとにスポーンクラスに消滅情報を送る
    //    /*
    //                 PlanetSpawner.Instance.PlanetDestroy();
    //        this.gameObject.SetActive(false);*/
    //    Destroy(this.gameObject);
    //    下はオブジェクトプール用

    //}
}
