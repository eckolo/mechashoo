using UnityEngine;
using System.Collections;

public class Shell : Bullet
//Bulletの中でも特に弾丸っぽいやつ
{
    //弾丸が自動で移動方向を向く
    public override void setVerosityAction(Vector2 verosity, float speed)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.right, verosity);
    }
}
