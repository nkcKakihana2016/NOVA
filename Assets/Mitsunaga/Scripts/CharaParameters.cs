using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;

public class CharaParameters : MonoBehaviour
{
    // 星のサイズ
    FloatReactiveProperty starSize = new FloatReactiveProperty(0.0f);
    float nextSize = 0.0f; // 星のサイズを変化させる場合の目標

    int starID = 0;                      // ID　1ならプレイヤー,2ならボス,3以降はモブ
    [SerializeField] Color starColor1;   // 色1
    [SerializeField] Color starColor2;   // 色2
    [SerializeField] Texture2D starTex1; // テクスチャ1
    [SerializeField] Texture2D starTex2; // テクスチャ2
    [SerializeField] Texture2D starMask; // マスクテクスチャ
    [SerializeField] Color starLight;    // 光
    [SerializeField] float starSpeed;    // 移動速度(未実装)

    IEnumerator routine;

    public void Awake()
    {
        // コルーチンの再生、停止をコントロールするためにここで宣言
        routine = SetStarSizeCoroutine(nextSize);

        // starSizeの値が変化したら、transformに適用する
        starSize.Subscribe(c =>
        {
            transform.localScale = new Vector3(starSize.Value, starSize.Value, starSize.Value);
        }).AddTo(gameObject);

        starSize.Value = transform.localScale.x;

        // マテリアルの初期設定
        InitMaterial();
    }

    void Start()
    {
        
    }
    public void Update()
    {
        // デバッグ用　→ 大きくなる　← 小さくなる
        if (Input.GetKeyDown(KeyCode.RightArrow)) SetStarSize(1.0f);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SetStarSize(-1.0f);
    }

    // 星のサイズ設定
    // float size … 変化量
    public void SetStarSize(float size)
    {
        nextSize = starSize.Value + size;

        StopCoroutine(routine);
        routine = null;
        routine = SetStarSizeCoroutine(nextSize);
        StartCoroutine(routine);
    }
    // 星のサイズを変化させるコルーチン (大きくするときは滑らかに動くが、小さくするときは一瞬…)
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
        // マテリアルに色とライトを適用
        Material starMat = GetComponent<Renderer>().material;
        if (starColor1 != null) starMat.SetColor("_Color1", starColor1);
        if (starColor2 != null) starMat.SetColor("_Color2", starColor2);
        if (starTex1 != null) starMat.SetTexture("_MainTex", starTex1);
        if (starTex2 != null) starMat.SetTexture("_SubTex", starTex2);
        if (starMask != null) starMat.SetTexture("_MaskTex", starMask);
        if (starLight != null) starMat.SetColor("_RimColor", starLight);
    }
}
