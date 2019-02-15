using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Create MatTable",fileName ="MatTable")]
public class MatTable : ScriptableObject
{
    [SerializeField, Header("メインテクスチャ")]
    public Color color1 = new Color(1, 1, 1, 1);
    [SerializeField]
    public Texture2D tex1;

    [SerializeField, Header("サブテクスチャ")]
    public Color color2 = new Color(1, 1, 1, 1);
    [SerializeField]
    public Texture2D tex2;

    [SerializeField, Header("マスクテクスチャ,閾値(0~1)")]
    public Texture2D mask;
    [SerializeField]
    public float Threshold = 0.5f;

    [SerializeField, Header("スフィアの滑らかさ(0~1)")]
    public float Smoothness = 0.5f;
    [SerializeField, Header("スフィアの金属感(0~1)")]
    public float Metallic = 0.5f;

    [SerializeField, Header("リムライト")]
    public Color rimLight = new Color(0.5f, 0.5f, 0.5f, 1);
}
