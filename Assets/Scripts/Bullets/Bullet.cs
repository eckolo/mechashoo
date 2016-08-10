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
    [SerializeField]
    protected float basePower = 1;
    /// <summary>
    /// 弾の移動スピード
    /// </summary>
    public float initialSpeed = 0;
    /// <summary>
    /// 弾の移動方向ベクトル
    /// </summary>
    public Vector2 initialVelocity;

    /// <summary>
    ///衝突時消滅フラグ
    /// </summary>
    [SerializeField]
    private bool collisionDestroy = true;
    /// <summary>
    ///対弾丸衝突フラグ
    /// </summary>
    [SerializeField]
    private bool collisionBullet = true;
    /// <summary>
    ///対弾丸強度
    ///負の値の場合は非破壊
    /// </summary>
    public float collisionStrength = 1;
    /// <summary>
    ///自動消滅時限
    ///0の場合は時間無制限
    /// </summary>
    [SerializeField]
    protected int destroyLimit = 0;
    /// <summary>
    ///連続ヒット間隔
    ///0未満にすることで連続ヒットオフ
    /// </summary>
    [SerializeField]
    protected int hitInterval = -1;
    [SerializeField]
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();
    /// <summary>
    ///ヒット時エフェクト
    /// </summary>
    [SerializeField]
    protected Hit hitEffect = null;
    /// <summary>
    ///弾丸ヒット時エフェクト
    /// </summary>
    [SerializeField]
    protected Hit hitBulletEffect = null;
    /// <summary>
    ///ヒット時SE
    /// </summary>
    public AudioClip hitSE = null;

    /// <summary>
    ///弾丸生成後経過タイマー名称
    /// </summary>
    private static string timerName = "bullet";

    protected Vector2 initialScale;

    protected override void baseStart()
    {
        // 移動
        attachRigidbody();
        initialScale = transform.localScale;
        setVerosity(initialVelocity, initialSpeed);
        timerName = timer.start(timerName);
        Action();
    }
    protected override void baseUpdate()
    {
        // 毎フレーム消滅判定
        autoClear();
    }

    /// <summary>
    /// 弾丸威力取得関数
    /// 基本的に出現後の経過時間とかで変動する
    /// </summary>
    public virtual float getPower()
    {
        return basePower;
    }

    /// <summary>
    /// Rigidbody2Dコンポーネントをアタッチするだけの関数
    /// </summary>
    protected Rigidbody2D attachRigidbody()
    {
        var rigidbody = gameObject.AddComponent<Rigidbody2D>();

        rigidbody.gravityScale = 0;

        return rigidbody;
    }
    /// <summary>
    /// ぶつかっている間呼び出される処理
    /// </summary>
    void OnTriggerStay2D(Collider2D target)
    {
        contactShip(target.GetComponent<Ship>(), false);
        contactBullet(target.GetComponent<Bullet>(), false);
    }
    /// <summary>
    /// ぶつかった瞬間に呼び出される
    /// </summary>
    void OnTriggerEnter2D(Collider2D target)
    {
        contactShip(target.GetComponent<Ship>(), true);
        contactBullet(target.GetComponent<Bullet>(), true);
    }
    protected void contactShip(Ship target, bool first)
    {
        if (target == null) return;
        if (!hitTimer.ContainsKey(target)) hitTimer.Add(target, hitInterval);
        if (first) hitTimer[target] = hitInterval;

        if (hitInterval >= 0 ? hitTimer[target]++ >= hitInterval : first)
        {
            soundSE(hitSE, 0.5f);
            outbreakHit(target);

            hitTimer[target] = 0;

            target.receiveDamage(getPower());

            // 弾の削除
            if (collisionDestroy) selfDestroy();
        }
    }
    protected void contactBullet(Bullet target, bool first)
    {
        if (target == null) return;

        if (collisionBullet && first)
        {
            soundSE(hitSE, 0.5f);
            outbreakHit(target, hitBulletEffect);

            // 弾の削除
            if (0 <= collisionStrength)
            {
                if (target.collisionStrength < 0) selfDestroy();
                if (collisionStrength <= target.collisionStrength) selfDestroy();
            }
        }
    }
    /// <summary>
    /// ヒットエフェクトの作成
    /// </summary>
    protected Hit outbreakHit(Material target, Hit hitObject = null)
    {
        var setHit = hitObject ?? hitEffect;
        if (setHit == null) return null;

        Vector2 setPosition = (target.transform.position - transform.position) / 2;
        Hit effect = outbreakEffect(setHit, 1, setPosition).GetComponent<Hit>();
        effect.transform.localScale = getLossyScale();

        addEffect(effect);
        return effect;
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
        //時間判定
        if (destroyLimit > 0 && timer.get(timerName) > destroyLimit)
        {
            selfDestroy();
        }
    }
}