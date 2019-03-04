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
        base.Awake();

        moveSpeed = UnityEngine.Random.Range(10.0f, 30.0f);
    }

    void Start()
    {
        this.gameObject.SetActive(true);

        AInum = UnityEngine.Random.Range(0, 3);

        // AIを起動
        Observable.FromCoroutine<string>(observer => AICoroutine(observer, AInum))
            .Subscribe(i => Debug.Log("EnemyAI : " + i.ToString()));

        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                // マウスカーソルの影響を受ける(プレイヤーよりも少ない)
                if (Vector3.Distance(this.transform.position, GameManager.Instance.cursorPosition) 
                    <= cursorSpace + (this.transform.localScale.x / 2))
                {
                    Vector3 cursorDir = (GameManager.Instance.cursorPosition - this.transform.position).normalized;   // マウスカーソルへの方向を取得
                    starRig.AddForce(cursorSpeedMul * ((cursorDir * cursorSpeed) - starRig.velocity));
                }

            }).AddTo(this.gameObject);

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Subscribe(c =>
            {
                ParticleSystem ps = Instantiate(enemyPS);
                ps.transform.position = this.transform.position;

            }).AddTo(this.gameObject);
    }

    IEnumerator AICoroutine(IObserver<string> observer ,int AInumber)
    {
        

        switch (AInumber)
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

        observer.OnNext("AI Number : " + AInumber.ToString() + "   Speed : " + moveSpeed.ToString());

        while (this.gameObject.activeInHierarchy)
        {
            Vector3 playerPos = GameManager.Instance.playerTransform.position;

            // プレイヤーが近くにいれば移動を実行、いなければ停止
            if (Vector3.Distance(this.transform.position, playerPos) <= moveSpace)
            {

                if (isLookPlayer)
                {
                    moveDir = (playerPos - this.transform.position).normalized;
                }

                starRig.AddForce(moveSpeedMul * ((moveDir * moveSpeed) - starRig.velocity));
            }
            else
            {
                starRig.velocity = Vector3.zero;
            }

            yield return null;
        }
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
    public void PlanetSpawn(Vector3 pos, float scale, int AInumber)
    {
        transform.position = pos;
        SetStarSize(scale);

        // AIのナンバーを適用する
        AInum = AInumber;
    }

    // 消滅情報をスポーンクラスに送る
    public void Stop()
    {
        PlanetSpawner.Instance.PlanetDestroy();
    }
}
