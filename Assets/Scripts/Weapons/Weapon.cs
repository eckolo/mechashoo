using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;

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
    [Serializable]
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
        /// 連射数特殊指定
        /// </summary>
        public int burst = 0;
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
    [SerializeField]
    private List<Injection> injections = new List<Injection>();
    public virtual List<Injection> nowInjections => injections;
    /// <summary>
    /// 所定のアクションタイプに合致した発射孔のみを拾って返す
    /// </summary>
    /// <param name="actionType">所定のアクションタイプ</param>
    /// <returns></returns>
    protected List<Injection> onTypeInjections => nowInjections
            .Where(injection => injection.timing.Contains(nowAction) || !injection.timing.Any())
            .ToList();
    /// <summary>
    /// 弾のPrefab
    /// </summary>
    [SerializeField]
    private Bullet defBullet = null;
    public Bullet getBullet(Injection injection) => injection.bullet ?? defBullet;

    /// <summary>
    ///攻撃動作開始可能かどうか(つまり動作中か否か)の内部フラグ
    /// </summary>
    protected bool notInAction = true;

    /// <summary>
    ///1モーションの所要時間
    /// </summary>
    [SerializeField]
    protected int timeRequired;
    /// <summary>
    /// アクション毎の間隔
    /// </summary>
    [SerializeField]
    private int actionDelay;
    protected int actionDelayFinal => Mathf.CeilToInt(actionDelay * delayTweak);
    /// <summary>
    ///弾丸密度
    /// </summary>
    [SerializeField]
    protected int density = 1;
    /// <summary>
    ///デフォルトの向き
    /// </summary>
    [SerializeField]
    public float defAngle = 0;
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
    /// 行動間隔の補正値
    /// </summary>
    protected float delayTweak = 1;
    /// <summary>
    /// 現在のモーションの内部指定値
    /// </summary>
    public ActionType nowAction { get; protected set; } = ActionType.NOMOTION;
    /// <summary>
    /// 次のモーションの内部指定値
    /// </summary>
    public ActionType nextAction { get; protected set; } = ActionType.NOMOTION;

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

    protected override void updateMotion()
    {
        base.updateMotion();
        updateRecoil();
    }

    public float setBaseAngle(float setedAngle)
    {
        return baseAngle = setedAngle;
    }
    public override float setAngle(float settedAngle)
    {
        return base.setAngle(baseAngle + settedAngle);
    }

    public override Vector2 nowLengthVector
    {
        get {
            return nowInjections.Max(injection => injection.hole) - handlePosition;
        }
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
    public bool action(ActionType actionType = ActionType.NOMAL, float setActionDelayTweak = 1, int actionNum = 0)
    {
        if(!canAction || actionType == ActionType.NOMOTION)
        {
            nextAction = actionType;
            return false;
        }

        notInAction = false;
        nowAction = actionType;
        delayTweak = setActionDelayTweak;
        return base.action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        bool normalOperation = reduceShipFuel(motionFuelCost);

        if(normalOperation) yield return base.baseMotion(actionNum);

        var timerKey = "weaponEndMotion";
        timer.start(timerKey);
        if(normalOperation) yield return endMotion(actionNum);
        if(actionDelayFinal > 0) yield return wait(actionDelayFinal - timer.get(timerKey));
        timer.stop(timerKey);

        notInAction = true;
        if(nextAction != ActionType.NOMOTION)
        {
            action(nextAction, delayTweak, actionNum);
            nextAction = ActionType.NOMOTION;
        }

        yield break;
    }

    protected override IEnumerator motion(int actionNum)
    {
        yield break;
    }
    protected virtual IEnumerator endMotion(int actionNum)
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
    protected virtual Bullet inject(Injection injection, float fuelCorrection = 1, float angleCorrection = 0)
    {
        if(injection == null) return null;

        var confirmBullet = getBullet(injection);
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
    /// モーションの雛形となるクラスインターフェース
    /// </summary>
    /// <typeparam name="WeaponType">モーションの適用される武装種別クラス</typeparam>
    protected interface IMotion<WeaponType>
    {
        IEnumerator mainMotion(WeaponType weapon, bool forward = true);
        IEnumerator endMotion(WeaponType weapon, bool forward = true);
    }

    protected float tokenArmLength
    {
        get {
            var tokenHand = nowParent.GetComponent<Hand>();
            if(tokenHand == null) return 0;

            var tokenArm = tokenHand.nowParent.GetComponent<Arm>();
            if(tokenArm == null) return tokenHand.nowLengthVector.magnitude;

            return (tokenArm.nowLengthVector + tokenHand.nowLengthVector).magnitude;
        }
    }
    protected Hand tokenHand
    {
        get {
            if(nowParent == null) return null;

            var parentHand = nowParent.GetComponent<Hand>();
            if(parentHand != null) return parentHand;

            var parentWeapon = nowParent.GetComponent<Weapon>();
            if(parentWeapon != null) return parentWeapon.tokenHand;

            return null;
        }
    }

    /// <summary>
    /// 武器振り動作
    /// </summary>
    /// <param name="endPosition">動作終着地点座標</param>
    /// <param name="timeLimit">必要時間</param>
    /// <param name="timeEasing">動作速度のイージング関数</param>
    /// <param name="clockwise">時計回りフラグ</param>
    /// <param name="midstreamProcess">並列動作</param>
    /// <returns></returns>
    protected IEnumerator swingAction(Vector2 endPosition,
        int timeLimit,
        Func<float, float, float, float> timeEasing,
        bool clockwise,
        UnityAction<int, float, int> midstreamProcess = null)
    {
        var startPosition = correctionVector;
        var radiusCriteria = tokenArmLength;

        for(int time = 0; time < timeLimit; time++)
        {
            var limit = timeLimit - 1;
            float localTime = timeEasing(limit, time, limit);

            correctionVector = MathV.EasingV.elliptical(startPosition, endPosition * radiusCriteria, localTime, limit, clockwise);
            midstreamProcess?.Invoke(time, localTime, limit);
            yield return wait(1);
        }
        yield break;
    }

    /// <summary>
    /// 反動開始関数
    /// </summary>
    /// <param name="injection">反動発生元発射孔の情報</param>
    /// <param name="_recoilRate">反動量係数</param>
    protected void setRecoil(Injection injection, float _recoilRate = 1)
    {
        var injectBullet = getBullet(injection);
        var recoilPower = _recoilRate * injection.initialVelocity;
        var setedRecoil = (injection.angle + 180).recalculation(recoilPower) * injectBullet.weight;

        var ship = nowParent.GetComponent<Ship>();
        if(ship != null)
        {
            var direction = getWidthRealRotation(getLossyRotation() * (lossyScale.y.toSign() * injection.angle).toRotation()) * Vector2.left;
            ship.exertPower(direction, Mathf.Log(setedRecoil.magnitude + 1, 2) * baseMas.magnitude);
        }
        else
        {
            var recoil = Vector2.right * Mathf.Log(Mathf.Abs(setedRecoil.x) + 1) * setedRecoil.x.toSign() + Vector2.up * Mathf.Log(Mathf.Abs(setedRecoil.y) + 1) * setedRecoil.y.toSign();
            setRecoil(recoil);
        }
    }
    /// <summary>
    /// 反動開始関数
    /// </summary>
    /// <param name="hangForce">反動量</param>
    protected void setRecoil(Vector2 hangForce)
    {
        Vector2 degree = hangForce - recoilSpeed;
        var length = degree.magnitude;
        float variation = length != 0 ? Mathf.Clamp(hangForce.magnitude / length, -1, 1) : 0;

        recoilSpeed = recoilSpeed + degree * variation;
    }
    /// <summary>
    /// 反動量の逐次計算実行関数
    /// </summary>
    void updateRecoil()
    {
        nowRecoil += recoilSpeed;
        if(nowRecoil.magnitude == 0) recoilSpeed = Vector2.zero;
        else if(nowRecoil.magnitude < tokenHand.power) recoilSpeed = -nowRecoil;
        else setRecoil(-nowRecoil.recalculation(tokenHand.power));
    }
    /// <summary>
    /// 現在の反動量
    /// </summary>
    Vector2 nowRecoil = Vector2.zero;
    /// <summary>
    /// 現在の反動量の変化量
    /// </summary>
    Vector2 recoilSpeed = Vector2.zero;
    /// <summary>
    /// 先端位置補正
    /// </summary>
    public override Vector2 correctionVector
    {
        get {
            return base.correctionVector + nowRecoil.rescaling(baseMas);
        }

        set {
            base.correctionVector = value;
        }
    }
}
