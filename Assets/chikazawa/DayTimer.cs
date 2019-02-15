using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimer : MonoBehaviour
{
    Text timerText;
    float timeCount;
    //　最初の時間
    float startTime;
    int week;
    int month;
    int year;
    bool stopFLG;

    void Start()
    {
        timerText = GetComponentInChildren<Text>();
        timeCount = 0;
        year = 2019;
    }

    void Update()
    {
        timeCount += Time.deltaTime;//毎フレームの時間を加算.
        DayCount(timeCount);

        timerText.text = year.ToString("0000") +"年 / "+ month.ToString("00") +"月 / 第" + week.ToString("0") + "週";


        //　マウスの左ボタン押しで一時停止
        if (Input.GetMouseButtonDown(0))
        {
            //boolで処理 Mathf.Approximately( 比較, 比較) ? true(一致) : false;
            Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;
            Debug.Log("set");
        }

    }
    void DayCount(float time)
    {
        bool countUp = false;
        week = (int)( 1 + (time % 4));
        if (week == 1 && countUp)
        {
            countUp = !countUp;

        }
        if (countUp)
        {
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

    }
}
//timerText.text = minute.ToString("00") + second.ToString("00") + msecond.ToString("00");

