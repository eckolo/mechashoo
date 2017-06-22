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
    protected float limitSize = 1;
    /// <summary>
    ///最大化までの所要時間
    /// </summary>
    protected int maxSizeTime = 10;
    protected float powerTweak = 1;
    /// <summary>
    ///最大化までの所要時間のデフォルト値
    /// </summary>
    [SerializeField]
    private int defaultMaxSizeTime = 10;

    /// <summary>
    ///パラメータのセット
    /// </summary>
    public void setParamate(float size, float _powerTweak, int? maxlim = null, int? destroylim = null)
    {
        limitSize = size;
        powerTweak = _powerTweak;
        maxSizeTime = maxlim ?? defaultMaxSizeTime;
        destroyLimit = destroylim ?? destroyLimit;
    }

    public override void Start()
    {
        base.Start();
        updateScale(0);
        updateAlpha(0);
    }

    protected override IEnumerator motion(int actionNum)
    {
        for(int time = 0; time < destroyLimit; time++)
        {
            updateScale(time);
            updateAlpha(time);
            yield return wait(1);
        }

        selfDestroy();
    }

    private void updateScale(int time)
    {
        var nowSizeX = time < maxSizeTime
            ? Easing.cubic.Out(limitSize, time, maxSizeTime)
            : limitSize;
        var nowSizeY = time < destroyLimit
            ? Easing.quadratic.Out(limitSize.log(10), time, destroyLimit)
            : limitSize.log(10);
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
    }
    public override float nowPower
    {
        get {
            return base.nowPower * limitSize.log(2) * nowAlpha * powerTweak;
        }
    }

    private void updateAlpha(int time)
    {
        nowAlpha = Easing.quintic.SubIn(1, time, destroyLimit);
    }
    protected override void addEffect(Hit effect)
    {
        effect.transform.rotation = transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);
        effect.transform.localScale *= 2;
    }

    public override Vector2 nowSpeed
    {
        get {
            return Vector2.zero;
        }
    }

    protected override float impactCorrection
    {
        get {
            return base.impactCorrection * limitSize;
        }
    }
}
