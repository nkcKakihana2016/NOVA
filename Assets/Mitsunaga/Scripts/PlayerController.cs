using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

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

    [SerializeField, Header("星の衝突時、合体時の待ち時間")]
    float hitStopCount = 0.03f;
    [SerializeField]
    float waitCount = 2;

    [SerializeField, Header("カメラのオブジェクト")]
    Transform cameraPos;        // 移動に追従するカメラ

    // 定数　このへんもっと分かりやすい変数名教えてくれ…
    const float MOVEDISTANCE = 10.0f;   // マウスの反応する距離　これ+星の直径
    const float CDISTANCE = 50.0f;      // マウスカーソルを置きたい奥行とカメラとの距離

    new void Awake()
    {
        base.Awake();

        // プレイヤー情報
        GameManager.Instance.playerTransform = this.gameObject.transform;
        playerRig = GetComponent<Rigidbody>();

        holeFlg = true;
        holes[1].SetActive(false);
        holes[0].SetActive(true);
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
                    GameManager.Instance.NextState(GameManager.GameState.Start);

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

                // カメラ処理
                // カメラ処理はCinemachineに一任することになりました！解散！
                //MoveCamera();
            });

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Where(x => x.gameObject.GetComponent<_StarParam>().starID != 1)
            .Subscribe(x =>
            {
                // 当たった星のサイズが自分と同じか小さければ破壊して合体
                if (x.transform.localScale.x <= transform.localScale.x)
                {
                    // コルーチンを回し、observer<>で戻り値を受け取ってSubscribe()に流す
                    Observable.FromCoroutine<float>(observer => WaitCoroutine(observer, waitCount))
                    .Subscribe(t => Debug.Log(t));

                    SetStarSize(x.gameObject.transform.localScale.x / 2);
                    Destroy(x.gameObject);
                }
                else
                {
                    GameManager.Instance.isGameOver.Value = true;
                    Destroy(this.gameObject);
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

    // カメラ処理(仮)
    void MoveCamera()
    {
        // 一旦線形補間使ってごまかし(うまくいってない！！！)
        float lerp = 0.1f;
        Vector3 movePos = new Vector3(0.0f, CDISTANCE, 0.0f);

        movePos.x = Mathf.Lerp(transform.position.x, cameraPos.position.x, lerp);
        movePos.z = Mathf.Lerp(transform.position.z, cameraPos.position.z, lerp);

        cameraPos.position = movePos;
    }

    // 衝突後の待ち時間を管理するコルーチン
    // 待機時間が終わるまでRigidbody.isLinematicをtrueにすることで動きを止める
    IEnumerator WaitCoroutine(System.IObserver<float> observer,float waitCount)
    {
        float count = 0.0f;

        StartCoroutine(HitStopCoroutine(hitStopCount));
        playerRig.isKinematic = true;

        while (count < waitCount)
        {
            observer.OnNext(count += Time.deltaTime);   // デバッグ用

            yield return null;
        }

        playerRig.isKinematic = false;
        observer.OnNext(waitCount);                     // デバッグ用
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
