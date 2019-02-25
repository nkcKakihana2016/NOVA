using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;

public class PlanetDestroy : MonoBehaviour
{
    
    [SerializeField] PlanetSpawner spawner; // 惑星が持つスクリプト 30秒経つと消える
    public Transform myTrans;               // 自身のトランスフォーム
    string sceneName;                       // スポーン時点でのシーン名
    float time;                             // スポーンしてからの経過時間

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
        //Observable.EveryUpdate().TakeUntilDestroy(this.gameObject).Subscribe();
        // スポーン時にシーン名を取得
        sceneName = PlanetSpawner.Instance.NowSceneName();
        // シーンが切り替わってもDestroyしない問題を解決
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間をカウントし格納
        //time += Time.deltaTime;
        //if (time > 30.0f)
        //{
        //    // 30秒経つとにスポーンクラスに消滅情報を送る
        //    PlanetSpawner.Instance.PlanetDestroy();
        //    Destroy(this.gameObject);
        //}
    }

    // 出現後、30秒経過したら非表示にする
    public IObservable<UniRx.Unit> PlanetSpawn(Vector3 pos)
    {
        transform.position = pos;

        return Observable.Timer(TimeSpan.FromSeconds(30.0f))
            .ForEachAsync(_ => { Stop(); }).TakeUntilDestroy(this.gameObject);
    }

    // 惑星非表示メソッド
    public void Stop()
    {
        // 30秒経つとにスポーンクラスに消滅情報を送る
        PlanetSpawner.Instance.PlanetDestroy();
        //Destroy(this.gameObject);
        // 下はオブジェクトプール用
        this.gameObject.SetActive(false);
    }
}
