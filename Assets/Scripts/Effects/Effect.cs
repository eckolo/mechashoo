using UnityEngine;
using System.Collections;

public class Effect : Material
{
    /// <summary>
    /// エフェクトの基準サイズ
    /// </summary>
    public float baseScale = 1;

    public override void Start()
    {
        base.Start();

        transform.localScale = Vector3.one * baseScale;

        Action();
    }
}
