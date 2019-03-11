using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class IsClear : MonoBehaviour
{
    float waitTime = 2.0f;

    void Start()
    {
        GameManager.Instance.isClear
            .Where(x => x)
            .Subscribe(_ =>
            {
                GameManager.Instance.isPause.Value = true;

                // ここにクリア時の処理を書いてね
                Debug.Log("IsClear : " + _.ToString());
                GameManager.Instance.bigText.text = "CLEAR!";

                StartCoroutine(changeScene(waitTime));

                GameManager.Instance.isClear.Value = false;
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
