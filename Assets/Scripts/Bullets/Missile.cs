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
    ///誘導補正値
    /// </summary>
    [SerializeField]
    private float correctionDegree = 0;
    /// <summary>
    ///誘導ブレ
    /// </summary>
    [SerializeField]
    private float correctionShake = 0;
    /// <summary>
    ///誘導間隔フレーム数
    /// </summary>
    [SerializeField]
    private int correctionInterval = 0;
    /// <summary>
    ///誘導期限
    /// </summary>
    [SerializeField]
    private int correctionLimit = 0;
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
        correction(timer.get(timerName));
    }
    protected override IEnumerator motion(int actionNum)
    {
        for(int time = 0; time <= thrustLimit; time++)
        {
            exertPower(nowForward, thrustPower);
            generateLocus(time);
            yield return wait(1);
        }
        yield return base.motion(actionNum);
        yield break;
    }

    private void correction(int time)
    {
        if(target == null) return;
        if(correctionDegree <= 0) return;
        if(time % Mathf.Max(correctionInterval + 1, 1) > 0) return;
        if(correctionLimit != 0 && time > correctionLimit) return;

        var vector = MathV.correct((target.globalPosition - globalPosition).normalized, nowSpeed.normalized, correctionDegree);
        var rotation = Quaternion.AngleAxis(Random.Range(-correctionShake, correctionShake), Vector3.forward);
        setAngle(MathA.toAngle(rotation * vector));

        return;
    }
}
