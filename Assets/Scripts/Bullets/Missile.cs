using UnityEngine;
using System.Collections;

/// <summary>
/// 誘導弾クラス
/// </summary>
public class Missile : Shell {
    /// <summary>
    ///誘導対象
    /// </summary>
    public Ship target = null;

    /// <summary>
    ///誘導補正値
    /// </summary>
    [SerializeField]
    private float correctionDegree = 0.5f;
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

    public override void Start() {
        base.Start();
        target = nowNearTarget;
        timerName = timer.start(timerName);
    }

    protected override void updateMotion() {
        base.updateMotion();
        correction(timer.get(timerName));
    }

    private void correction(int time) {
        if(target == null)
            return;
        if(time % Mathf.Max(correctionInterval + 1, 1) > 0)
            return;
        if(correctionLimit != 0 && time > correctionLimit)
            return;

        var vector = MathV.correct((target.transform.position - transform.position).normalized, nowSpeed.normalized, correctionDegree);
        var rotation = Quaternion.AngleAxis(Random.Range(-correctionShake, correctionShake), Vector3.forward);
        setVerosity(rotation * vector, nowSpeed.magnitude);

        return;
    }
}
