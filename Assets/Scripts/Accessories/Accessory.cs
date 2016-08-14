using UnityEngine;
using System.Collections;

public class Accessory : Parts
{
    protected override void baseStart()
    {
        base.baseStart();
        Action();
    }
    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public virtual void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        return;
    }
}
