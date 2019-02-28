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
        GameManager.Instance.NextState(GameManager.GameState.Main);
    }
    public void OnTitleButtonClicked()
    {
        GameManager.Instance.NextState(GameManager.GameState.Title);
    }
}
