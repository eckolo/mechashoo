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
    ///威力基準値
    /// </summary>
    private float nowPower;

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
        updateScale(0);
        updateAlpha(0);
    }

    protected override IEnumerator Motion(int actionNum)
    {
        for (int time = 0; time < destroyLimit; time++)
        {
            updateScale(time);
            updateAlpha(time);

            var innnerSpeed = time < maxSizeTime ? 16 * limitSize / maxSizeTime : 0;
            setVerosity(transform.rotation * Vector2.right, innnerSpeed);

            yield return null;
        }

        selfDestroy();
    }

    private void updateScale(int time)
    {
        var nowSizeX = time < maxSizeTime
            ? easing.cubic.Out(limitSize, time, maxSizeTime)
            : limitSize;
        var nowSizeY = time < destroyLimit
            ? easing.quadratic.Out(limitSize / 3, time, destroyLimit)
            : limitSize / 3;
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
        nowPower = base.getPower() * nowSizeX;
    }
    public override float getPower()
    {
        return nowPower;
    }

    private void updateAlpha(int time)
    {
        setAlpha(easing.quintic.SubIn(1, time, destroyLimit));
    }
    protected override void addEffect(Hit effect)
    {
        effect.transform.rotation = transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);
        effect.transform.localScale *= 2;
    }
}
