using UnityEngine;
using System.Collections;

public class Panel : Methods
{
    private static int zPosition = 10;

    public override void Update()
    {
        base.Update();
        if(nowZ != zPosition) nowZ = zPosition;
    }
}
