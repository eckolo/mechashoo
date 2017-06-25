using UnityEngine;
using System.Collections;

/// <summary>
/// ローラーシップクラス
/// </summary>
public class Xewusigume : Npc
{
    /// <summary>
    /// 画面内に位置強制するフラグ
    /// </summary>
    protected override bool forcedInScreen
    {
        get {
            if(onTheWay && normalCourse.normalized == nowSpeed.normalized) return false;
            return base.forcedInScreen;
        }
    }

    /// <summary>
    /// 初回行動フラグ
    /// </summary>
    bool first = true;
    /// <summary>
    /// 通りすがりモードフラグ
    /// </summary>
    public override bool onTheWay
    {
        get {
            if(first) return false;
            return base.onTheWay;
        }
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var direction = !onTheWay ? nowForward : normalCourse;
        yield return AimingAction(nearTarget.position, interval * 2, aimingProcess: () => Thrust(direction, reactPower, maximumSpeed));
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        yield return AimingAction(nearTarget.position, interval, aimingProcess: () => Thrust(!onTheWay ? nowForward : normalCourse, reactPower, lowerSpeed));
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        if(!inField) yield break;
        SetFixedAlignment(position + siteAlignment);
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.Action();
        }
        for(var time = 0; time < interval; time++)
        {
            ThrustStop();
            yield return Wait(1);
        }
        first = false;
        yield break;
    }
}