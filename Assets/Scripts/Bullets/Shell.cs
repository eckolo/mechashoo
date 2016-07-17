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
    ///弾丸が自動で移動方向を向く
    /// </summary>
    protected override void setVerosityAction(Vector2 speed)
    {
        setAngle(speed, widthPositive);
    }
    protected override IEnumerator Motion(int actionNum)
    {
        for (int motionStage = 0; motionStage < accelerationList.Count; motionStage++)
        {
            if (motionStage >= accelerationTimeLimits.Count) break;

            float degree = accelerationList[motionStage] - nowSpeed.magnitude;
            int timeLimit = accelerationTimeLimits[motionStage];
            Vector2 baseSpeed = nowSpeed;

            for (int time = 0; time < timeLimit; time++)
            {
                float setSpeed = baseSpeed.magnitude + easing.quadratic.Out(degree, time, timeLimit);
                Vector2 setVector = nowSpeed.magnitude != 0
                    ? nowSpeed.normalized
                    : initialVelocity;

                Debug.Log(gameObject.name + ":" + setVector + ":" + setSpeed);
                setVerosity(setVector, setSpeed);

                yield return null;
            }
        }
        yield break;
    }
}
