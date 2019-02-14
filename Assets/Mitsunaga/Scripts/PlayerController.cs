using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharaParameters
{
    // プレイヤーのコントロールを行うスクリプト

    [SerializeField, Header("マウスカーソル用のゲームオブジェクト")]
    GameObject cursorObj;

    new void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) SetStarSize(1.0f);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SetStarSize(-1.0f);

        Vector3 mousePos = Input.mousePosition;
        Debug.Log(mousePos.x.ToString());
        Debug.Log(mousePos.y.ToString());
        Debug.Log(mousePos.z.ToString());

        mousePos.z = 30.0f;
        Vector3 mouseStW = Camera.main.ScreenToWorldPoint(mousePos);

        Debug.Log(mouseStW.x.ToString());
        Debug.Log(mouseStW.y.ToString());
        Debug.Log(mouseStW.z.ToString());

        cursorObj.transform.position = mouseStW;
    }
}
