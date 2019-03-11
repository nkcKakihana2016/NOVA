using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class IsGameOver : MonoBehaviour
{
    float waitTime = 2.0f;

    void Start()
    {
        GameManager.Instance.isGameOver
            .Where(x => x)
            .Subscribe(_ =>
            {
                GameManager.Instance.isPause.Value = true;

                // ここにゲームオーバー時の処理を書いてね
                Debug.Log("IsGameOver : " + _.ToString());
                GameManager.Instance.bigText.text = "GAME OVER";

                StartCoroutine(changeScene(waitTime));

                GameManager.Instance.isGameOver.Value = false;
            })
            .AddTo(gameObject);
    }

    IEnumerator changeScene(float wait)
    {
        float time = 0.0f;

        while (time < wait)
        {
            time += Time.deltaTime;

            yield return null;
        }
        GameManager.Instance.bigText.text = "";
        GameManager.Instance.NextState(GameManager.GameState.Title);
    }
}
