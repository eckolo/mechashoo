using UnityEngine;
using System.Collections;

public class Laser : Bullet
{
    protected override IEnumerator Motion(int actionNum)
    {
        selfDestroy();
        yield break;
    }
}
