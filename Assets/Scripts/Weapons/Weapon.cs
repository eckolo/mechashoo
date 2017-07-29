using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Events;

/// <summary>
/// 武装クラス
/// </summary>
public partial class Weapon : Parts
{
    /// <summary>
    /// 持ち手の座標
    /// </summary>
    public Vector2 handlePosition = Vector2.zero;
    /// <summary>
    /// 持ち手の奥行き座標
    /// </summary>
    public float handledZ = 1;
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
    public Bullet GetBullet(Injection injection) => injection.bullet ?? defBullet;

    /// <summary>
    /// 1モーションの所要時間
    /// </summary>
    [SerializeField]
    private int _timeRequired = 1;
    /// <summary>
    /// 1モーションの所要時間
    /// </summary>
    protected virtual int timeRequired => _timeRequired;
    /// <summary>
    /// アクション毎の間隔
    /// </summary>
    [SerializeField]
    private int actionDelay = 1;
    protected int actionDelaySum => Mathf.CeilToInt(actionDelay * delayTweak);
    /// <summary>
    /// 弾丸密度
    /// </summary>
    protected virtual int density => _density;
    /// <summary>
    /// 弾丸密度
    /// </summary>
    [SerializeField]
    protected int _density = 1;
    /// <summary>
    /// デフォルトの向き
    /// </summary>
    [SerializeField]
    public float defAngle = 0;
    /// <summary>
    /// 外部由来の基礎角度
    /// </summary>
    public float baseAngle { get; set; } = 0;
    /// <summary>
    /// Handに対しての描画順の前後のデフォルト値
    /// </summary>
    [SerializeField]
    public int defaultZ = -1;
    /// <summary>
    /// 起動時燃料消費量基準値
    /// </summary>
    [SerializeField]
    public float motionFuelCost = 1;
    /// <summary>
    /// 射出時燃料消費量基準値
    /// </summary>
    [SerializeField]
    public float injectionFuelCost = 1;
    /// <summary>
    /// 弾ブレ度合い
    /// </summary>
    [SerializeField]
    protected float noAccuracy = 0;

    /// <summary>
    /// チャージエフェクト
    /// </summary>
    [SerializeField]
    protected Effect chargeEffect = null;

    /// <summary>
    /// 武装の使用者
    /// </summary>
    protected Ship user = null;

    /// <summary>
    /// 1アクション毎の燃料消費量最終値
    /// </summary>
    protected virtual float motionFuelCostSum
        => motionFuelCost * onTypeInjections?.Max(injection => injection.fuelCostPar) ?? 0;

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

