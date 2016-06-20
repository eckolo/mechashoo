using UnityEngine;
using System.Collections;

public class Shell : Bullet
//Bulletの中でも特に弾丸っぽいやつ
{
    //弾丸が自動で移動方向を向く
    protected override void setVerosityAction(Vector2 verosity, float speed)
    {
        setAngle(verosity, widthPositive);
    }
}
