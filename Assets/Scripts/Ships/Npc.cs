using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Linq;

/// <summary>
/// NPC機体の制御クラス
/// </summary>
public class Npc : Ship
{
    /// <summary>
    /// 反応距離
    /// </summary>
    [SerializeField]
    private float _reactionDistance = 240;
    /// <summary>
    /// 装甲補正値
    /// </summary>
    [SerializeField]
    private float armorCorrectionRate = 0.5f;
    /// <summary>
    /// 障壁補正値
    /// </summary>
    [SerializeField]
    private float barrierCorrectionRate = 0.5f;
    /// <summary>
    ///行動開始時のモーションを示す番号
    /// </summary>
    [SerializeField]
    private ActionPattern initialActionState = ActionPattern.NON_COMBAT;
    /// <summary>
    /// 専用BGM
    /// </summary>
    [SerializeField]
    public AudioClip privateBgm = null;

    /// <summary>
    /// 非戦闘時進路方向の角度指定
    /// </summary>
    public Vector2 normalCourse
    {
        get {
            return _normalCourse ?? nowForward;
        }
        set {
            _normalCourse = value;
        }
    }
    /// <summary>
    /// 非戦闘時進路方向の角度指定
    /// </summary>
    Vector2? _normalCourse = null;

    protected override Vector2 baseAimPosition
    {
        get {
            if(wings.Any(wing => wing.rollable)) return normalCourse;
            return base.baseAimPosition;
        }
    }

    /// <summary>
    /// 反応距離
    /// </summary>
    protected float reactionDistance
    {
        get {
            return isReaction ? _reactionDistance * 2 : _reactionDistance;
        }
    }
    /// <summary>
    /// 反応状態
    /// </summary>
    protected bool isReaction
    {
        get {
            return nowActionState != ActionPattern.NON_COMBAT;
        }
        set {
            if(!value)
            {
                nowActionState = ActionPattern.NON_COMBAT;
                nextActionState = ActionPattern.NON_COMBAT;
            }
            else if(nowActionState == ActionPattern.NON_COMBAT)
            {
                nowActionState = initialActionState;
            }
        }
    }

    protected override bool forcedInScreen
    {
        get {
            return isReaction;
        }
    }

    /// <summary>
    /// 最大装甲値
    /// </summary>
    protected override float maxArmor
    {
        get {
            return base.maxArmor * armorCorrectionRate;
        }
    }
    /// <summary>
    /// 最大障壁値
    /// </summary>
    protected override float maxBarrier
    {
        get {
            return base.maxBarrier * barrierCorrectionRate;
        }
    }

    /// <summary>
    ///現在のモーションを示す番号
    /// </summary>
    public enum ActionPattern
    {
        NON_COMBAT,
        MOVE,
        AIMING,
        ATTACK,
        ESCAPE
    };

    /// <summary>
    ///現在のモーションを示す番号
    /// </summary>
    protected ActionPattern nowActionState { get; private set; }
    /// <summary>
    ///現在のモーションを示す番号
    /// </summary>
    protected ActionPattern preActionState { get; private set; }
    /// <summary>
    ///次のモーション番号予約
    /// </summary>
    protected ActionPattern nextActionState { get; set; }
    /// <summary>
    ///次のモーションの詳細識別番号
    /// </summary>
    protected int nextActionIndex { get; set; } = 0;
    /// <summary>
    ///モーションの切り替わりタイミングフラグ
    /// </summary>
    bool timingSwich = true;

    /// <summary>
    /// 戦闘終了時限
    /// </summary>
    public int activityLimit { get; set; } = 0;
    const string NPC_TIMER_NAME = "NPC";

    /// <summary>
    ///機体性能の基準値
    /// </summary>
    public float shipLevel
    {
        get {
            return Mathf.Log(_shipLevel + 1, 2);
        }
        set {
            _shipLevel = Mathf.Max(value, 0);
        }
    }
    /// <summary>
    ///機体性能の基準値
    /// </summary>
    float _shipLevel = 1;

    /// <summary>
    ///撃破時の獲得得点
    /// </summary>
    public int points = 0;

    public override void Start()
    {
        invertWidth(false);
        invertWidth(normalCourse);
        base.Start();
        timer.start(NPC_TIMER_NAME);
    }

    public override void Update()
    {
        base.Update();
        action(nextActionIndex);
    }

    public override bool action(int? actionNum = null)
    {
        if(!timingSwich) return false;
        timingSwich = false;

        return base.action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        isReaction = inField && captureTarget(nowNearTarget);
        if(activityLimit > 0 && timer.get(NPC_TIMER_NAME) >= activityLimit) nowActionState = ActionPattern.ESCAPE;

        if(!isReaction || nowActionState == ActionPattern.ESCAPE)
        {
            var speed = isReaction ? maximumSpeed : (lowerSpeed + maximumSpeed) / 2;
            exertPower(normalCourse, reactPower, speed);
            aiming(position + baseAimPosition);
        }

        yield return base.baseMotion(actionNum);
        if(isReaction) normalCourse = nowSpeed.magnitude > 0 ? nowSpeed : siteAlignment;

        preActionState = nowActionState;
        nowActionState = nextActionState;
        timingSwich = true;

        yield break;
    }

