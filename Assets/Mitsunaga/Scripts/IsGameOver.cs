using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class IsGameOver : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.IOisGameOver
            .Where(x => x)
            .Subscribe(_ =>
            {
                // ここにゲームオーバー時の処理を書いてね
                Debug.Log("IsGameOver : " + _.ToString());
            })
            .AddTo(gameObject);
    }
}
