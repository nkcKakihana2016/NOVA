using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    private Text timerText;
    float timeCount;
    //　最初の時間
    private float startTime;
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
        int msecond = (int)(timeCount * 1000 % 100);


        //　マウスの左ボタン押しで一時停止
        if ()
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