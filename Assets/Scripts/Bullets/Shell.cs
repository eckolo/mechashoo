using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///Bulletの中でも特に弾丸っぽいやつ
/// </summary>
public class Shell : Bullet
{
    /// <summary>
    /// 減速度
    /// </summary>
    [SerializeField]
    public float Deceleration = 0.1f;
    /// <summary>
    /// 最終速度最大値
    /// </summary>
    [SerializeField]
    public float endSpeedMax = 1f;
    /// <summary>
    /// 最終速度最小値
    /// </summary>
    [SerializeField]
    public float endSpeedMin = 0f;
    /// <summary>
    /// 消滅までの猶予時間
    /// </summary>
    [SerializeField]
    public int delay = 0;

    /// <summary>
    ///通過後に発生する系のエフェクトが常に発生するか否か
    /// </summary>
    [SerializeField]
    protected bool alwaysLocus = true;
    /// <summary>
    ///通過後に発生する系のエフェクト
    /// </summary>
    [SerializeField]
    protected Locus locus = null;
    /// <summary>
    ///通過後に発生する系のエフェクトの発生間隔
    /// </summary>
    [SerializeField]
    protected int locusInterval = 1;
    /// <summary>
    ///通過後に発生する系のエフェクトのサイズ倍率
    /// </summary>
    [SerializeField]
    protected float locusScale = 1;
    /// <summary>
    ///通過後に発生する系のエフェクトの発生位置
    /// </summary>
    [SerializeField]
    protected Vector2 locusPosition = Vector2.zero;

    protected override IEnumerator Motion(int actionNum)
    {
        maxSpeed = nowSpeed.magnitude;
        for(int time = 0; nowSpeed.magnitude < endSpeedMin || endSpeedMax < nowSpeed.magnitude; time++)
        {
            Stopping(Deceleration);

            if(alwaysLocus) GenerateLocus(time);

            yield return Wait(1);
        }
        yield return Wait(delay);
        DestroyMyself();
        yield break;
    }
    protected Effect GenerateLocus(int time)
    {
        var setedLocus = locus;
        if(setedLocus == null) return null;
        if(time % Mathf.Max(locusInterval + 1, 1) > 0) return null;

        Vector2 _locusPosition = transform.rotation * locusPosition;
        float _locusScale = lossyScale.magnitude * locusScale / Vector2.one.magnitude;

        var result = OutbreakEffect(locus, _locusScale, _locusPosition);
        return result;
    }

    protected virtual float maxSpeed
    {
        get {
            return _maxSpeed;
        }
        private set {
            _maxSpeed = value;
        }
    }
    float _maxSpeed = 0;

    public override float nowPower
    {
        get {
            return base.nowPower * nowSpeed.magnitude / maxSpeed;
        }
    }
}
