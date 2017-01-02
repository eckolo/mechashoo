using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// NPC機体の制御クラス
/// </summary>
public class Npc : Ship
{
    /// <summary>
    ///反応距離
    /// </summary>
    [SerializeField]
    private float _reactionDistance = 240;
    protected float reactionDistance
    {
        get
        {
            return nowActionState != ActionPattern.NON_COMBAT
                ? _reactionDistance * 2
                : _reactionDistance;
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
        if(inField) action((int)nowActionState);
    }

    public override bool action(int? actionNum = null)
    {
        if(!timingSwich) return false;
        timingSwich = false;

        return base.action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        yield return base.baseMotion(actionNum);

        preActionState = nowActionState;
        nowActionState = nextActionState;
        timingSwich = true;

        yield break;
    }

    protected override IEnumerator motion(int actionNum)
    {
        yield return motion(Enums<ActionPattern>.normalize(actionNum));
        yield break;
    }
    protected virtual IEnumerator motion(ActionPattern actionNum)
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
        get
        {
            return base.siteSpeed + palamates.baseSiteSpeed * shipLevel;
        }
    }

    protected bool captureTarget(Things target, float? distance = null)
    {
        if(target == null) return false;
        return MathV.scaling(target.position - position, baseMas).magnitude <= (distance ?? reactionDistance);
    }
}
