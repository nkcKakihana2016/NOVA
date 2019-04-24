using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor.VFX;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Experimental.VFX;

using UniRx;
using UniRx.Triggers;

public class StarAura : MonoBehaviour
{
    // オーラのパラメータ
    [SerializeField,Header("親のオブジェクト")]
    GameObject parentObject;        // 親星
    _StarParam parentParam;         // 親星のパラメータ

    [SerializeField]
    float auraRange = 4;            // 親星からの距離
    [SerializeField]
    int auraMaxHP = 3;              // オーラの耐久力(回数)
    [SerializeField]
    float auraRecoverCount = 2.0f;  // オーラが自動回復する時間

    VisualEffect auraEffect;    // オーラエフェクト

    IntReactiveProperty auraHP = new IntReactiveProperty(0);

    void Start()
    {
        // エフェクトと親オブジェクトのコンポーネントを取得する
        auraEffect = this.GetComponent<VisualEffect>();
        parentParam = parentObject.GetComponent<_StarParam>();

        // オーラの耐久度が変更されたらパーティクルのアルファ値に適用
        auraHP.Subscribe(i =>
        {
            auraEffect.SetFloat("Alpha", auraHP.Value / auraMaxHP);
        })
        .AddTo(this.gameObject);

        auraHP.Value = auraMaxHP;

        float auraCount = 0.0f;     // オーラの回復カウント用

        this.UpdateAsObservable()
            .Subscribe(c =>
            {
                // オーラが最大値よりも低い場合、一定時間ごとにオーラを回復する
                if(auraHP.Value < auraMaxHP)
                {
                    auraCount += Time.deltaTime;

                    if(auraCount >= auraRecoverCount)
                    {
                        auraCount = 0.0f;
                        ++auraHP.Value;
                    }
                }
                else
                {
                    auraCount = 0.0f;
                }

                // 親オブジェクトに追従させる
                this.transform.position = parentObject.transform.position;
                auraEffect.SetVector3("CenterPosition", this.transform.position);
                // 親オブジェクトの大きさを反映させる
                auraEffect.SetFloat("CircleSize", parentParam.GetStarSize() + auraRange);
            })
            .AddTo(this.gameObject);

        this.OnTriggerEnterAsObservable()
            .Where(c => auraHP.Value > 0)
            .Subscribe(c =>
            {
                try {
                    _StarParam enemyParam = c.GetComponent<_StarParam>();

                    if (enemyParam != null&& enemyParam.starID != 1)
                    {
                        --auraHP.Value;

                        // 当たった星のサイズが親星よりも大幅に小さければ
                        if (enemyParam.GetStarSize() <= (parentParam.GetStarSize() / 4))
                        {
                            // 当たった星を破壊する
                            c.gameObject.SetActive(false);
                        }
                    }
                }
                catch
                {
                    Debug.Log("_StarParam is Null");
                }
            }).AddTo(this.gameObject);
    }
}
