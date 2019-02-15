using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

public class HolePool : ObjectPool<Transform>
{
    public GameObject holePrefab;

    protected override Transform CreateInstance()
    {
        //新しく生成
        return GameObject.Instantiate(holePrefab).GetComponent<Transform>();
    }
}
