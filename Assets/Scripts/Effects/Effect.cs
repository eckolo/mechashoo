using UnityEngine;
using System.Collections;

public class Effect : Material
{
    protected override void baseStart()
    {
        base.baseStart();
        Action();
    }
}
