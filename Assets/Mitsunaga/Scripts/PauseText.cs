using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PauseText : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.isPause
            .Subscribe(pause =>
            {
                if (pause &&
                !GameManager.Instance.isGameOver.Value &&
                !GameManager.Instance.isClear.Value)
                {
                    this.gameObject.SetActive(true);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
            })
            .AddTo(this.gameObject);
    }
}
