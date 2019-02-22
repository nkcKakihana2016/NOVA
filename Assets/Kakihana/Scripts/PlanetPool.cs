using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;

[System.Serializable]
public class PlanetPool : ObjectPool<PlanetDestroy>
{
    private readonly PlanetDestroy planetObj;
    private Transform myTrans;

    public PlanetPool(Transform trans,PlanetDestroy planetPre)
    {
        myTrans = trans;
        planetObj = planetPre;
    }

    protected override PlanetDestroy CreateInstance()
    {
        var e = GameObject.Instantiate(planetObj);
        e.transform.SetParent(myTrans);

        return e;
    }
}
