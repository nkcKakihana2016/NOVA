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

    public int AInum;      // AIの番号

    // 移動のパラメータ
    // AI
    Vector3 moveDir = Vector3.zero;                             // 移動方向
    float moveSpeed;                                            // 移動速度
    float moveSpeedMul = 1.0f;                                  // 移動速度への追従度
    float moveSpace = 150.0f;                                   // 移動可能距離(プレイヤーからの距離)

    bool isLookPlayer = false;                                  // プレイヤーを追従するか否か

    // マウスカーソル
    float cursorSpeed = 20.0f;
    float cursorSpeedMul = 1.0f;
    float cursorSpace = 30.0f;

    new void Awake()
    {
        // baseクラスのAwake()を実行する
        base.Awake();
    }

    void Start()
    {
        // 星をアクティブにする
        this.gameObject.SetActive(true);

        // 移動速度をランダムに取得する
        moveSpeed = UnityEngine.Random.Range(3.0f, 10.0f);
        // AIナンバーをランダムに取得する
        AInum = UnityEngine.Random.Range(0, 3);

        // 簡単なAIの挙動(プレイヤーの方向を向くか、ランダムな方向を向くか)
        switch (AInum)
        {
            case 0: // ランダム方向にまっすぐ進む
                moveDir.x = UnityEngine.Random.Range(-1.0f, 1.0f);
                moveDir.z = UnityEngine.Random.Range(-1.0f, 1.0f);
                break;
            case 1: // プレイヤーを追いかける
                moveSpace *= 0.5f;
                isLookPlayer = true;
                break;
            case 2: // プレイヤーから逃げる
                moveSpace *= 0.5f;
                isLookPlayer = true;
                moveSpeed = -moveSpeed;
                break;
        }

        this.FixedUpdateAsObservable()
            .Where(_ => !GameManager.Instance.isPause.Value)
            .Where(_ => starID != 2)
            .Subscribe(_ =>
            {
                // プレイヤーのポジションを取得
                Vector3 playerPos = GameManager.Instance.playerTransform.position;

                // プレイヤーが近くにいれば移動を実行、いなければ停止
                if (Vector3.Distance(this.transform.position, playerPos) <= moveSpace && !GameManager.Instance.isPause.Value)
                {
                    // プレイヤーの方向を向くか
                    if (isLookPlayer)
                    {
                        moveDir = (playerPos - this.transform.position).normalized;
                    }

                    // 速度と方向を計算して、力を加える
                    starRig.AddForce(moveSpeedMul * ((moveDir * moveSpeed) - starRig.velocity));
                }
                else
                {
                    starRig.velocity = Vector3.zero;
                }

                // マウスカーソルの影響を受ける(プレイヤーよりも少ない)
                if (Vector3.Distance(this.transform.position, GameManager.Instance.cursorPosition) 
                    <= cursorSpace + (this.transform.localScale.x / 2))
                {
                    // マウスカーソルへの方向を取得
                    Vector3 cursorDir = (GameManager.Instance.cursorPosition - this.transform.position).normalized;
                    // マウスカーソルの引力と斥力を判断して、力を加える
                    starRig.AddForce(cursorSpeedMul * ((cursorDir *(
                        (GameManager.Instance.cursorFlg) ? cursorSpeed : -cursorSpeed
                        )) - starRig.velocity));
                }

            }).AddTo(this.gameObject);

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Subscribe(c =>
            {
                if(c.gameObject.GetComponent<_StarParam>().starID == 3 &&
                    transform.localScale.x < c.transform.localScale.x)
                {
                    // Destroy(this.gameObject);
                }

                ParticleSystem ps = Instantiate(enemyPS);
                ps.transform.position = this.transform.position;

            }).AddTo(this.gameObject);
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

    // 消滅情報をスポーンクラスに送る
    public void Stop()
    {
        try
        {
            PlanetSpawner.Instance.PlanetDestroy();
        }
        catch
        {

        }
    }
}
