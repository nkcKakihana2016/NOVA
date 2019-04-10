using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;
using Cinemachine;

public class PlayerController : _StarParam
{
    // プレイヤーとカメラのコントロールを行う
    [SerializeField, Header("ボスのトランスフォーム,ラインレンダラー")]
    Transform bossTransform;
    [SerializeField]
    LineRenderer linePtB;

    // 星の移動関連
    [SerializeField, Header("星の加速度、加速度への追従度、反応距離")]
    float moveSpeed = 10.0f;
    [SerializeField]
    float moveSpeedMul = 2.0f;

    // マウスカーソル関連
    [SerializeField, Header("マウスカーソルを追従するオブジェクト、その中身(0黒1白)")]
    Transform cursorParent;     // カーソルに追従させる実際のオブジェクト(このオブジェクトの下にホールのパーティクルを入れる)
    [SerializeField]
    GameObject[] holes;         // 0にブラックホール、1にホワイトホール
    bool holeFlg;               // trueならブラックホール(0)、falseならホワイトホール(1)

    // 衝突関連
    [SerializeField, Header("星の衝突時、合体時の待ち時間、衝突時のパーティクル")]
    float hitStopTime = 0.2f;
    [SerializeField]
    float waitCount = 4.5f;
    [SerializeField]
    ParticleSystem[] hitPS;

    // カメラ関連
    [SerializeField, Header("シネマシーンのカメラ")]
    CinemachineVirtualCamera vcam;
    const float CDISTANCE = 100.0f; // カメラの引き

    // 音楽関連
    AudioSource collisionAudioSource;

    // 定数　このへんもっと分かりやすい変数名教えてくれ…
    const float MOVEDISTANCE = 20.0f;   // マウスの反応する距離　これ+星の直径

    new void Awake()
    {
        base.Awake();

        // プレイヤー情報
        GameManager.Instance.playerTransform = this.gameObject.transform;
        collisionAudioSource = GetComponent<AudioSource>();

        holeFlg = true;
        holes[1].SetActive(false);
        holes[0].SetActive(true);

        SetCamera();
    }

    void Start()
    {
        // ラインレンダラーの情報を指定
        linePtB.startWidth = 0.1f;  // 開始点の幅
        linePtB.endWidth = 0.1f;    // 終点の幅
        linePtB.positionCount = 2;  // 頂点の数

        // クリックでポーズを解除する
        this.UpdateAsObservable()
            .Where(c => GameManager.Instance.isPause.Value)
            .Where(c => !GameManager.Instance.isGameOver.Value)
            .Where(c => !GameManager.Instance.isClear.Value)
            .Where(c => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                GameManager.Instance.isPause.Value = false;
            })
            .AddTo(this.gameObject);

