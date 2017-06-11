using UnityEngine;
using System.Collections;
using System.Linq;

public class Sejiziuequje : Npc
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
        yield return aimingAction(nearTarget.position, interval * 2, aimingProcess: () => thrust(direction, reactPower, maximumSpeed));
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
        yield return aimingAction(nearTarget.position, interval, aimingProcess: () => thrust(nowForward, reactPower, lowerSpeed));
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
        if(!inField) yield break;
        setFixedAlignment(position + siteAlignment);
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.action();
        }
        for(var time = 0; time < interval; time++)
        {
            thrustStop();
            yield return wait(1);
        }
        yield break;
    }
}
