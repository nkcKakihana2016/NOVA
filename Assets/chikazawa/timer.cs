using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class timer : MonoBehaviour
{
    Text timerText;
    float timeCount;
    //　最初の時間
    float startTime;
    bool stopFLG;
    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        timeCount = 0;
    }

    void Update()
    {

        //　Time.timeでの時間計測
        timeCount += Time.deltaTime;//毎フレームの時間を加算.
        int minute = (int)timeCount / 60;//分.timeを60で割った値.
        int second = (int)timeCount % 60;//秒.timeを60で割った余り.
        int msecond = (int)(timeCount * 100 % 99);

        timerText.text = minute.ToString("00") + second.ToString("00") + msecond.ToString("00");
        //　マウスの左ボタン押しで一時停止
        if (Input.GetMouseButtonDown(0))
        {
            //boolで処理 Mathf.Approximately( 比較, 比較) ? true(一致) : false;
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;
            Debug.Log("set");
        }
        
    }
     void FixedUpdate()
    {
        
    }
}