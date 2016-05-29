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
    [SerializeField]
    private bool flugCollisionDestroy = true;
    //連続ヒット間隔
    [SerializeField]
    private int hitInterval = 0;
    [SerializeField]
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();

    public virtual void Start()
    {
        // ローカル座標のY軸方向に移動する
        GetComponent<Rigidbody2D>().velocity = velocity.normalized * speed;
    }
    public virtual void Update()
    {
        // ローカル座標のY軸方向に移動する
        autoClear();
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

    void autoClear()
    {
        var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(2, 2));
        var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(-1, -1));
        if (transform.position.x > upperRight.x
            || transform.position.x < lowerLeft.x
            || transform.position.y > upperRight.y
            || transform.position.y < lowerLeft.y)
        {
            Destroy(gameObject);
        }
    }

    void selfDestroy()
    {
        selfDestroyAction();
        Destroy(gameObject);
    }
    public virtual void selfDestroyAction() { }
}