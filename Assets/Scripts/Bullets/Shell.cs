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
    protected int locusInterval = 0;
    /// <summary>
    ///通過後に発生する系のエフェクトの発生位置
    /// </summary>
    [SerializeField]
    protected Vector2 locusPosition = new Vector2(0, 0);

    /// <summary>
    ///弾丸が自動で移動方向を向く
    /// </summary>
    protected override void setVerosityAction(Vector2 speed)
    {
        setAngle(speed, widthPositive);
        transform.localScale = new Vector2(initialScale.x * (1 + speed.magnitude / getPixel()), initialScale.y * (1 - speed.magnitude / getPixel()));
    }
    protected override IEnumerator Motion(int actionNum)
    {
        for (int motionStage = 0; motionStage < accelerationList.Count; motionStage++)
        {
            if (motionStage >= accelerationTimeLimits.Count) break;

            float degree = accelerationList[motionStage];
            int timeLimit = accelerationTimeLimits[motionStage];
            float baseSpeed = nowSpeed.magnitude;

            for (int time = 0; time < timeLimit; time++)
            {
                float setSpeed = baseSpeed + easing.quadratic.Out(degree, time, timeLimit);
                Vector2 setVector = nowSpeed.magnitude != 0
                    ? nowSpeed.normalized
                    : initialVelocity;

                setVerosity(setVector, setSpeed);

                generateLocus(time);

                yield return null;
            }
        }
        yield break;
    }
    private void generateLocus(int time)
    {
        var setedLocus = locus;
        if (setedLocus == null) return;
        if (time % Mathf.Max(locusInterval + 1, 1) > 0) return;

        Vector2 locusPositionLocal = transform.rotation * locusPosition;
        locusPositionLocal = (Vector2)transform.position
            + new Vector2(locusPositionLocal.x * getLossyScale().x,
            locusPositionLocal.y * getLossyScale().y);


        outbreakEffect(locus, getLossyScale().magnitude, locusPosition);
    }
    protected float getMaxSpeed()
    {
        float nowSpeed = initialSpeed;

        float returnValue = nowSpeed;
        foreach (var acceleration in accelerationList)
        {
            nowSpeed += acceleration;
            returnValue = Mathf.Max(nowSpeed, returnValue);
        }
        return returnValue;
    }
    public override float getPower()
    {
        return base.getPower() * nowSpeed.magnitude / getMaxSpeed();
    }
}
