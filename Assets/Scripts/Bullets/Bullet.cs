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
    private Dictionary<Ship, int> hitTimer = new Dictionary<Ship, int>();
    /// <summary>
    /// 衝突時の加力量補正
    /// </summary>
    [SerializeField]
    private float _impactCorrection = 0;
    protected virtual float impactCorrection
    {
        get {
            if(basePower == 0) return 0;
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
    public Ship user
    {
        get {
            if(_user == null) return null;
            return _user;
        }
        set {
            _user = value;
        }
    }
    Ship _user = null;
    /// <summary>
    /// 弾丸生成元の武装
    /// </summary>
    public Weapon userWeapon { get; set; } = null;

    public override void Start()
    {
        base.Start();
        initialScale = transform.localScale;
        bulletTimerName = timer.Start(bulletTimerName);
        if(user?.nowLayer != Configs.Layers.PLAYER) sys.CountEnemyAttackCount();
        Action();
    }
    protected override void UpdateMotion()
    {
        base.UpdateMotion();
        // 毎フレーム消滅判定
        AutoClear();
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
    /// ぶつかっている間呼び出される処理
    /// </summary>
    void OnTriggerStay2D(Collider2D target)
    {
        if(!OnEnter(target)) return;
        ContactShip(target.GetComponent<Ship>(), false);
    }
    /// <summary>
    /// ぶつかった瞬間に呼び出される
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D target)
    {
        if(!OnEnter(target)) return;
        ContactShip(target.GetComponent<Ship>(), true);
        ContactBullet(target.GetComponent<Bullet>());
        base.OnTriggerEnter2D(target);
    }
    protected virtual bool ContactShip(Ship target, bool first)
    {
        if(!OnEnter(target)) return false;

        if(isContinueHit)
        {
            if(!hitTimer.ContainsKey(target)) hitTimer.Add(target, hitInterval);
            if(first) hitTimer[target] = hitInterval;
            if(hitTimer[target]++ < hitInterval) return false;
        }
        else if(!first) return false;

        SoundSE(hitSE);
        OutbreakHit(target);

        if(isContinueHit) hitTimer[target] = 0;

        var damage = target.ReceiveDamage(nowPower);
        if(damage > 0) target.lastToHitShip = user;
        if(basePower > 0 && user?.GetComponent<Player>() != null) sys.CountAttackHits();

        // 弾の削除
        if(collisionDestroy) DestroyMyself();
        return true;
    }
    protected virtual bool ContactBullet(Bullet target)
    {
        if(!collisionBullet) return false;
        if(!OnEnter(target)) return false;
        if(!target.collisionBullet) return false;

        SoundSE(hitSE);
        OutbreakHit(target, hitBulletEffect);
        if(basePower > 0 && user?.GetComponent<Player>() != null) sys.CountAttackHits();

        // 弾の削除
        if(destroyableBullet)
        {
            collisionStrength -= target.nowPower;
            if(collisionStrength <= 0) DestroyMyself();
        }
        return true;
    }
    /// <summary>
    /// 物体生成
    /// </summary>
    /// <typeparam name="Injected">生成するオブジェクトの型</typeparam>
    /// <param name="injectObject">生成するオブジェクトの雛形</param>
    /// <param name="injectPosition">生成する座標</param>
    /// <param name="injectAngle">生成時の角度</param>
    /// <returns>生成されたオブジェクト</returns>
    protected override Injected Inject<Injected>(Injected injectObject, Vector2 injectPosition, float injectAngle = 0)
    {
        var injected = base.Inject(injectObject, injectPosition, injectAngle);
        var bullet = injected.GetComponent<Bullet>();
        if(bullet != null) bullet.user = user;
        return injected;
    }
    /// <summary>
    /// ヒットエフェクトの作成
    /// </summary>
    protected Hit OutbreakHit(Things target, Hit hitObject = null)
    {
        var setHit = hitObject ?? hitEffect;
        if(setHit == null) return null;

        Vector2 setPosition = GetHitPosition(target);
        Hit effect = OutbreakEffect(setHit, 1, setPosition).GetComponent<Hit>();
        effect.transform.localScale = lossyScale;

        AddEffect(effect);
        return effect;
    }
    protected virtual void AddEffect(Hit effect) { }
    protected virtual Vector2 GetHitPosition(Things target) => (target.globalPosition - globalPosition) / 2;
    protected sealed override Vector2 OnCrashImpact(Things target) => base.OnCrashImpact(target) + ImpactDirection(target).ToVector(impactCorrection);
    protected virtual Vector2 ImpactDirection(Things target) => nowSpeed;

    /// <summary>
    /// 自動消滅関数
    /// </summary>
    void AutoClear()
    {
        //位置判定
        var upperRight = fieldUpperRight + viewSize;
        var lowerLeft = fieldLowerLeft - viewSize;
        if(globalPosition.x > upperRight.x
            || globalPosition.x < lowerLeft.x
            || globalPosition.y > upperRight.y
            || globalPosition.y < lowerLeft.y)
        {
            DestroyMyself();
        }
        //時間判定
        if(destroyLimit > 0 && timer.Get(bulletTimerName) > destroyLimit)
        {
            DestroyMyself();
        }
    }
}