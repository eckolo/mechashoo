using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Nusepuleje : Kemi
{
    Weapon lance => allWeapons[0];
    Weapon grenade => allWeapons[1];
    Weapon assaulter => allWeapons[2];

    enum Motion
    {
        LANCE,
        SOMERSAULT,
        GRENADE,
        GRENADE_BURST,
        NAPALM,
        ASSAULTER,
        ASSAULTER_BURST
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        yield return headingDestination(bodyAimPosition, moderateSpeed, () => {
            aiming(nearTarget.position);
            setBaseAiming();
        });
        yield return stoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                Motion.LANCE,
                Motion.SOMERSAULT,
                Motion.GRENADE,
                Motion.GRENADE_BURST,
                Motion.NAPALM,
                Motion.ASSAULTER,
                Motion.ASSAULTER_BURST
            }.selectRandom(new[] { 1, 3, 1, 2, 3, 1, 2 }) :
            (int)new[] {
                Motion.LANCE,
                Motion.GRENADE,
                Motion.GRENADE_BURST,
                Motion.ASSAULTER
            }.selectRandom(new[] { 2, 2, 1, 2 });
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
        var motion = actionNum.normalize<Motion>();
        var distination = Vector2.zero;
        switch(motion)
        {
            case Motion.LANCE:
                distination = nearTarget.position - Vector2.right * spriteSize.x * targetSign;
                setFixedAlignment(nearTarget.position);
                yield return headingDestination(distination, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.down;
                    aiming(nearTarget.position + tweak, 0);
                });
                yield return stoppingAction();
                break;
            case Motion.SOMERSAULT:
                distination = nearTarget.position + Vector2.right * viewSize.x / 2 * targetSign;
                yield return headingDestination(distination, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    resetAllAim(2);
                });
                yield return stoppingAction();
                break;
            case Motion.GRENADE:
            case Motion.ASSAULTER:
            case Motion.ASSAULTER_BURST:
                if(motion != Motion.ASSAULTER_BURST) setFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                yield return headingDestination(bodyAimPosition, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    setBaseAiming();
                });
                yield return stoppingAction();
                break;
            case Motion.NAPALM:
                distination = nearTarget.position - Vector2.right * spriteSize.x * 2 * targetSign;
                yield return headingDestination(distination, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    setBaseAiming();
                });
                yield return stoppingAction();
                break;
            case Motion.GRENADE_BURST:
                distination = bodyAimPosition
                    + Vector2.up * maximumSpeed * interval * new[] { 1, -1 }.selectRandom();
                yield return headingDestination(distination, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    setBaseAiming();
                });
                yield return stoppingAction();
                break;
            default:
                break;
        }
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
        var motion = actionNum.normalize<Motion>();
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        var limit = 1f;

        switch(motion)
        {
            case Motion.LANCE:
                yield return wait(() => lance.canAction);
                lance.action(Weapon.ActionType.NOMAL);
                yield return wait(() => lance.canAction);
                break;
            case Motion.SOMERSAULT:
                yield return wait(() => lance.canAction);
                var approachDistance = Vector2.right * viewSize.x / 2 * targetSign;
                var distination = nearTarget.position - approachDistance;
                setFixedAlignment(nearTarget.position);
                lance.action(Weapon.ActionType.SINK);
                yield return headingDestination(distination, maximumSpeed * 1.5f, () => {
                    aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.up;
                    aiming(nearTarget.position + tweak, 0);
                });
                yield return stoppingAction();
                yield return wait(() => lance.canAction);
                break;
            case Motion.GRENADE:
                yield return wait(() => grenade.canAction);
                grenade.action(Weapon.ActionType.NOMAL);
                yield return wait(() => grenade.canAction);
                break;
            case Motion.GRENADE_BURST:
                limit = Random.Range(1, shipLevel) * 5 * interval;
                for(int time = 0; time < limit; time++)
                {
                    if(grenade.canAction) setFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                    thrust(bodyAimPosition - position, reactPower, maximumSpeed);
                    aiming(nearTarget.position);
                    setBaseAiming();
                    grenade.action(Weapon.ActionType.NOMAL, 0.5f);
                    yield return wait(1);
                }
                yield return stoppingAction();
                break;
            case Motion.NAPALM:
                setFixedAlignment(new Vector2(3, bodyWeaponRoot.y), true);
                yield return wait(() => grenade.canAction);
                grenade.action(Weapon.ActionType.SINK);
                for(int time = 0; time < interval; time++)
                {
                    thrust(nowForward * -1, reactPower, moderateSpeed);
                    aiming(nearTarget.position);
                    setBaseAiming();
                    yield return wait(1);
                }
                yield return stoppingAction();
                break;
            case Motion.ASSAULTER:
                yield return wait(() => assaulter.canAction);
                var diff = (nearTarget.position - position).magnitude;
                if(seriousMode && diff < gunDistance) assaulter.action(Weapon.ActionType.SINK);
                else assaulter.action(Weapon.ActionType.NOMAL);
                yield return wait(() => assaulter.canAction);
                break;
            case Motion.ASSAULTER_BURST:
                var direction = new[] { 90f, -90f }.selectRandom();
                limit = Random.Range(2, shipLevel) * 5 * interval;
                for(int time = 0; time < limit || !assaulter.canAction; time++)
                {
                    if(assaulter.canAction) setFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                    var tweak = direction.toRotation() * (position - nearTarget.position);
                    var destination = nearTarget.position + (Vector2)tweak;
                    thrust(destination - position, reactPower, maximumSpeed);
                    aiming(nearTarget.position);
                    setBaseAiming(2);
                    if(time < limit) assaulter.action(Weapon.ActionType.NOMAL, 0.8f);
                    yield return wait(1);
                }
                setFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                assaulter.action(Weapon.ActionType.SINK);
                yield return stoppingAction();
                break;
            default:
                break;
        }
        for(int time = 0; time < interval; time++)
        {
            aiming(nearTarget.position);
            setBaseAiming(2);
            thrustStop();
            yield return wait(1);
        }
        yield break;
    }
    protected override float properDistance => base.properDistance * 2;
}
