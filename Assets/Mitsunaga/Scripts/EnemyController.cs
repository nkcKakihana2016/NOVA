using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class EnemyController : _StarParam
{
    // エネミーのAIを管理するサブジェクト
    Subject<int> enemyAISubject = new Subject<int>();
    public IObservable<int> IOEnemyAI { get{ return enemyAISubject; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
