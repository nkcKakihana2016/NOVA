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
                StartCoroutine(AICoroutine(i));

            }).AddTo(gameObject);
    }

    IEnumerator AICoroutine(int AInum)
    {
        while (this.gameObject.activeInHierarchy)
        {
            // AIの番号に基づいた行動を実行
            switch (AInum)
            {
                case 1:
                    transform.LookAt(GameManager.Instance.playerTransform);
                    break;
            }

            yield return null;
        }
    }
}
