using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour
{
    // StageSelect画面の管理、GameManagerのお手伝い

    // ステージセレクト画面でのボタン
    // 後から、ぶつかったときの判定で移動するようにしようかな
    public void OnStageSelectButtonClicked()
    {
        GetComponent<AudioSource>().Play();
        GameManager.Instance.NextState(GameManager.GameState.Main);
    }
    public void OnTitleButtonClicked()
    {
        GetComponent<AudioSource>().Play();
        GameManager.Instance.NextState(GameManager.GameState.Title);
    }
}
