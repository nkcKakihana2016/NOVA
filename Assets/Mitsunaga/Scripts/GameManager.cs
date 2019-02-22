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
        Start,          // スタート画面
        StageSelect,    // ステージセレクト画面
        Main,           // メインゲーム画面
        Result          // リザルト画面
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
        Debug.Log(playerTransform.position.x.ToString());

        if (Input.GetKeyDown(KeyCode.P))
        {
            NextState(nextGameState);
        }
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
                nextGameState = GameState.StageSelect;
                //SceneManager.LoadScene(0);
                break;
            case GameState.StageSelect:
                Debug.Log("ChangeState StageSelect");
                nextGameState = GameState.Main;
                //SceneManager.LoadScene(1);
                break;
            case GameState.Main:
                Debug.Log("ChangeState Main");
                nextGameState = GameState.Result;
                //SceneManager.LoadScene(2);
                break;
            case GameState.Result:
                Debug.Log("ChangeState Result");
                nextGameState = GameState.Start;
                //SceneManager.LoadScene(3);
                break;
        }
    }
}
