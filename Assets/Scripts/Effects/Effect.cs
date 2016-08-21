using UnityEngine;
using System.Collections;

public class Effect : Material
{
    /// <summary>
    /// エフェクトの基準サイズ
    /// </summary>
    public float baseScale = 1;

    protected override void baseStart()
    {
        base.baseStart();

        transform.localScale = Vector3.one * baseScale;

        Action();
    }
}
