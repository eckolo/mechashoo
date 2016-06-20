using UnityEngine;
using System.Collections;

/// <summary>
/// 誘導弾クラス
/// </summary>
public class Missile : Bullet
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
    private int correctionCount = 0;

    public override void Start()
    {
        base.Start();
        target = getNearTarget();
        correctionDegree = 0.01f;
    }

    public override void Update()
    {
        base.Update();
        if (target != null && (correctionLimit == 0 || correctionCount++ < correctionLimit))
        {
            velocity = correctVector(velocity, target.transform.position - transform.position, 1 - correctionDegree);
            setVerosity(velocity, initialSpeed);
        }
    }
}
