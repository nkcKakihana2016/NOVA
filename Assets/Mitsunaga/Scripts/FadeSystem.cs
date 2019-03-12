﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FadeSystem : MonoBehaviour
{
    // しきい値
    [SerializeField]
    FloatReactiveProperty cutoutRange = new FloatReactiveProperty(0.0f);

    [SerializeField]
    private Material uiMask;
    [SerializeField]
    private RenderTexture maskTexture; 
    [SerializeField]
    private Texture texture;
    [SerializeField]
    private Mask mask;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float startRange = 0.0f;

    private void Start()
    {
        cutoutRange.Subscribe(c =>
        {
            // アルファ値をマテリアルに適用する
            uiMask.SetFloat("_Threshold", c);

            // マスクを更新する
            mask.enabled = false;
            mask.enabled = true;
        }).AddTo(gameObject);

        cutoutRange.Value = startRange;
    }

    // フェードアウト<出現>
    public IEnumerator FadeOutCoroutine(System.IObserver<bool> observer, float time)
    {
        // 最初にあたり判定を出し、誤クリックを減らす
        // canvasGroup.blocksRaycasts = true;

        float endTime = Time.timeSinceLevelLoad +　time * (cutoutRange.Value);

        while (Time.timeSinceLevelLoad <= endTime)
        {
            // しきい値を変更し、フレーム終了を待つ
            cutoutRange.Value = (endTime - Time.timeSinceLevelLoad) / time;
            yield return null;
        }
        cutoutRange.Value = 0.0f;

        observer.OnNext(true);
    }

    //  フェードイン<消滅>
    public IEnumerator FadeInCoroutine(float time)
    {
        float endTime = time * (1.0f - cutoutRange.Value);
        float thisTime = 0.0f;

        while (thisTime <= endTime)
        {
            thisTime += Time.deltaTime;

            // しきい値を変更し、フレーム終了を待つ
            cutoutRange.Value = 1.0f - ((endTime - thisTime) / time);
            yield return null;
        }
        cutoutRange.Value = 1.0f;

        // 最後にあたり判定を消す
        canvasGroup.blocksRaycasts = false;
    }
}
