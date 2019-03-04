using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class IsClear : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.isClear
            .Where(x => x)
            .Subscribe(_ =>
            {
                // ここにクリア時の処理を書いてね
                Debug.Log("IsClear : " + _.ToString());
                GameManager.Instance.bigText.text = "CLEAR!";
            })
            .AddTo(gameObject);
    }
}
