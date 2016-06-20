using UnityEngine;
using System.Collections;

public class Ringcutter : Bullet
{
    //誘導対象
    public Ship target = null;

    //誘導補正値
    [SerializeField]
    private float correctionDegree = 0.5f;
    //誘導期限
    [SerializeField]
    private int correctionLimit = 0;
    //誘導期限計算用カウント
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
