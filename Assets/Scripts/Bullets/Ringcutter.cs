using UnityEngine;
using System.Collections;

public class Ringcutter : Bullet
{
    //誘導対象
    public GameObject target = null;

    //誘導補正値
    [SerializeField]
    private float correctionDegree = 0.5f;

    public override void Update()
    {
        base.Update();
        if (target != null)
        {
            velocity = correctVector(velocity, target.transform.position - transform.position, correctionDegree);
            setVerosity(velocity, speed);
        }
    }
}
