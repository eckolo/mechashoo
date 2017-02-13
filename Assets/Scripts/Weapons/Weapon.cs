using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 武装クラス
/// </summary>
public class Weapon : Parts
{
    /// <summary>
    ///現在攻撃動作可能かどうかの判定フラグ
    /// </summary>
    public bool canAction
    {
        get {
            return notInAction && _canAction;
        }
        set {
            _canAction = value;
        }
    }
    bool _canAction = true;
    /// <summary>
    ///持ち手の座標
    /// </summary>
    public Vector2 handlePosition = Vector2.zero;
    /// <summary>
    ///持ち手の奥行き座標
    /// </summary>
    public float handledZ = 1;
    /// <summary>
    ///射出孔関連のパラメータ
    /// </summary>
    [System.Serializable]
    public class Injection
    {
        /// <summary>
        ///射出孔の座標
        /// </summary>
        public Vector2 hole = Vector2.zero;
        /// <summary>
        ///射出角補正
        /// </summary>
        public float angle = 0;
        /// <summary>
        ///初速度
        /// </summary>
        public float initialVelocity = 1;
        /// <summary>
        /// 射出時燃料消費量補正値
        /// </summary>
        public float fuelCostPar = 1;
        /// <summary>
        ///射出弾丸の特殊指定
        /// </summary>
        public Bullet bullet = null;
        /// <summary>
        /// 射出後の弾丸を武装と連結状態にするフラグ
        /// </summary>
        public bool union = false;
        /// <summary>
        ///射出時の効果音
        /// </summary>
        public AudioClip se = null;
        /// <summary>
        /// 射出前のチャージエフェクト特殊指定
        /// </summary>
        public Effect charge = null;
        /// <summary>
        ///射出タイミング特殊指定
        /// </summary>
        public List<ActionType> timing = new List<ActionType>();
    }
    public List<Injection> injections = new List<Injection>();
    /// <summary>
    /// 弾のPrefab
    /// </summary>
    public Bullet Bullet;

    /// <summary>
    ///攻撃動作開始可能かどうか(つまり動作中か否か)の内部フラグ
    /// </summary>
    [System.NonSerialized]
    protected bool notInAction = true;

    /// <summary>
    ///1モーションの所要時間
    /// </summary>
    [SerializeField]
    protected int timeRequired;
    /// <summary>
    /// アクション毎の間隔
    /// </summary>
    public int actionDelay;
    /// <summary>
    ///弾丸密度
    /// </summary>
    [SerializeField]
    protected int density = 1;
    /// <summary>
    ///デフォルトの向き
    /// </summary>
    [SerializeField]
    protected float defAngle = 0;
    /// <summary>
    ///外部由来の基礎角度
    /// </summary>
    private float baseAngle = 0;
    /// <summary>
    ///Handに対しての描画順の前後のデフォルト値
    /// </summary>
    [SerializeField]
    public int defaultZ = -1;
    /// <summary>
    ///起動時燃料消費量基準値
    /// </summary>
    [SerializeField]
    public float motionFuelCost = 1;
    /// <summary>
    ///射出時燃料消費量基準値
    /// </summary>
    [SerializeField]
    public float injectionFuelCost = 1;

    /// <summary>
    /// チャージエフェクト
    /// </summary>
    [SerializeField]
    protected Effect chargeEffect = null;

    /// <summary>
    ///次のモーションの内部指定値
    /// </summary>
    protected ActionType nextAction = ActionType.NOMOTION;

    public override void Start()
    {
        base.Start();
        setAngle(baseAngle + defAngle);
    }

    public override void Update()
    {
        base.Update();
        if(inAction)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.8f, 1);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        }
    }

    public float setBaseAngle(float setedAngle)
    {
        return baseAngle = setedAngle;
    }
    public override float setAngle(float settedAngle)
    {
        return base.setAngle(baseAngle + settedAngle);
    }

    public bool inAction
    {
        get {
            if(nowRoot == null) return !notInAction;
            if(nowRoot.GetComponent<Weapon>() == null) return !notInAction;
            return nowRoot.GetComponent<Weapon>().inAction;
        }
    }

    public enum ActionType
    {
        NOMOTION,
        NOMAL,
        SINK,
        FIXED,
        NPC
    }
    public bool action(ActionType action = ActionType.NOMAL)
    {
        if(!canAction || action == ActionType.NOMOTION)
        {
            nextAction = action;
            return false;
        }

        notInAction = false;
        return base.action((int)action);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        bool normalOperation = reduceShipFuel(motionFuelCost);

        if(normalOperation) yield return base.baseMotion(actionNum);

        var timerKey = "weaponEndMotion";
        timer.start(timerKey);
        if(normalOperation) yield return endMotion(actionNum);
        if(actionDelay > 0) yield return wait(actionDelay - timer.get(timerKey));
        timer.stop(timerKey);

        notInAction = true;
        if(nextAction != ActionType.NOMOTION)
        {
            action(nextAction);
            nextAction = ActionType.NOMOTION;
        }

        yield break;
    }

    protected override IEnumerator motion(int actionNum)
    {
        if(Enums<ActionType>.isDefined(actionNum)) yield return motion((ActionType)actionNum);
        yield break;
    }
    protected virtual IEnumerator motion(ActionType action)
    {
        inject(injections[(int)action]);
        yield break;
    }
    protected IEnumerator endMotion(int actionNum)
    {
        if(Enums<ActionType>.isDefined(actionNum)) yield return endMotion((ActionType)actionNum);
        yield break;
    }
    protected virtual IEnumerator endMotion(ActionType action)
    {
        yield break;
    }

    protected bool reduceShipFuel(float reduceValue, float fuelCorrection = 1)
    {
        Ship rootShip = nowRoot.GetComponent<Ship>();
        if(rootShip == null) return true;
        return rootShip.reduceFuel(reduceValue * fuelCorrection);
    }

    /// <summary>
    /// 弾の作成
    /// 武装毎の射出孔番号で指定するタイプ
    /// </summary>
    protected Bullet inject(Injection injection, float fuelCorrection = 1, float angleCorrection = 0)
    {
        if(injection == null) return null;

        var confirmBullet = injection.bullet ?? Bullet;
        if(confirmBullet == null) return null;

        if(!reduceShipFuel(injectionFuelCost * injection.fuelCostPar, fuelCorrection)) return confirmBullet;

        var forwardAngle = injection.angle + angleCorrection;

        soundSE(injection.se);
        var bullet = inject(confirmBullet, injection.hole, forwardAngle);
        bullet.setVerosity(forwardAngle.toRotation() * nowForward * injection.initialVelocity);
        if(injection.union) bullet.nowParent = transform;

        return bullet;
    }

    /// <summary>
    /// 所定のアクションタイプに合致した発射孔のみを拾って返す
    /// </summary>
    /// <param name="actionType">所定のアクションタイプ</param>
    /// <returns></returns>
    protected List<Injection> getOnTypeInjections(ActionType actionType) => injections
            .Where(injection => injection.timing.Contains(actionType) || !injection.timing.Any())
            .ToList();

    /// <summary>
    /// モーションの雛形となるクラスインターフェース
    /// </summary>
    /// <typeparam name="WeaponType">モーションの適用される武装種別クラス</typeparam>
    protected interface IMotion<WeaponType>
    {
        IEnumerator mainMotion(WeaponType weapon);
        IEnumerator endMotion(WeaponType weapon);
    }
}
