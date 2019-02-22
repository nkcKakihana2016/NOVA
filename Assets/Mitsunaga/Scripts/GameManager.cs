using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class GameManager : SingletonMonoBehaviourFast<GameManager>
{
    // シーン遷移やフラグ管理など、シーンをまたぐ情報の管理を行う

    // ゲームシーンのステート
    public enum GameState
    {
        Start,          // スタート画面
        StageSelect,    // ステージセレクト画面
        Stage01,           // メインゲーム画面
        Stage02          // リザルト画面
    }
    GameState gameState;        // 現在のステート
    GameState nextGameState;    // デバッグ用　次のステート

    // プレイヤーの情報
    public Transform playerTransform;

    // フラグ管理
    public BoolReactiveProperty isClear = new BoolReactiveProperty(false);       // クリア
    public BoolReactiveProperty isGameOver = new BoolReactiveProperty(false);    // ゲームオーバー
    public BoolReactiveProperty isPause = new BoolReactiveProperty(false);       // 一時停止

    // UniRxイベント
    Subject<bool> isClearSubject = new Subject<bool>();     // クリア
    public IObservable<bool> IOisClear { get { return isClearSubject; } }
    Subject<bool> isGameOverSubject = new Subject<bool>();  // ゲームオーバー
    public IObservable<bool> IOisGameOver { get { return isGameOverSubject; } }

    // 最初に呼び出されるシーンのタイミングでのみ処理される
    override protected void Awake()
    {
        // 親クラスのAwakeをはじめに呼び出す
        base.Awake();

        Debug.Log("GameManager Awake!");

        // シーンが変わっても破棄されないようにする
        DontDestroyOnLoad(this.gameObject);

        // クリア時に発行
        isClear.Subscribe(c =>
        {
            isClearSubject.OnNext(isClear.Value);
        })
        .AddTo(gameObject);
        // ゲームオーバー時に発行
        isGameOver.Subscribe(c =>
        {
            isGameOverSubject.OnNext(isGameOver.Value);
        })
        .AddTo(gameObject);

        // ステートが変更されたとき、それに応じた処理を実行する
        this.ObserveEveryValueChanged(c => c.gameState)
            .Subscribe(_ => ChangeState(gameState))
            .AddTo(gameObject);
    }

    // デバッグ用
    void Update()
    {
    }

    // ステートの変更
    // nextState … 次のシーンのステート(GameState)
    public void NextState(GameState nextState)
    {
        gameState = nextState;
    }

    // ステート変更時の処理　主にシーン遷移
    public void ChangeState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Start:
                Debug.Log("ChangeState Start");
                SceneManager.LoadScene(1);
                break;
            case GameState.StageSelect:
                Debug.Log("ChangeState StageSelect");
                SceneManager.LoadScene(2);
                break;
            case GameState.Stage01:
                Debug.Log("ChangeState Main");
                SceneManager.LoadScene(3);
                break;
            case GameState.Stage02:
                Debug.Log("ChangeState Result");
                SceneManager.LoadScene(4);
                break;
        }
    }
}
