using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EnemyAI : MonoBehaviour
{
    void Start()
    {
        // AIのナンバーを受け取り、AIのコルーチンを開始する
        GetComponent<EnemyController>().enemyAISubject
            .Subscribe(i =>
            {
                Debug.Log("Enemy!I : " + i.ToString());

            }).AddTo(gameObject);
    }
}
