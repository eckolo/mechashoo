using UnityEngine;
using System.Collections;

/// <summary>
/// 斬撃系弾丸基本クラス
/// </summary>
public class Slash : Bullet
{
    /// <summary>
    /// 最大化までの所要時間のデフォルト値
    /// </summary>
    [SerializeField]
    private int defaultMaxSizeTime = 10;
    [SerializeField]
    private float widthTweak = 10;
    [SerializeField]
    private AudioClip injectSE = null;

    /// <summary>
    /// 最終的なサイズ
    /// </summary>
    protected float limitSize = 1;
    /// <summary>
    /// 最大化までの所要時間
    /// </summary>
    protected int maxSizeTime = 10;
    protected float powerTweak = 1;

    /// <summary>
    /// パラメータのセット
    /// </summary>
    public void SetParamate(float size, float _powerTweak, int? maxlim = null, int? destroylim = null)
    {
        limitSize = size;
        powerTweak = _powerTweak;
        maxSizeTime = maxlim ?? defaultMaxSizeTime;
        destroyLimit = destroylim ?? destroyLimit;
    }

    public override void Start()
    {
        base.Start();
        UpdateScale(0);
        UpdateAlpha(0);
    }

    protected override IEnumerator Motion(int actionNum)
    {
        SoundSE(injectSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            UpdateScale(time);
            UpdateAlpha(time);
            yield return Wait(1);
        }

        DestroyMyself();
    }

    private void UpdateScale(int time)
    {
        var nowSizeX = time < maxSizeTime ?
            Easing.cubic.Out(limitSize, time, maxSizeTime) :
            limitSize;
        var width = limitSize.Log(widthTweak);
        var nowSizeY = time < destroyLimit ?
            Easing.quadratic.Out(width, time, destroyLimit) :
            width;
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
    }
    public override float nowPower
    {
        get {
            return base.nowPower * limitSize.Log(2) * nowAlpha * powerTweak;
        }
    }

    private void UpdateAlpha(int time)
    {
        nowAlpha = Easing.quintic.SubIn(1, time, destroyLimit);
    }
    protected override void AddEffect(Hit effect)
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
