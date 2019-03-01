﻿using System;
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
        Title,          // スタート画面
        StageSelect,    // ステージセレクト画面
        Main,           // メインゲーム画面
    }
    GameState gameState;        // 現在のステート
    GameState nextGameState;    // デバッグ用　次のステート

    [SerializeField, Header("シーン遷移")]
    FadeSystem fadeSystem;
    [SerializeField]
    float fadeTime;


    [Header("ここから下は確認用")]
    // プレイヤーの情報
    public Transform playerTransform;
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
        this.ObserveEveryValueChanged(c => c.gameState)
            .Subscribe(_ => ChangeState(gameState))
            .AddTo(gameObject);
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
            Debug.Log("FadeOut!");
            SceneManager.LoadScene(sceneNumber);
            FadeIn();
        });
    }
    void FadeIn()
    {
        IObservable<bool> obsIn = Observable.FromCoroutine<bool>(observer => fadeSystem.FadeInCoroutine(observer, fadeTime));
        obsIn.Subscribe(onCompleted => 
        {
            Debug.Log("FadeIn!");
            isPause.Value = false;
        });
    }
}
