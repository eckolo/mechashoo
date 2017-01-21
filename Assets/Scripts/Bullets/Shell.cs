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
        transform.localScale = new Vector2(initialScale.x * (1 + nowSpeed.magnitude / parPixel), initialScale.y * (1 - nowSpeed.magnitude / parPixel));
    }
    protected override IEnumerator motion(int actionNum)
    {
        for(int motionStage = 0; motionStage < accelerationList.Count; motionStage++)
        {
            if(motionStage >= accelerationTimeLimits.Count) break;

            int timeLimit = accelerationTimeLimits[motionStage];
            float baseSpeed = nowSpeed.magnitude;
            float degree = (baseSpeed != 0 ? baseSpeed : 1) * accelerationList[motionStage];

            for(int time = 0; time < timeLimit; time++)
            {
                float setSpeed = baseSpeed + easing.quadratic.Out(degree, time, timeLimit);
                Vector2 setVector = nowSpeed.magnitude != 0
                    ? nowSpeed.normalized
                    : initialVelocity;

                setVerosity(setVector, setSpeed);

                generateLocus(time);

                yield return wait(1);
            }
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
    /// <summary>
    /// 最大速度取得関数
    /// </summary>
    protected float getMaxSpeed()
    {
        float nowSpeed = initialSpeed;

        float returnValue = nowSpeed;
        foreach(var acceleration in accelerationList)
        {
            nowSpeed += (nowSpeed != 0 ? nowSpeed : 1) * acceleration;
            returnValue = Mathf.Max(nowSpeed, returnValue);
        }
        return returnValue;
    }
    public override float nowPower
    {
        get {
            return base.nowPower * nowSpeed.magnitude / getMaxSpeed();
        }
    }
}