        // 毎フレーム呼び出される
        this.UpdateAsObservable()
            .Where(c => !GameManager.Instance.isPause.Value)
            .Subscribe(_ => 
            {
            // プレイヤー情報
                GameManager.Instance.playerTransform = this.gameObject.transform;

            // マウスのクリック処理
                if (Input.GetMouseButtonDown(0))
                {
                    // 移動速度を反転させる
                    moveSpeed = -moveSpeed;

                    if (holeFlg)
                    {
                        holeFlg = !holeFlg;

                        holes[0].SetActive(false);
                        holes[1].SetActive(true);
                    }
                    else
                    {
                        holeFlg = !holeFlg;

                        holes[1].SetActive(false);
                        holes[0].SetActive(true);
                    }
                }

            // マウスカーソル・移動処理
                MoveCursor();

            // ボスとの間に線を引く
                linePtB.SetPosition(0, transform.position);     // 開始点の座標
                linePtB.SetPosition(1, bossTransform.position); // 終点の座標

            });

        Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe(c =>
        {
            SetCamera();
        })
        .AddTo(this.gameObject);

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Where(c => c.gameObject.GetComponent<_StarParam>().starID != 1)
            .Subscribe(c =>
            {
                collisionAudioSource.Play();    // 衝突の音を出す

                // 当たった星のサイズが
                if(c.transform.localScale.x <= (transform.localScale.x / 3))
                {
                    // 1. 自分よりも圧倒的に小さければそのまま吸収

                    // ボスを倒すとクリア
                    if (c.gameObject.GetComponent<_StarParam>().starID == 2)
                    {
                        GameManager.Instance.isClear.Value = true;
                    }
                    else
                    {
                        // 小さい星は成長速度が遅め
                        SetStarSize(c.transform.localScale.x / 4);
                        SetCamera();
                        c.gameObject.SetActive(false);
                    }
                }
                else if (c.transform.localScale.x <= (transform.localScale.x * 1.1f))
                {
                    // 2. 自分と同じくらいならばお互いを破壊して合体

                    // パーティクル再生
                    foreach (ParticleSystem ps in hitPS)
                    {
                        ps.Play();
                    }

                    // ボスを倒すとクリア
                    if (c.gameObject.GetComponent<_StarParam>().starID == 2)
                    {
                        GameManager.Instance.isClear.Value = true;
                    }
                    else
                    {
                        // ぶつかったら、砕けて待ち時間のカウントを進める
                        StartCoroutine(WaitCoroutine(waitCount, c.transform.localScale.x / 2));
                    }

                    // 相手のオブジェクトを非表示にする
                    c.gameObject.SetActive(false);
                }
                else
                {
                    // 3. 自分より大きければ自分が破壊される　ゲームオーバー

                    GameManager.Instance.isGameOver.Value = true;
                    this.gameObject.SetActive(false);
                }
            });
    }

    // マウスカーソル・移動処理
    void MoveCursor()
    {
        // マウスカーソルの座標を取得
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = CDISTANCE;
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        cursorParent.transform.position = cursorPos;

        // ゲームマネージャーに情報を渡す
        GameManager.Instance.cursorPosition = cursorPos;
        GameManager.Instance.cursorFlg = holeFlg;

        // 力を加える
        Vector3 moveDir = (cursorPos - transform.position).normalized;   // マウスカーソルへの方向を取得
        starRig.AddForce(moveSpeedMul * ((moveDir * moveSpeed) - starRig.velocity));
    }
    // カメラの処理
    void SetCamera()
    {
        // カメラ初期位置と星の半径を足した距離分、カメラを離す
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance
            = CDISTANCE + (transform.localScale.x / 1.5f);
    }

    // 衝突後の待ち時間を管理するコルーチン
    // 待機時間が終わるまでRigidbody.isLinematicをtrueにすることで動きを止める
    // observer  : 値を返すもの(ググってくれ)
    // waitCount : 待ち時間(単位：秒)
    // nextSize  : 待ち時間が終わった後に大きくする星のサイズ
    IEnumerator WaitCoroutine(float waitCount,float nextSize)
    {
        
        float count = 0.0f;                     // 待ち時間を計測する変数
        float size = transform.localScale.x;    // プレイヤーのサイズを保存する

        // ヒットストップを最初に起動する
        StartCoroutine(HitStopCoroutine(hitStopTime));

        // プレイヤーを停止、星のサイズを0に
        starRig.isKinematic = true;
        SetStarSize(-size);

        while (count < waitCount - 0.5f)
        {
            count += Time.deltaTime;

            yield return null;
        }

        // 待ち時間が終わる0.5秒前に、星のサイズを適用する
        SetStarSize(size + nextSize);

        while (count < waitCount)
        {
            // 待ち時間のカウントを進め、デバッグ用の値を渡す
            count += Time.deltaTime;

            yield return null;
        }

        // プレイヤーの停止を終了、カメラをセットする
        starRig.isKinematic = false;
        SetCamera();
    }

    // 衝突時のヒットストップを管理するコルーチン
    // コライダーの判定を取ったときに、一瞬スローになる演出
    // stopFrame
    IEnumerator HitStopCoroutine(float stopTime)
    {
        float count = 0.0f;

        Time.timeScale = 0.03f;

        while (count < stopTime)
        {
            count += Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
