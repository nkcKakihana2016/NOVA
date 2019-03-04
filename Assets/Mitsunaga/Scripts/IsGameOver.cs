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
        GameManager.Instance.isGameOver
            .Where(x => x)
            .Subscribe(_ =>
            {
                // ここにゲームオーバー時の処理を書いてね
                Debug.Log("IsGameOver : " + _.ToString());
                GameManager.Instance.bigText.text = "GAME OVER...";
            })
            .AddTo(gameObject);
    }
}
