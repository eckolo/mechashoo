using UnityEngine;
using System.Collections;

public class MobileBattery : Gun
{
    public override void Start()
    {
        base.Start();
        Action();
    }

    protected override IEnumerator BaseMotion(int actionNum)
    {
        yield return base.BaseMotion(actionNum);
        DestroyMyself();
        yield break;
    }
}
