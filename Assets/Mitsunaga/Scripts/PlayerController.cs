using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharaParameters
{
    new void Awake()
    {
        base.Awake();
    }
    new void Update()
    {
        base.Update();

        transform.eulerAngles += new Vector3(0.0f, 60.0f * Time.deltaTime, 0.0f);
    }
}
