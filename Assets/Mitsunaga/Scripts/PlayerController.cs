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

        // 最初はブラックホール
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
            .Where(c => Input.GetMouseButtonDown(0))            // マウスがクリックされた場合
            .Where(c => GameManager.Instance.isPause.Value)     // ポーズ中        である
            .Where(c => !GameManager.Instance.isGameOver.Value) // ゲームオーバー　ではない
            .Where(c => !GameManager.Instance.isClear.Value)    // ゲームクリア    ではない
            .Subscribe(_ =>                                     // 場合のみ処理を実行
            {
                GameManager.Instance.isPause.Value = false;
            })
            .AddTo(this.gameObject);

        // 毎フレーム呼び出される
        this.UpdateAsObservable()
            .Where(c => !GameManager.Instance.isPause.Value)    // ポーズ中ではない
            .Subscribe(_ =>                                     // 場合のみ処理を実行
            {
            // プレイヤー情報をGameManagerに入力
                GameManager.Instance.playerTransform = this.gameObject.transform;

            // マウスのクリック処理
                if (Input.GetMouseButtonDown(0))
                {
                    // 移動速度を反転させる
                    moveSpeed = -moveSpeed;

                    // マウスカーソルの表示切替
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

        Observable.Interval(TimeSpan.FromSeconds(1.0))
            .Subscribe(c =>
            {
                SetCamera();
            }).AddTo(this.gameObject);

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Subscribe(c =>
            {
                float enemySize = -1.0f;

                try
                {
                    enemySize = c.gameObject.GetComponent<_StarParam>().GetStarSize();

                    collisionAudioSource.Play();    // 衝突の音を出す

                    // 当たった星のサイズが
                    if (enemySize <= (GetStarSize() / 4))
                    {
                        // 1. 自分よりも圧倒的に小さければそのまま吸収

                        // ボスを倒すとゲームクリア
                        if (c.gameObject.GetComponent<_StarParam>().starID == 2)
                        {
                            GameManager.Instance.isClear.Value = true;
                        }
                        else
                        {
                            // 小さい星では成長しない
                            c.gameObject.SetActive(false);
                        }
                    }
                    else if (c.transform.localScale.x <= (GetStarSize() * 1.1f))
                    {
                        // 2. 自分と同じくらいならばお互いを破壊して合体

                        // パーティクル再生
                        foreach (ParticleSystem ps in hitPS)
                        {
                            ps.Play();
                        }

                        // ボスを倒すとゲームクリア
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
                }
                catch
                {
                    Debug.Log("_StarParam is Null");
                }

                if(enemySize != -1.0f)
                {

                }
            }).AddTo(this.gameObject);
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

    // 衝突後の待ち時間、星の再構成を管理するコルーチン
    // waitCount：待ち時間(単位：秒)
    // nextSize ：待ち時間が終わった後に大きくする星のサイズ
    IEnumerator WaitCoroutine(float waitCount,float nextSize)
    {
        
        float count = 0.0f;                     // 待ち時間を計測する変数
        float size = transform.localScale.x;    // プレイヤーのサイズを保存する

        // ヒットストップを最初に起動
        StartCoroutine(HitStopCoroutine(hitStopTime));

        starRig.isKinematic = true; // プレイヤーを移動不能に
        SetStarSize(0.0f);          // 星のサイズを0に

        // 待ち時間のカウント
        while (count < waitCount - 0.5f)
        {
            count += Time.deltaTime;
            yield return null;
        }

        // 待ち時間が終わる0.5秒前に、星のサイズを適用
        SetStarSize(size + nextSize);

        // 待ち時間のカウント
        while (count < waitCount)
        {
            count += Time.deltaTime;
            yield return null;
        }

        starRig.isKinematic = false;    // プレイヤーを移動可能に
        SetCamera();                    // カメラをセットする
    }

    // 衝突時のヒットストップを管理するコルーチン
    // stopTime：待ち時間(単位：秒)
    IEnumerator HitStopCoroutine(float stopTime)
    {
        float count = 0.0f;

        // Time.TimeScale … 時間の進む速さを変更する(通常 1.0f)
        Time.timeScale = 0.05f;

        // 待ち時間のカウント
        while (count < stopTime)
        {
            // Time.unscaledDeltaTime … タイムスケールの影響を受けないDeltaTime
            count += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
