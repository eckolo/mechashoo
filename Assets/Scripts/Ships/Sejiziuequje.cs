using UnityEngine;
using System.Collections;
using System.Linq;

public class Sejiziuequje : Boss
{
    public override void Update()
    {
        base.Update();
        foreach(var weaponBase in weaponBases) weaponBase.nowColor = nowColor;
    }

    /// <summary>
    /// 変形状態フラグ
    /// </summary>
    public bool transformation { get; set; } = false;

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var direction = nowForward;
        yield return AimingAction(nearTarget.position, interval * 2, aimingProcess: () => Thrust(direction, reactPower, maximumSpeed));
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        yield return AimingAction(nearTarget.position, interval, aimingProcess: () => Thrust(nowForward, reactPower, lowerSpeed));
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
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
        yield break;
    }
    /// <summary>
    /// 常に行われる攻撃行動
    /// </summary>
    private void AlwaysAttack()
    {
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.Action();
        }
    }
}
