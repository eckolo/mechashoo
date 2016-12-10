using UnityEngine;
using System.Collections;

public class Panel : Methods
{
    private static int zPosition = 10;

    public override void Update()
    {
        base.Update();
        var keepPosition = transform.localPosition;
        if(keepPosition.z != zPosition) transform.localPosition = new Vector3(keepPosition.x, keepPosition.y, zPosition);
    }
}
