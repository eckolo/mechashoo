using UnityEngine;
using System.Collections;

/// <summary>
/// ローラーシップクラス
/// </summary>
public class Xewusigume : Npc
{
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var direction = nowForward;
        yield return aimingAction(nearTarget.position, interval * 2, () => exertPower(direction, reactPower, maximumSpeed));
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        yield return aimingAction(nearTarget.position, () => exertPower(nowForward, reactPower, lowerSpeed), 1);
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.action();
        }
        for(var time = 0; time < interval; time++)
        {
            stopping();
            yield return wait(1);
        }
        yield break;
    }
}