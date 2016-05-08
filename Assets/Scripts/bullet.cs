using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 攻撃力
    public int power = 1;
    // 弾の移動スピード
    public int speed = 10;
    // 弾の移動方向ベクトル
    public Vector2 velocity;

    //衝突時消滅フラグ
    public bool flugCollisionDestroy = true;
    //連続ヒット間隔
    public int hitInterval = 0;
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();

    // ゲームオブジェクト生成から削除するまでの時間
    public float lifeTime = 3;

    void Start()
    {
        // ローカル座標のY軸方向に移動する
        GetComponent<Rigidbody2D>().velocity = velocity.normalized * speed;

        // lifeTime秒後に削除
        Destroy(gameObject, lifeTime);
    }

    // ぶつかった瞬間に呼び出される
    void OnTriggerEnter2D(Collider2D target)
    {
        Ship targetShip;
        if ((targetShip = target.GetComponent<Ship>()) == null) return;
        if (!hitTimer.ContainsKey(targetShip)) hitTimer.Add(targetShip, hitInterval);
        if (hitTimer[targetShip]++ >= hitInterval)
        {
            hitTimer[targetShip] = 0;

            // 弾の削除
            if (flugCollisionDestroy) Destroy(gameObject);

            targetShip.receiveDamage(power);
        }
    }
}