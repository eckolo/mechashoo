using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Nusepuleje : Guhabaji
{
    Weapon lance => allWeapons[0];
    Weapon grenade => allWeapons[1];
    Weapon assaulter => allWeapons[2];

    enum MotionType
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
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        yield return HeadingDestination(bodyAimPosition, moderateSpeed, () => {
            Aiming(nearTarget.position);
            SetBaseAiming();
        });
        yield return StoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                MotionType.LANCE,
                MotionType.SOMERSAULT,
                MotionType.GRENADE,
                MotionType.GRENADE_BURST,
                MotionType.NAPALM,
                MotionType.ASSAULTER,
                MotionType.ASSAULTER_BURST
            }.SelectRandom(new[] { 1, 3, 1, 2, 3, 1, 2 }) :
            (int)new[] {
                MotionType.LANCE,
                MotionType.GRENADE,
                MotionType.GRENADE_BURST,
                MotionType.ASSAULTER
            }.SelectRandom(new[] { 2, 2, 1, 2 });
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
        var motion = actionNum.Normalize<MotionType>();
        var distination = Vector2.zero;
        switch(motion)
        {
            case MotionType.LANCE:
                distination = nearTarget.position - Vector2.right * spriteSize.x * targetSign;
                SetFixedAlignment(nearTarget.position);
                yield return HeadingDestination(distination, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.down;
                    Aiming(nearTarget.position + tweak, 0);
                });
                yield return StoppingAction();
                break;
            case MotionType.SOMERSAULT:
                distination = nearTarget.position + Vector2.right * viewSize.x / 2 * targetSign;
                yield return HeadingDestination(distination, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    ResetAllAim(2);
                });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE:
            case MotionType.ASSAULTER:
            case MotionType.ASSAULTER_BURST:
                if(motion != MotionType.ASSAULTER_BURST) SetFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                yield return HeadingDestination(bodyAimPosition, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
                break;
            case MotionType.NAPALM:
                distination = nearTarget.position - Vector2.right * spriteSize.x * 2 * targetSign;
                yield return HeadingDestination(distination, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE_BURST:
                distination = bodyAimPosition
                    + Vector2.up * maximumSpeed * interval * new[] { 1, -1 }.SelectRandom();
                yield return HeadingDestination(distination, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
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
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<MotionType>();
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        var limit = 1f;

        switch(motion)
        {
            case MotionType.LANCE:
                yield return Wait(() => lance.canAction);
                lance.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => lance.canAction);
                break;
            case MotionType.SOMERSAULT:
                yield return Wait(() => lance.canAction);
                var approachDistance = Vector2.right * viewSize.x / 2 * targetSign;
                var distination = nearTarget.position - approachDistance;
                SetFixedAlignment(nearTarget.position);
                lance.Action(Weapon.ActionType.SINK);
                yield return HeadingDestination(distination, maximumSpeed * 1.5f, () => {
                    Aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.up;
                    Aiming(nearTarget.position + tweak, 0);
                });
                yield return StoppingAction();
                yield return Wait(() => lance.canAction);
                break;
            case MotionType.GRENADE:
                yield return Wait(() => grenade.canAction);
                grenade.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => grenade.canAction);
                break;
            case MotionType.GRENADE_BURST:
                limit = Random.Range(1, shipLevel) * 5 * interval;
                for(int time = 0; time < limit; time++)
                {
                    if(grenade.canAction) SetFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                    Thrust(bodyAimPosition - position, reactPower, maximumSpeed);
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                    grenade.Action(Weapon.ActionType.NOMAL, 0.5f);
                    yield return Wait(1);
                }
                yield return StoppingAction();
                break;
            case MotionType.NAPALM:
                SetFixedAlignment(new Vector2(3, bodyWeaponRoot.y), true);
                yield return Wait(() => grenade.canAction);
                grenade.Action(Weapon.ActionType.SINK);
                for(int time = 0; time < interval; time++)
                {
                    Thrust(nowForward * -1, reactPower, moderateSpeed);
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                    yield return Wait(1);
                }
                yield return StoppingAction();
                break;
            case MotionType.ASSAULTER:
                yield return Wait(() => assaulter.canAction);
                var diff = (nearTarget.position - position).magnitude;
                if(seriousMode && diff < gunDistance) assaulter.Action(Weapon.ActionType.SINK);
                else assaulter.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => assaulter.canAction);
                break;
            case MotionType.ASSAULTER_BURST:
                var direction = new[] { 90f, -90f }.SelectRandom();
                limit = Random.Range(2, shipLevel) * 5 * interval;
                for(int time = 0; time < limit || !assaulter.canAction; time++)
                {
                    if(assaulter.canAction) SetFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                    var tweak = direction.ToRotation() * (position - nearTarget.position);
                    var destination = nearTarget.position + (Vector2)tweak;
                    Thrust(destination - position, reactPower, maximumSpeed);
                    Aiming(nearTarget.position);
                    SetBaseAiming(2);
                    if(time < limit) assaulter.Action(Weapon.ActionType.NOMAL, 0.8f);
                    yield return Wait(1);
                }
                SetFixedAlignment(new Vector2(gunDistance, bodyWeaponRoot.y), true);
                assaulter.Action(Weapon.ActionType.SINK);
                yield return StoppingAction();
                break;
            default:
                break;
        }
        for(int time = 0; time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAiming(2);
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
    protected override float properDistance => base.properDistance * 2;
}
