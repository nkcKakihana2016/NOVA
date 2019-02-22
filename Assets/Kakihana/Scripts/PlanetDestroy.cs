using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetDestroy : MonoBehaviour
{
    
    [SerializeField] PlanetSpawner spawner; // 惑星が持つスクリプト 30秒経つと消える
    public Transform myTrans;               // 自身のトランスフォーム
    string sceneName;                       // スポーン時点でのシーン名
    float time;                             // スポーンしてからの経過時間

    // Start is called before the first frame update
    void Start()
    {
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
        time += Time.deltaTime;
        if (time > 30.0f)
        {
            // 30秒経つとにスポーンクラスに消滅情報を送る
            PlanetSpawner.Instance.PlanetDestroy();
            Destroy(this.gameObject);
        }
    }
}
