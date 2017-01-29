using UnityEngine;
using System.Collections;

/// <summary>
/// 誘導弾クラス
/// </summary>
public class Missile : Shell
{
    /// <summary>
    ///推進力
    /// </summary>
    [SerializeField]
    private float thrustPower = 0.1f;
    /// <summary>
    ///推進期限
    /// </summary>
    [SerializeField]
    private int thrustLimit = 3;

    /// <summary>
    ///誘導対象
    /// </summary>
    [System.NonSerialized]
    public Ship target = null;

    /// <summary>
    ///誘導が常に発生するか否か
    /// </summary>
    [SerializeField]
    protected bool alwaysInduce = true;
    /// <summary>
    ///誘導補正値
    /// </summary>
    [SerializeField]
    private float induceDegree = 0;
    /// <summary>
    ///誘導ブレ
    /// </summary>
    [SerializeField]
    private float induceShake = 0;
    /// <summary>
    ///誘導間隔フレーム数
    /// </summary>
    [SerializeField]
    private int induceInterval = 0;
    /// <summary>
    ///誘導期限
    /// </summary>
    [SerializeField]
    private int induceLimit = 0;
    /// <summary>
    ///誘導期限計算用カウント
    /// </summary>
    private static string timerName = "correction";

    public override void Start()
    {
        base.Start();
        target = nowNearTarget;
        timerName = timer.start(timerName);
    }

    protected override void updateMotion()
    {
        base.updateMotion();
        induce(timer.get(timerName));
    }
    protected override IEnumerator motion(int actionNum)
    {
        for(int time = 0; time < thrustLimit; time++)
        {
            exertPower(nowForward, thrustPower);
            generateLocus(time);
            yield return wait(1);
        }
        yield return base.motion(actionNum);
        yield break;
    }

    private void induce(int time)
    {
        if(target == null) return;
        if(induceDegree <= 0) return;
        if(time % Mathf.Max(induceInterval + 1, 1) > 0) return;
        if(induceLimit != 0 && time > induceLimit) return;

        var direction = MathV.correct(target.globalPosition - globalPosition, nowForward, induceDegree);
        var correction = MathA.toRotation(Random.Range(-induceShake, induceShake));
        nowForward = correction * direction;
        if(alwaysInduce && time >= thrustLimit) exertPower(nowForward, thrustPower, nowSpeed.magnitude);

        return;
    }
}
