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

    [SerializeField, Header("デバッグ用、シーン移動処理を消すフラグ、でかいテキスト")]
    bool isDebug = false;
    [SerializeField]
    public Text bigText;
    // ゲームシーンのステート
    public enum GameState
    {
        Title,          // スタート画面
        StageSelect,    // ステージセレクト画面
        Main,           // メインゲーム画面
    }
    public GameState gameState;        // 現在のステート
    GameState nextGameState;    // デバッグ用　次のステート

    [SerializeField, Header("シーン遷移")]
    FadeSystem fadeSystem;
    [SerializeField]
    float fadeTime;

    [Header("ここから下は確認用")]
    // プレイヤーの情報
    public Transform playerTransform;   // プレイヤーのトランスフォーム
    public Vector3 cursorPosition;      // カーソルの位置
    public bool cursorFlg;              // カーソルのフラグ(0ならブラックホール、1ならホワイトホール)
    // フラグ管理
    public BoolReactiveProperty isClear = new BoolReactiveProperty(false);       // クリア
    public BoolReactiveProperty isGameOver = new BoolReactiveProperty(false);    // ゲームオーバー
    public BoolReactiveProperty isPause = new BoolReactiveProperty(false);       // 一時停止

    // 最初に呼び出されるシーンのタイミングでのみ処理される
    override protected void Awake()
    {
        // 親クラスのAwakeをはじめに呼び出す
        base.Awake();

        // シーンが変わっても破棄されないようにする
        DontDestroyOnLoad(this.gameObject);

        // ステートが変更されたとき、それに応じた処理を実行する
        // デバッグ中はシーンの切り替えを停止
        if (!isDebug)
        {
            this.ObserveEveryValueChanged(c => c.gameState)
            .Subscribe(_ => ChangeState(gameState))
            .AddTo(gameObject);
        }
        else
        {
            FadeIn();
        }
    }
    // ステートの変更
    // nextState … 次のシーンのステート(GameState)
    public void NextState(GameState state)
    {
        gameState = state;
    }

    // ステート変更時の処理　主にシーン遷移
    public void ChangeState(GameState state)
    {
        switch (state)
        {
            case GameState.Title:
                Debug.Log("ChangeState Title");
                FadeOut(1);
                break;
            case GameState.StageSelect:
                Debug.Log("ChangeState StageSelect");
                FadeOut(2);
                break;
            case GameState.Main:
                Debug.Log("ChangeState Main");
                FadeOut(3);
                break;
        }
    }

    // シーン遷移
    // フェードアウト後にシーンを切り替え、フェードインする(ポーズにしておく)
    void FadeOut(int sceneNumber)
    {
        isPause.Value = true;
        IObservable<bool> obsOut = Observable.FromCoroutine<bool>(observer => fadeSystem.FadeOutCoroutine(observer, fadeTime));
        obsOut.Subscribe(onCompleted =>
        {
            SceneManager.LoadScene(sceneNumber);
            FadeIn();
        })
        .AddTo(this.gameObject);
    }
    void FadeIn()
    {
        StartCoroutine(
        fadeSystem.FadeInCoroutine(fadeTime));
    }
}
