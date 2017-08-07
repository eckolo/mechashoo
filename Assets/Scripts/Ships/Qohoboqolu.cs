using UnityEngine;
using System.Collections;
using System.Linq;

public class Qohoboqolu : Npc
{
    Weapon grenade => allWeapons[0];
    Weapon rifle => allWeapons[1];

    enum MotionType
    {
        GRENADE,
        RIFLE,
        RIFLE_BURST
    }

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
    /// 非反応時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionNonCombat(int actionNum)
    {
        yield return NomalMoving((maximumSpeed + lowerSpeed) / 2);
        yield break;
    }
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var wraps = Random.Range(0, Mathf.Max(6 - (int)shipLevel, 0)) + 2 + onTheWay.ToInt();
        var baseSpeed = normalCourse;
        var initialDirection = new[] { 1, -1 }.SelectRandom();
        for(int wrap = 0; wrap < wraps && inField; wrap++)
        {
            var nowAngle = (baseSpeed.magnitude > 0 ? baseSpeed : siteAlignment).ToAngle();
            var correctAngle = !onTheWay
                ? Random.Range(90f, 270f)
                : Random.Range(45f, 75f) * initialDirection * (wrap % 2 == 0).ToSign();
            var direction = (nowAngle + correctAngle).Compile();
            var targetPosition = nearTarget.position;
            yield return AimingAction(targetPosition,
                interval,
                aimingProcess: () => {
                    Thrust(direction.ToVector(), reactPower, maximumSpeed);
                    Aiming(standardAimPosition, 0);
                    Aiming(targetPosition, 1);
                });
            if(onTheWay)
            {
                yield return StoppingAction();
                yield return Wait(() => rifle.canAction);
                rifle.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => rifle.canAction);
            }
            else baseSpeed = nowSpeed;
        }
        normalCourse = baseSpeed;
        nextActionIndex = (int)new[] {
            MotionType.GRENADE,
            MotionType.RIFLE,
            MotionType.RIFLE_BURST
        }.SelectRandom(new[] { 1, 3, 1 });
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
        switch(motion)
        {
            case MotionType.GRENADE:
                {
                    var targetPosition = nearTarget.position;
                    yield return AimingAction(() => targetPosition,
                        armIndex: 0,
                        aimingProcess: () => {
                            Thrust(!onTheWay ? position - nearTarget.position : normalCourse, reactPower, lowerSpeed);
                            Aiming(targetPosition);
                            Aiming(standardAimPosition, 1);
                        });
                    yield return StoppingAction();
                }
                break;
            case MotionType.RIFLE:
                {
                    var targetPosition = nearTarget.position;
                    yield return AimingAction(() => targetPosition,
                        armIndex: 1,
                        aimingProcess: () => {
                            Thrust(!onTheWay ? nearTarget.position - position : normalCourse, reactPower, lowerSpeed);
                            Aiming(targetPosition);
                            Aiming(standardAimPosition, 0);
                        });
                    yield return StoppingAction();
                }
                break;
            case MotionType.RIFLE_BURST:
                {
                    var targetPosition = nearTarget.position - Vector2.down * grappleDistance;
                    yield return AimingAction(() => targetPosition,
                        armIndex: 1,
                        aimingProcess: () => {
                            Thrust(!onTheWay ? nearTarget.position - position : normalCourse, reactPower, lowerSpeed);
                            Aiming(targetPosition);
                            Aiming(standardAimPosition, 0);
                        });
                    yield return StoppingAction();
                }
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
        switch(motion)
        {
            case MotionType.GRENADE:
                {
                    SetFixedAlignment(1);
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.NOMAL);
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.RIFLE:
                {
                    SetFixedAlignment(1);
                    yield return Wait(() => rifle.canAction);
                    rifle.Action(Weapon.ActionType.NOMAL);
                    yield return Wait(() => rifle.canAction);
                }
                break;
            case MotionType.RIFLE_BURST:
                {
                    SetFixedAlignment(1);
                    yield return Wait(() => rifle.canAction);
                    rifle.Action(Weapon.ActionType.SINK);
                    yield return Wait(() => rifle.onAttack);
                    while(rifle.onAttack)
                    {
                        yield return AimingAction(() => nearTarget.position, armIndex: 1);
                        yield return Wait(1);
                    }
                    yield return Wait(() => rifle.canAction);
                }
                break;
            default:
                break;
        }
        for(int time = 0; time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
    /// <summary>
    /// 逃走時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionEscape(int actionNum)
    {
        yield return NomalMoving(maximumSpeed);
        yield break;
    }

    IEnumerator NomalMoving(float speed)
    {
        var direction = Random.Range(-10f, 10f).ToRotation() * normalCourse;
        for(int time = 0; time < interval; time++)
        {
            Thrust(direction, reactPower, speed);
            Aiming(position + baseAimPosition);
            yield return Wait(1);
        }
        yield return StoppingAction();
        yield break;
    }
}
