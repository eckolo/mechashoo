using UnityEngine;
using System.Collections;

/// <summary>
/// 誘導弾クラス
/// </summary>
public class Missile : Shell
{
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
        target = getNearTarget();
        correctionDegree = 0.01f;
        timerName = timer.start(timerName);
    }

    public override void Update()
    {
        base.Update();
        if (target != null && (correctionLimit == 0 || timer.get(timerName) < correctionLimit))
        {
            velocity = correctValue(target.transform.position - transform.position, velocity, correctionDegree);
            setVerosity(velocity, initialSpeed);
        }
    }
}
