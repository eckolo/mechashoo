using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///Bulletの中でも特に弾丸っぽいやつ
/// </summary>
public class Shell : Bullet
{
    /// <summary>
    /// 加速度行列
    /// </summary>
    [SerializeField]
    protected List<float> accelerationList = new List<float>();
    /// <summary>
    /// 加速度変更タイミング行列
    /// </summary>
    [SerializeField]
    protected List<int> accelerationTimeLimits = new List<int>();

    /// <summary>
    /// 減速度
    /// </summary>
    public float Deceleration = 0.1f;
    /// <summary>
    /// 最終速度最大値
    /// </summary>
    public float endSpeedMax = 1f;
    /// <summary>
    /// 最終速度最小値
    /// </summary>
    public float endSpeedMin = 0f;

    /// <summary>
    ///通過後に発生する系のエフェクト
    /// </summary>
    [SerializeField]
    protected Effect locus;
    /// <summary>
    ///通過後に発生する系のエフェクトの発生間隔
    /// </summary>
    [SerializeField]
    protected int locusInterval = 1;
    /// <summary>
    ///通過後に発生する系のエフェクトの発生位置
    /// </summary>
    [SerializeField]
    protected Vector2 locusPosition = Vector2.zero;

    /// <summary>
    ///弾丸が自動で移動方向を向く
    /// </summary>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        base.setVerosityAction(acceleration);
        setAngle(nowSpeed);
        if(initialScale != null) transform.localScale = MathV.scaling((initialScale ?? Vector2.zero), (1 + nowSpeed.magnitude / parPixel), (1 - nowSpeed.magnitude / parPixel));
    }
    protected override IEnumerator motion(int actionNum)
    {
        maxSpeed = nowSpeed.magnitude;
        for(int time = 0; nowSpeed.magnitude < endSpeedMin || endSpeedMax < nowSpeed.magnitude; time++)
        {
            stopping(Deceleration);

            generateLocus(time);

            yield return wait(1);
        }
        yield break;
    }
    private void generateLocus(int time)
    {
        var setedLocus = locus;
        if(setedLocus == null) return;
        if(time % Mathf.Max(locusInterval + 1, 1) > 0) return;

        Vector2 locusPositionLocal = transform.rotation * locusPosition;
        float locusScaleLocal = lossyScale.magnitude / Vector2.one.magnitude;

        outbreakEffect(locus, locusScaleLocal, locusPositionLocal);
    }

    protected float maxSpeed
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