    protected override IEnumerator motion(int actionNum)
    {
        if(actionNum != 0) setVerosity(Vector2.left, lowerSpeed);
        yield break;
    }

    public override void selfDestroy(bool system)
    {
        if(!system) sys.nowStage.points += points;
        base.selfDestroy();
    }
    protected override float siteSpeed
    {
        get {
            return base.siteSpeed + palamates.baseSiteSpeed * shipLevel;
        }
    }

    public override float receiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
        if(damage > 0) isReaction = true;
        return base.receiveDamage(damage, penetration, continuation);
    }

    protected bool captureTarget(Things target, float? distance = null)
    {
        if(target == null) return false;
        return (target.position - position).scaling(baseMas).magnitude <= (distance ?? reactionDistance);
    }

    protected IEnumerator aimingAction(Func<Vector2> destination, Func<bool> continueAimConditions, UnityAction aimingProcess = null)
    {
        while(continueAimConditions())
        {
            yield return wait(1);
            aiming(destination());
            aimingProcess?.Invoke();
        }

        yield break;
    }
    protected IEnumerator aimingAction(Func<Vector2> destination, UnityAction aimingProcess = null, float finishRange = 0)
    {
        finishRange = Mathf.Max(finishRange, 1);

        yield return aimingAction(destination,
            () => (destination() - (position + siteAlignment)).magnitude > finishRange / baseMas.magnitude,
            () => {
                aimingProcess?.Invoke();
                finishRange *= 1.01f;
            });

        yield break;
    }
    protected IEnumerator aimingAction(Func<Vector2> destination, int timelimit, UnityAction aimingProcess = null)
    {
        int time = 0;
        yield return aimingAction(destination, () => time++ < timelimit, aimingProcess);
        yield break;
    }
    protected IEnumerator aimingAction(Vector2 destination, Func<bool> continueAimConditions, UnityAction aimingProcess = null)
    {
        yield return aimingAction(() => destination, continueAimConditions, aimingProcess);
        yield break;
    }
    protected IEnumerator aimingAction(Vector2 destination, UnityAction aimingProcess = null, float finishRange = 0)
    {
        yield return aimingAction(() => destination, aimingProcess, finishRange);
        yield break;
    }
    protected IEnumerator aimingAction(Vector2 destination, int timelimit, UnityAction aimingProcess = null)
    {
        yield return aimingAction(() => destination, timelimit, aimingProcess);
        yield break;
    }
    protected Vector2 aiming(Vector2 destination, float siteSpeedCorrection = 1)
    {
        var degree = destination - (position + siteAlignment);

        siteAlignment = degree.magnitude < siteSpeed
            ? destination - position
            : siteAlignment + degree.normalized * siteSpeed;
        invertWidth(siteAlignment.x);
        return siteAlignment;
    }
    protected IEnumerable headingDestination(Vector2 destination, float headingSpeed)
    {
        while((destination - position).magnitude > nowSpeed.magnitude)
        {
            exertPower(destination - position, reactPower, headingSpeed);
            yield return wait(1);
        }
    }

    /// <summary>
    /// 偏差射撃の目標地点計算
    /// </summary>
    /// <typeparam name="Target">偏差射撃対象のクラス</typeparam>
    /// <param name="target">偏差射撃対象</param>
    /// <returns></returns>
    protected Vector2 getDeviationTarget<Target>(Target target, float intensity = 1)
        where Target : Things
        => target.position + target.nowSpeed * Mathf.Log((target.position - position).scaling(baseMas).magnitude + 2, 2) * intensity;

    /// <summary>
    /// 最適距離
    /// </summary>
    protected virtual float properDistance
    {
        get {
            return arms.Max(arm => arm.tipLength) * 2;
        }
    }
    /// <summary>
    /// 最適距離を維持するための移動目標地点の相対座標計算
    /// </summary>
    /// <param name="target">最適距離を維持したい対象</param>
    /// <param name="angleCorrection">目標と自身の直線状からの角度的なずれ（度）</param>
    /// <returns></returns>
    protected Vector2 getProperPosition(Things target, float angleCorrection = 0)
        => getProperPosition(target.position - position, angleCorrection);
    /// <summary>
    /// 最適距離を維持するための移動目標地点の相対座標計算
    /// </summary>
    /// <param name="direction">目的地の相対座標</param>
    /// <param name="angleCorrection">目的地と自身の直線状からの角度的なずれ（度）</param>
    /// <returns></returns>
    protected Vector2 getProperPosition(Vector2 direction, float angleCorrection = 0)
    {
        var difference = -direction.recalculation(properDistance);
        var rotate = angleCorrection.toRotation();

        return direction + (Vector2)(rotate * difference);
    }
}
