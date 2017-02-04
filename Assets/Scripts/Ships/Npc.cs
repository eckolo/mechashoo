using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System;

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
            }
            else if(nowActionState == ActionPattern.NON_COMBAT)
            {
                nowActionState = initialActionState;
            }
        }
    }

    protected override bool forcedScreen
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
    protected enum ActionPattern
    {
        NON_COMBAT,
        MOVE,
        AIMING,
        ATTACK
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
    ///機体性能の基準値
    /// </summary>
    public ulong shipLevel = 1;

    /// <summary>
    ///撃破時の獲得得点
    /// </summary>
    public int points = 0;

    public override void Start()
    {
        invertWidth(false);
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if(inField) action(nextActionIndex);
    }

    public override bool action(int? actionNum = null)
    {
        if(!timingSwich) return false;
        timingSwich = false;

        return base.action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        nextActionIndex = 0;
        isReaction = captureTarget(nowNearTarget);

        yield return base.baseMotion(actionNum);

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
            return base.siteSpeed + palamates.baseSiteSpeed * Mathf.Log(shipLevel);
        }
    }

    protected bool captureTarget(Things target, float? distance = null)
    {
        if(target == null) return false;
        return MathV.scaling(target.position - position, baseMas).magnitude <= (distance ?? reactionDistance);
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
}
