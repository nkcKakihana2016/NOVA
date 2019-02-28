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

    // プレイヤーのパラメータ関連
    Rigidbody playerRig;                // プレイヤーのRigidbody

    // 星の移動関連
    [SerializeField, Header("星の加速度、加速度への追従度、反応距離")]
    float moveSpeed = 10.0f;
    [SerializeField]
    float moveSpeedMul = 2.0f;

    // マウスカーソル関連
    [SerializeField, Header("マウスカーソルを追従するオブジェクト、その中身(0黒1白)")]
    Transform cursorParent;
    [SerializeField]
    GameObject[] holes;
    bool holeFlg;               // trueならブラックホール(0)、falseならホワイトホール(1)

    // 衝突関連
    [SerializeField, Header("星の衝突時、合体時の待ち時間")]
    float hitStopCount = 0.03f;
    [SerializeField]
    float waitCount = 4.5f;
    [SerializeField, Header("衝突時のパーティクル群(全部入れてね！)")]
    ParticleSystem[] hitPS;

    // カメラ関連
    [SerializeField, Header("シネマシーンのカメラ")]
    CinemachineVirtualCamera vcam;
    const float CDISTANCE = 100.0f;      // マウスカーソルを置きたい奥行とカメラとの距離
    float cursorPosZ;

    // 定数　このへんもっと分かりやすい変数名教えてくれ…
    const float MOVEDISTANCE = 20.0f;   // マウスの反応する距離　これ+星の直径

    new void Awake()
    {
        base.Awake();

        // プレイヤー情報
        // GameManager.Instance.playerTransform = this.gameObject.transform;
        playerRig = GetComponent<Rigidbody>();

        holeFlg = true;
        holes[1].SetActive(false);
        holes[0].SetActive(true);

        SetCamera();
    }

    void Start()
    {
        // 毎フレーム呼び出される
        this.FixedUpdateAsObservable()
            .Subscribe(_ => 
            {
            // マウスのクリック処理
                if (Input.GetMouseButtonDown(0))
                {
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
            });

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Where(x => x.gameObject.GetComponent<_StarParam>().starID != 1)
            .Subscribe(x =>
            {
                foreach(ParticleSystem ps in hitPS)
                {
                    ps.Play();
                }

                // 当たった星のサイズが
                // 自分と同じか小さければ星を破壊して合体
                // 自分より大きければ自分が破壊される
                if (x.transform.localScale.x <= transform.localScale.x)
                {
                    // コルーチンを回し、observer<>で戻り値を受け取ってSubscribe()に流す
                    Observable.FromCoroutine<float>(observer => WaitCoroutine(observer, waitCount,(x.transform.localScale.x/2)))
                    .Subscribe(t => Debug.Log(t),
                              () => SetCamera());

                    x.gameObject.SetActive(false);

                }
                else
                {
                    //GameManager.Instance.isGameOver.Value = true;
                    this.gameObject.SetActive(false);
                }
            });
    }

    // マウスカーソル・移動処理
    void MoveCursor()
    {
        Vector3 mouseScreen = Input.mousePosition;                      // マウスカーソルの座標を取得
        mouseScreen.z = CDISTANCE;                                      // 奥行を指定
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreen); // スクリーン座標系からワールド座標系に変換
        cursorParent.transform.position = mousePos;                     // マウスカーソルの座標にオブジェクトを同期

        // 反応距離より近ければ、AddForceを適用
        if (Vector3.Distance(mousePos, transform.position) <= MOVEDISTANCE + transform.localScale.x)
        {
            Vector3 moveDir = (mousePos - transform.position).normalized;   // マウスカーソルへの方向を取得
            playerRig.AddForce(moveSpeedMul * ((moveDir * moveSpeed) - playerRig.velocity));
        }
    }
    // カメラの処理
    void SetCamera()
    {
        cursorPosZ = CDISTANCE + (transform.localScale.x / 2);
        vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = cursorPosZ;
        Debug.Log("SetCamera Completed");
    }

    // 衝突後の待ち時間を管理するコルーチン
    // 待機時間が終わるまでRigidbody.isLinematicをtrueにすることで動きを止める
    IEnumerator WaitCoroutine(System.IObserver<float> observer,float waitCount,float nextSize)
    {
        
        float count = 0.0f;                     // 待ち時間を計測する変数
        float size = transform.localScale.x;// プレイヤーのサイズを保存する

        StartCoroutine(HitStopCoroutine(hitStopCount));
        playerRig.isKinematic = true;
        SetStarSize(-size);

        while (count < waitCount- 0.5f)
        {
            observer.OnNext(count += Time.deltaTime);   // デバッグ用

            yield return null;
        }

        SetStarSize(size + nextSize);

        while (count < waitCount)
        {
            observer.OnNext(count += Time.deltaTime);   // デバッグ用

            yield return null;
        }

        playerRig.isKinematic = false;

        observer.OnCompleted();                     // デバッグ用
    }

    // 衝突時のヒットストップを管理するコルーチン
    // コライダーの判定を取ったときに、一瞬スローになる演出
    IEnumerator HitStopCoroutine(float stopCount)
    {
        float count = 0.0f;

        Time.timeScale = 0.05f;
        while (count < stopCount)
        {
            count += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
