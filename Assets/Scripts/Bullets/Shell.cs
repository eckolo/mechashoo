using UnityEngine;
using System.Collections;

/// <summary>
///Bulletの中でも特に弾丸っぽいやつ
/// </summary>
public class Shell : Bullet
{
    /// <summary>
    ///弾丸が自動で移動方向を向く
    /// </summary>
    protected override void setVerosityAction(Vector2 speed)
    {
        setAngle(speed, widthPositive);
    }
}