        SetAngle(defAngle);
        user = nowRoot?.GetComponent<Ship>();
    }

    public override void Update()
    {
        base.Update();
        if(inAction)
        {
            nowColor = new Color(1f, 0.6f, 0.8f, 1);
        }
        else
        {
            nowColor = new Color(1f, 1f, 1f, 1);
        }
    }

    protected override void UpdateMotion()
    {
        base.UpdateMotion();
        UpdateRecoil();
    }

    public override float SetAngle(float settedAngle)
    {
        return base.SetAngle(baseAngle + settedAngle);
    }

    public override Vector2 nowLengthVector
    {
        get {
            var maxLength = nowInjections.Max(injection => injection.hole.magnitude);
            var longestHole = nowInjections
                .Where(injection => injection.hole.magnitude == maxLength)
                .Select(injection => injection.hole)
                .FirstOrDefault();
            return longestHole - handlePosition;
        }
    }

    /// <summary>
    /// 現在攻撃動作可能かどうかの判定フラグ
    /// </summary>
    public bool canAction
    {
        get {
            return !inAction && _canAction;
        }
        set {
            _canAction = value;
            if(!value) nextAction = ActionType.NOMOTION;
        }
    }
    bool _canAction = true;
    /// <summary>
    /// 現在攻撃動作中か否かの判定
    /// </summary>
    public virtual bool onAttack
    {
        get {
            return _onAttack;
        }
        protected set {
            _onAttack = value;
        }
    }
    bool _onAttack = false;

    /// <summary>
    /// 動作中フラグ
    /// </summary>
    public bool inAction
    {
        get {
            if(nowRoot == null) return _inAction;
            if(nowRoot.GetComponent<Weapon>() == null) return _inAction;
            return nowRoot.GetComponent<Weapon>().inAction;
        }
    }
    bool _inAction = false;

    /// <summary>
    /// 武装動作の一時停止フラグ
    /// </summary>
    public bool motionAccumulating { get; set; } = false;

    public enum ActionType
    {
        NOMOTION,
        NOMAL,
        SINK,
        FIXED,
        NPC
    }
    public bool Action(ActionType actionType = ActionType.NOMAL, float setActionDelayTweak = 1, int actionNum = 0)
    {
        if(!canAction || actionType == ActionType.NOMOTION)
        {
            nextAction = actionType;
            return false;
        }

        _inAction = true;
        nowAction = actionType;
        delayTweak = setActionDelayTweak;
        return base.Action(actionNum);
    }
    protected override IEnumerator BaseMotion(int actionNum)
    {
        bool normalOperation = ReduceShipFuel(motionFuelCostSum);

        if(normalOperation)
        {
            if(motionFuelCost > 0 && user?.GetComponent<Player>() != null) sys.CountAttackCount();
            yield return BeginMotion(actionNum);
            yield return Wait(() => !motionAccumulating);
            onAttack = true;
            yield return base.BaseMotion(actionNum);
        }
        onAttack = false;

        var timerKey = "weaponEndMotion";
        timer.Start(timerKey);
        if(normalOperation) yield return EndMotion(actionNum);
        if(actionDelaySum > 0) yield return Wait(actionDelaySum - timer.Get(timerKey));
        timer.Stop(timerKey);

        _inAction = false;
        if(nextAction != ActionType.NOMOTION)
        {
            Action(nextAction, delayTweak, actionNum);
            nextAction = ActionType.NOMOTION;
        }

        yield break;
    }

    protected virtual IEnumerator BeginMotion(int actionNum)
    {
        yield break;
    }
    protected override IEnumerator Motion(int actionNum)
    {
        yield break;
    }
    protected virtual IEnumerator EndMotion(int actionNum)
    {
        yield break;
    }

    protected bool ReduceShipFuel(float reduceValue, float fuelCorrection = 1)
    {
        Ship rootShip = nowRoot.GetComponent<Ship>();
        if(rootShip == null) return true;
        return rootShip.ReduceFuel(reduceValue * fuelCorrection);
    }

    /// <summary>
    /// 弾の作成
    /// 武装毎の射出孔番号で指定するタイプ
    /// </summary>
    protected virtual Bullet Inject(Injection injection, float fuelCorrection = 1, float angleCorrection = 0)
    {
        if(injection == null) return null;

        var confirmBullet = GetBullet(injection);
        if(confirmBullet == null) return null;

        if(!ReduceShipFuel(injectionFuelCost * injection.fuelCostPar, fuelCorrection)) return confirmBullet;

        if(injectionFuelCost > 0 && user?.GetComponent<Player>() != null) sys.CountAttackCount();
        var forwardAngle = injection.angle + angleCorrection;

        SoundSE(injection.se);
        var bullet = Inject(confirmBullet, injection.hole, forwardAngle + injection.bulletAngle);
        if(bullet == null) return bullet;

        bullet.user = user;
        bullet.userWeapon = this;
        var shake = Mathf.Max(noAccuracy + injection.noAccuracy, 0).ToMildRandom();
        var forward = shake.ToRotation() * nowForward;
        bullet.SetVerosity(forwardAngle.ToRotation() * forward * injection.initialVelocity);
        if(injection.union) bullet.nowParent = transform;

        return bullet;
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
    protected IEnumerator SwingAction(Vector2 endPosition,
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

            correctionVector = MathV.EasingV.Elliptical(startPosition, endPosition * radiusCriteria, localTime, limit, clockwise);
            midstreamProcess?.Invoke(time, localTime, limit);
            yield return Wait(1);
        }
        yield break;
    }

    /// <summary>
    /// 反動開始関数
    /// </summary>
    /// <param name="injection">反動発生元発射孔の情報</param>
    /// <param name="_recoilRate">反動量係数</param>
    protected void SetRecoil(Injection injection, float _recoilRate = 1)
    {
        var injectBullet = GetBullet(injection);
        var recoilPower = _recoilRate * injection.initialVelocity;
        var setedRecoil = (injection.angle + 180).ToVector(recoilPower) * injectBullet.weight;

        var ship = nowParent.GetComponent<Ship>()
            ?? nowParent.GetComponent<WeaponBase>()?.nowParent.GetComponent<Ship>();
        if(ship != null)
        {
            var direction = GetWidthRealRotation(GetLossyRotation() * (lossyScale.y.ToSign() * injection.angle).ToRotation()) * Vector2.left;
            ship.ExertPower(direction, Mathf.Log(setedRecoil.magnitude + 1, 2) * baseMas.magnitude);
        }
        else
        {
            var recoil = Vector2.right * Mathf.Log(Mathf.Abs(setedRecoil.x) + 1) * setedRecoil.x.ToSign() + Vector2.up * Mathf.Log(Mathf.Abs(setedRecoil.y) + 1) * setedRecoil.y.ToSign();
            SetRecoil(recoil);
        }
    }
    /// <summary>
    /// 反動開始関数
    /// </summary>
    /// <param name="hangForce">反動量</param>
    protected void SetRecoil(Vector2 hangForce)
    {
        Vector2 degree = hangForce - recoilSpeed;
        var length = degree.magnitude;
        float variation = length != 0 ? Mathf.Clamp(hangForce.magnitude / length, -1, 1) : 0;

        recoilSpeed = recoilSpeed + degree * variation;
    }
    /// <summary>
    /// 反動量の逐次計算実行関数
    /// </summary>
    void UpdateRecoil()
    {
        nowRecoil += recoilSpeed;
        if(nowRecoil.magnitude == 0) recoilSpeed = Vector2.zero;
        else if(nowRecoil.magnitude < tokenHand.power) recoilSpeed = -nowRecoil;
        else SetRecoil(-nowRecoil.ToVector(tokenHand.power));
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
            return base.correctionVector + nowRecoil.Rescaling(baseMas);
        }
        set {
            base.correctionVector = value;
        }
    }
}
