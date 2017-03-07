using UnityEngine;
using System.Collections;

public class Luwucijiqi : Npc
{
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var wraps = Random.Range(2, 3 + (int)shipLevel);

        for(int wrap = 0; wrap < wraps; wrap++)
        {
            var nowAngle = (nowSpeed.magnitude > 0 ? nowSpeed : siteAlignment)
                .correct(position - nearTarget.position, 0.8f)
                .toAngle();
            var direction = nowAngle + Random.Range(90, 270);
            yield return aimingAction(nearTarget.position,
                interval,
                () => exertPower(direction, reactPower, maximumSpeed));
        }
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
        nextActionIndex = (nearTarget.position.magnitude < arms[1].tipLength).toInt();
        yield return aimingAction(() => nearTarget.position,
            interval,
            () => thrust(nearTarget.position - position, reactPower, lowerSpeed));
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

        int armNum = (siteAlignment.magnitude > arms[1].tipLength).toInt();
        arms[armNum].tipHand.actionWeapon(Weapon.ActionType.NOMAL);

        yield return stoppingAction();
        yield return wait(() => arms[armNum].tipHand.takeWeapon.canAction);
        yield break;
    }
}
