using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class TitleManager : MonoBehaviour
{
    // スタート画面でのボタン
    public void OnTitleButtonClicked()
    {
        GetComponent<AudioSource>().Play();
        GameManager.Instance.NextState(GameManager.GameState.StageSelect);
    }
}
