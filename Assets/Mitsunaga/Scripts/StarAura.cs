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
    GameObject parentObject;    // 親星
    _StarParam parentParam;     // 親星のパラメータ
    [Header(" ")]
    [SerializeField]
    float auraRange = 4;        // 親星からの距離
    [SerializeField]
    int auraHP = 3;             // オーラの耐久力(回数)

    VisualEffect auraEffect;    // オーラエフェクト

    void Start()
    {
        int maxHP = auraHP;

        auraEffect = this.GetComponent<VisualEffect>();
        parentParam = parentObject.GetComponent<_StarParam>();

        this.UpdateAsObservable()
            .Subscribe(c =>
            {
                this.transform.position = parentObject.transform.position;

                auraEffect.SetVector3("CenterPosition", this.transform.position);
                auraEffect.SetFloat("CircleSize", parentParam.GetStarSize() + auraRange);
            }).AddTo(this.gameObject);

        this.OnTriggerEnterAsObservable()
            .Where(c => auraHP > 0)
            .Subscribe(c =>
            {
                try {
                    _StarParam enemyParam = c.GetComponent<_StarParam>();

                    if (enemyParam != null&& enemyParam.starID != 1)
                    {
                        --auraHP;
                        auraEffect.SetFloat("Alpha", auraHP / maxHP);

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
