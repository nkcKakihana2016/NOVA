using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        
    }
    // スタート画面でのボタン
    public void OnTitleButtonClicked()
    {
        GameManager.Instance.NextState(GameManager.GameState.StageSelect);
    }
}
