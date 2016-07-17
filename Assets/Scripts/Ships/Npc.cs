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
    protected float reactionDistance = 2400;
    /// <summary>
    ///現在のモーションを示す番号
    /// </summary>
    public int nowActionNum = 0;
    /// <summary>
    ///行動選択肢の最大数
    /// </summary>
    protected int maxActionChoices = 1;
    /// <summary>
    ///モーションの切り替わりタイミングフラグ
    /// </summary>
    protected bool timingSwich = true;

    /// <summary>
    ///機体性能の基準値
    /// </summary>
    public ulong shipLevel = 1;

    /// <summary>
    ///撃破時の獲得得点
    /// </summary>
    public int points = 0;

    public override void Update()
    {
        base.Update();
        if (inScreen()) Action(nowActionNum);
    }

    protected virtual int setNextMotion(int actionNum)
    {
        if (actionNum != 0) return actionNum;
        return Random.Range(0, maxActionChoices + 1);
    }

    public override bool Action(int? actionNum = null)
    {
        if (!timingSwich) return false;
        timingSwich = false;

        return base.Action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        yield return base.baseMotion(actionNum);

        nowActionNum = setNextMotion(actionNum);
        timingSwich = true;

        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        if (actionNum != 0) setVerosity(Vector2.left, 1);
        yield break;
    }

    protected override void onDestroyAction(bool fromPlayer)
    {
        if (fromPlayer) getSystem().nowStage.points += points;
    }

    protected bool captureTarget(Material target, float? distance = null)
    {
        return (target.transform.position - transform.position).magnitude <= (distance ?? reactionDistance) / getPixel();
    }
}
