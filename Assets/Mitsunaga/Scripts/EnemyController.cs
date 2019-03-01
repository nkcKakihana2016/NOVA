using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniRx;


public class EnemyController : _StarParam
{
    // エネミーのAIを管理するサブジェクト
    public Subject<int> enemyAISubject = new Subject<int>();

    void InitEnemy(float enemySize,int enemyAI)
    {
        // 星の初期サイズは基本「１」なので、設定したいサイズから「１」減らして適用
        SetStarSize(enemySize - 1.0f);

        // AIのナンバーを適用する
        enemyAISubject.OnNext(enemyAI);
    }
}
