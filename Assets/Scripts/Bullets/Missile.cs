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
    private const string TIMER_NAME = "induce";

    public override void Start()
    {
        base.Start();
        target = target ?? nowNearTarget;
        timer.Start(TIMER_NAME);
    }

    protected override void UpdateMotion()
    {
        base.UpdateMotion();
        Induce(timer.Get(TIMER_NAME));
    }
    protected override IEnumerator Motion(int actionNum)
    {
        for(int time = 0; time < thrustLimit; time++)
        {
            ExertPower(nowForward, thrustPower);
            GenerateLocus(time);
            yield return Wait(1);
        }
        yield return base.Motion(actionNum);
        yield break;
    }

    protected override float maxSpeed
    {
        get {
            if(_maxSpeed == null) _maxSpeed = GetExertPowerResult(nowForward, thrustPower, thrustLimit).magnitude;
            return _maxSpeed ?? 0;
        }
    }
    float? _maxSpeed = null;

    private void Induce(int time)
    {
        if(target == null) return;
        if(!target.inField) return;
        if(induceDegree <= 0) return;
        if(time % Mathf.Max(induceInterval + 1, 1) > 0) return;
        if(induceLimit != 0 && time > induceLimit) return;

        var direction = (target.globalPosition - globalPosition).Correct(nowForward, induceDegree);
        var correction = Random.Range(-induceShake, induceShake).ToRotation();
        nowForward = correction * direction;
        if(alwaysInduce && time >= thrustLimit) ExertPower(nowForward, thrustPower, nowSpeed.magnitude);

        return;
    }
}
