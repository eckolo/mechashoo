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
    ///タイマーの名前
    /// </summary>
    private static string timerName = "slash";
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
        timerName = timer.start(timerName);
        basePower = power;
        updateScale();
        updateAlpha();
    }

    public override void Update()
    {
        if (timer.get(timerName) > destroyLimit) selfDestroy();
        base.Update();
        updateScale();
        updateAlpha();

        setVerosity(
            transform.rotation * Vector2.right
            , timer.get(timerName) < maxSizeTime ? 32 / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit * limitSize / maxSizeTime : 0
            );

    }

    private void updateScale()
    {
        var nowSizeX = timer.get(timerName) < maxSizeTime
            ? easing.cubic.Out(limitSize, timer.get(timerName), maxSizeTime)
            : limitSize;
        var nowSizeY = timer.get(timerName) < destroyLimit
            ? easing.quadratic.Out(limitSize / 3, timer.get(timerName), destroyLimit)
            : limitSize / 3;
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
        power = basePower * nowSizeX;
    }

    private void updateAlpha()
    {
        var color = GetComponent<SpriteRenderer>().color;

        color.a = 1 - easing.quintic.In(1, timer.get(timerName), destroyLimit);
        GetComponent<SpriteRenderer>().color = color;
    }
    protected override void addEffect(Hit effect)
    {
        effect.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
        effect.transform.localScale *= 2;
    }
}
