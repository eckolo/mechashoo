using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸クラス
/// </summary>
public class Bullet : Material
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    public float power = 1;
    /// <summary>
    /// 弾の移動スピード
    /// </summary>
    public float initialSpeed = 10;
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
    /// <summary>
    ///ヒット時エフェクト
    /// </summary>
    [SerializeField]
    protected Hit hitEffect = null;

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

        if (hitEffect != null)
        {
            Hit effect = (Hit)Instantiate(hitEffect, (transform.position + targetShip.transform.position) / 2, transform.rotation);
            effect.transform.localScale = getLossyScale();
            addEffect(effect);
        }

        if (hitTimer[targetShip]++ >= hitInterval)
        {
            hitTimer[targetShip] = 0;

            // 弾の削除
            if (flugCollisionDestroy) selfDestroy();

            targetShip.receiveDamage(power);
        }
    }
    protected virtual void addEffect(Hit effect) { }

    /// <summary>
    /// 自動消滅関数
    /// </summary>
    void autoClear()
    {
        //位置判定
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

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public override void selfDestroy()
    {
        selfDestroyAction();
        Destroy(gameObject);
    }
    protected virtual void selfDestroyAction() { }
}