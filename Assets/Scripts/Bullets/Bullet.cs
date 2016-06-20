using System.Collections.Generic;
using UnityEngine;

public class Bullet : Material
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    public int power = 1;
    /// <summary>
    /// 弾の移動スピード
    /// </summary>
    public int initialSpeed = 10;
    /// <summary>
    /// 弾の移動方向ベクトル
    /// </summary>
    public Vector2 velocity;

    /// <summary>
    ///衝突時消滅フラグ
    /// </summary>
    [SerializeField]
    private bool flugCollisionDestroy = true;
    /// <summary>
    ///連続ヒット間隔
    /// </summary>
    [SerializeField]
    private int hitInterval = 0;
    [SerializeField]
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();

    protected override void baseStart()
    {
        // 移動
        setVerosity(velocity, initialSpeed);
    }
    protected override void baseUpdate()
    {
        // 毎フレーム消滅判定
        autoClear();
    }

    /// <summary>
    /// ぶつかった瞬間に呼び出される
    /// </summary>
    void OnTriggerEnter2D(Collider2D target)
    {
        Ship targetShip;
        if ((targetShip = target.GetComponent<Ship>()) == null) return;
        if (!hitTimer.ContainsKey(targetShip)) hitTimer.Add(targetShip, hitInterval);
        if (hitTimer[targetShip]++ >= hitInterval)
        {
            hitTimer[targetShip] = 0;

            // 弾の削除
            if (flugCollisionDestroy) selfDestroy();

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
            selfDestroy();
        }
    }

    protected void selfDestroy()
    {
        selfDestroyAction();
        Destroy(gameObject);
    }
    public virtual void selfDestroyAction() { }
}