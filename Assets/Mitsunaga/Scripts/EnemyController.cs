using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;
using UniRx.Triggers;


public class EnemyController : _StarParam
{
    // パラメータ
    [SerializeField] PlanetSpawner spawner; // 惑星が持つスクリプト 30秒経つと消える

    [SerializeField, Header("衝突時のパーティクルのプレハブ")]
    ParticleSystem enemyPS;

    // エネミーのAIを管理するサブジェクト
    public Subject<int> enemyAISubject = new Subject<int>();

    // 初期設定
    new void Awake()
    {
        base.Awake();

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Subscribe(c =>
            {
                ParticleSystem ps = Instantiate(enemyPS);
                ps.transform.position = this.transform.position;

            }).AddTo(this.gameObject);
    }

    void Start()
    {
        this.gameObject.SetActive(true);
    }

    // 惑星スポーンの座標設定
    public void PlanetSpawn(Vector3 pos)
    {
        transform.position = pos;
    }

    // 惑星スポーンの設定（オーバーロード、スケール値追加）
    public void PlanetSpawn(Vector3 pos, float scale)
    {
        transform.position = pos;
        SetStarSize(scale);
    }

    // 惑星スポーンの設定（オーバーロード、AIの番号を追加）
    public void PlanetSpawn(Vector3 pos, float scale, int AInum)
    {
        transform.position = pos;
        SetStarSize(scale);

        // AIのナンバーを適用する
        enemyAISubject.OnNext(AInum);
    }

    // 消滅情報をスポーンクラスに送る
    public void Stop()
    {
        PlanetSpawner.Instance.PlanetDestroy();
    }
}
