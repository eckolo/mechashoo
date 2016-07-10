using UnityEngine;
using System.Collections;

/// <summary>
/// 斬撃系弾丸基本クラス
/// </summary>
public class Slash : Bullet
{
    /// <summary>
    ///最終的なサイズ
    /// </summary>
    [SerializeField]
    private float limitSize = 1;
    /// <summary>
    ///最大化までの所要時間
    /// </summary>
    [SerializeField]
    private int maxSizeTime = 10;
    /// <summary>
    ///残存時間
    /// </summary>
    [SerializeField]
    private int destroyLimit = 20;
    /// <summary>
    ///タイマーの名前
    /// </summary>
    private static string counteName = "slashcount";
    /// <summary>
    ///威力基準値
    /// </summary>
    protected float basePower;

    /// <summary>
    ///パラメータのセット
    /// </summary>
    public void setParamate(float? size = null, int? maxlim = null, int? destroylim = null)
    {
        limitSize = size ?? limitSize;
        maxSizeTime = maxlim ?? maxSizeTime;
        destroyLimit = destroylim ?? destroyLimit;
    }

    public override void Start()
    {
        base.Start();
        counterList.Add(counteName, 0);
        basePower = power;
        updateScale();
        updateAlpha();
    }

    public override void Update()
    {
        if (counterList[counteName] > destroyLimit) selfDestroy();
        base.Update();
        updateScale();
        updateAlpha();

        setVerosity(
            transform.rotation * Vector2.right
            , counterList[counteName] < maxSizeTime ? 32 / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit * limitSize / maxSizeTime : 0
            );

    }

    private void updateScale()
    {
        var nowSizeX = counterList[counteName] < maxSizeTime
            ? easing.cubic.Out(limitSize, counterList[counteName], maxSizeTime)
            : limitSize;
        var nowSizeY = counterList[counteName] < destroyLimit
            ? easing.quadratic.Out(limitSize / 3, counterList[counteName], destroyLimit)
            : limitSize / 3;
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
        power = basePower * nowSizeX;
    }

    private void updateAlpha()
    {
        var color = GetComponent<SpriteRenderer>().color;

        color.a = 1 - easing.quintic.In(1, counterList[counteName], destroyLimit);
        GetComponent<SpriteRenderer>().color = color;
    }
}
