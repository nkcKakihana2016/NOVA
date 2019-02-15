using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

public class PlayerController : CharaParameters
{
    // プレイヤーとカメラのコントロールを行う

    // プレイヤーのパラメータ関連
    Rigidbody playerRig;        // プレイヤーのRigidbody

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
    bool holeFlg;    // trueならブラックホール(0)、falseならホワイトホール(1)

    [SerializeField, Header("カメラのオブジェクト")]
    Transform cameraPos;        // 移動に追従するカメラ  
    
    float cameraDist = 50.0f;   // マウスカーソルを置きたい奥行とカメラとの距離
    Vector3 mousePos;           // 取得したマウスカーソルの座標

    new void Awake()
    {
        base.Awake();

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

            // マウスのカーソル・移動処理
                Vector3 mouseScreen = Input.mousePosition;                      // マウスカーソルの座標を取得
                mouseScreen.z = cameraDist;                                     // 奥行を指定
                mousePos = Camera.main.ScreenToWorldPoint(mouseScreen);         // スクリーン座標系からワールド座標系に変換
                cursorParent.transform.position = mousePos;                     // マウスカーソルの座標にオブジェクトを同期
                Vector3 movePos = (mousePos - transform.position).normalized;   // マウスカーソルへの方向を取得
                // 反応距離より近ければ、AddForceを適用
                if (Vector3.Distance(mousePos, transform.position) <= transform.localScale.x)
                {
                    playerRig.AddForce(moveSpeedMul * ((movePos * moveSpeed) - playerRig.velocity));
                }

                // カメラの処理(仮)
                cameraPos.position = new Vector3(transform.position.x, cameraPos.position.y, transform.position.z);
            }
            );

        // 当たり判定
        this.OnCollisionEnterAsObservable()
            .Where(x => x.gameObject.GetComponent<CharaParameters>().starID != 1)
            .Subscribe(x =>
            {
                // コルーチンを回し、observer<>で戻り値を受け取ってSubscribe()に流す
                Observable.FromCoroutine<float>(observer => WaitCoroutine(observer, 3))
            .       Subscribe(t => Debug.Log(t));

                SetStarSize(x.gameObject.transform.localScale.x / 2);
                Destroy(x.gameObject);
            });
    }

    // 衝突後の待ち時間を管理するコルーチン
    // 待機時間が終わるまでRigidbody.isLinematicをtrueにすることで動きを止める
    IEnumerator WaitCoroutine(System.IObserver<float> observer,float waitCount)
    {
        float count = 0.0f;

        while (count < waitCount)
        {
            playerRig.isKinematic = true;
            observer.OnNext(count += Time.deltaTime);

            yield return null;
        }

        playerRig.isKinematic = false;
        observer.OnNext(waitCount);
    }
}
