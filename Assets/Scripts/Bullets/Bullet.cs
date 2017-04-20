using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸クラス
/// </summary>
public class Bullet : Things
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    [SerializeField]
    protected float basePower = 1;

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
    ///対弾丸被破壊フラグ
    /// </summary>
    [SerializeField]
    private bool destroyableBullet = true;
    /// <summary>
    ///対弾丸強度残量
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
    /// <summary>
    ///連続ヒットするか否かのフラグ
    /// </summary>
    protected bool isContinueHit => hitInterval >= 0;
    [SerializeField]
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();
    /// <summary>
    /// 衝突時の加力量補正
    /// </summary>
    [SerializeField]
    private float _impactCorrection = 0;
    protected virtual float impactCorrection
    {
        get {
            return _impactCorrection * Mathf.Min(nowPower, basePower) / basePower;
        }
    }
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
    protected static string bulletTimerName = "bullet";

    protected Vector2? initialScale = null;

    /// <summary>
    /// 弾丸発射者
    /// </summary>
    public Ship user { get; set; } = null;
    /// <summary>
    /// 弾丸生成元の武装
    /// </summary>
    public Weapon userWeapon { get; set; } = null;

    public override void Start()
    {
        base.Start();
        // 移動
        attachRigidbody();
        initialScale = transform.localScale;
        bulletTimerName = timer.start(bulletTimerName);
        action();
    }
    protected override void updateMotion()
    {
        base.updateMotion();
        // 毎フレーム消滅判定
        autoClear();
    }

    /// <summary>
    /// 弾丸威力取得関数
    /// 基本的に出現後の経過時間とかで変動する
    /// </summary>
    public virtual float nowPower
    {
        get {
            return basePower;
        }
    }
    /// <summary>
    /// Rigidbody2Dコンポーネントをアタッチするだけの関数
    /// </summary>
    protected Rigidbody2D attachRigidbody()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        if(rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody2D>();

        rigidbody.gravityScale = 0;

        return rigidbody;
    }
    /// <summary>
    /// ぶつかっている間呼び出される処理
    /// </summary>
    void OnTriggerStay2D(Collider2D target)
    {
        if(!onEnter(target)) return;
        contactShip(target.GetComponent<Ship>(), false);
    }
    /// <summary>
    /// ぶつかった瞬間に呼び出される
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D target)
    {
        if(!onEnter(target)) return;
        contactShip(target.GetComponent<Ship>(), true);
        contactBullet(target.GetComponent<Bullet>());
        base.OnTriggerEnter2D(target);
    }
    protected void contactShip(Ship target, bool first)
    {
        if(!onEnter(target)) return;

        if(isContinueHit)
        {
            if(!hitTimer.ContainsKey(target)) hitTimer.Add(target, hitInterval);
            if(first) hitTimer[target] = hitInterval;
            if(hitTimer[target]++ < hitInterval) return;
        }
        else if(!first) return;

        soundSE(hitSE);
        outbreakHit(target);

        if(isContinueHit) hitTimer[target] = 0;

        var damage = target.receiveDamage(nowPower);
        if(damage > 0) target.lastToHitShip = user;
        if(user?.GetComponent<Player>() != null) sys.countAttackHits();

        // 弾の削除
        if(collisionDestroy) selfDestroy();
    }
    protected void contactBullet(Bullet target)
    {
        if(!onEnter(target)) return;

        if(collisionBullet)
        {
            soundSE(hitSE);
            outbreakHit(target, hitBulletEffect);

            // 弾の削除
            if(destroyableBullet)
            {
                collisionStrength -= target.nowPower;
                if(collisionStrength <= 0) selfDestroy();
            }
        }
    }
    /// <summary>
    /// ヒットエフェクトの作成
    /// </summary>
    protected Hit outbreakHit(Things target, Hit hitObject = null)
    {
        var setHit = hitObject ?? hitEffect;
        if(setHit == null) return null;

        Vector2 setPosition = getHitPosition(target);
        Hit effect = outbreakEffect(setHit, 1, setPosition).GetComponent<Hit>();
        effect.transform.localScale = lossyScale;

        addEffect(effect);
        return effect;
    }
    protected virtual void addEffect(Hit effect) { }
    protected virtual Vector2 getHitPosition(Things target) => (target.globalPosition - globalPosition) / 2;
    protected sealed override Vector2 onCrashImpact(Things target) => base.onCrashImpact(target) + impactDirection(target).recalculation(impactCorrection);
    protected virtual Vector2 impactDirection(Things target) => nowSpeed;

    /// <summary>
    /// 自動消滅関数
    /// </summary>
    void autoClear()
    {
        //位置判定
        var upperRight = fieldUpperRight + viewSize;
        var lowerLeft = fieldLowerLeft - viewSize;
        if(globalPosition.x > upperRight.x
            || globalPosition.x < lowerLeft.x
            || globalPosition.y > upperRight.y
            || globalPosition.y < lowerLeft.y)
        {
            selfDestroy();
        }
        //時間判定
        if(destroyLimit > 0 && timer.get(bulletTimerName) > destroyLimit)
        {
            selfDestroy();
        }
    }
}