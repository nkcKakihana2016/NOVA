using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
public class CharaParameters : MonoBehaviour
{
    [SerializeField, Header("星のID (使用用途については要相談)")]
    public int starID = 0;  // 自機やボスなどを特定できるようにするために使うかな？

    [SerializeField, Header("星のサイズ")]
    FloatReactiveProperty starSize = new FloatReactiveProperty(1.0f);

    [SerializeField,Header("マテリアル初期化用パラメータ")]
    MatTable matTable;

    IEnumerator routine;    // コルーチン用　コルーチンの管理
    float nextSize = 1.0f;  // コルーチン用　目標の星のサイズ

    public void Awake()
    {
        // コルーチンの再生、停止をコントロールするためにここで宣言
        routine = SetStarSizeCoroutine(nextSize);

        // starSizeの値が変化したら、transformに適用する
        starSize.Subscribe(c =>
        {
            transform.localScale = new Vector3(starSize.Value, starSize.Value, starSize.Value);
        }).AddTo(gameObject);

        // 初期設定
        InitMaterial();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
    }

    // 星のサイズ設定
    // float size … 変化量
    public void SetStarSize(float size)
    {
        // コルーチンがうまく止められないのでいったんNULLにしてから設定しなおす、という…
        nextSize = starSize.Value + size;

        StopCoroutine(routine);
        routine = null;
        routine = SetStarSizeCoroutine(nextSize);
        StartCoroutine(routine);
    }
    // 星のサイズを変化させるコルーチン (大きくするとき用です)
    // size … 目指す星のサイズ
    IEnumerator SetStarSizeCoroutine(float size)
    {
        // 変化開始時の大きさが、目指す大きさくらいになるまで継続
        while(size >= starSize.Value)
        {
            starSize.Value = Mathf.Lerp(starSize.Value, size, 0.05f);

            yield return null;
        }
        starSize.Value = size;
    }

    public void InitMaterial()
    {
        // マテリアルのいろいろな初期化
        Material starMat = GetComponent<Renderer>().material;
        starMat.SetColor    ("_Color1", matTable.color1);
        starMat.SetColor    ("_Color2", matTable.color2);
        starMat.SetTexture  ("_MainTex", matTable.tex1);
        starMat.SetTexture  ("_SubTex", matTable.tex2);
        starMat.SetTexture  ("_MaskTex", matTable.mask);
        starMat.SetColor    ("_RimColor", matTable.rimLight);
    }
}
