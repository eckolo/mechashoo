using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public partial class Sejiziuequje : Boss
{
    class HandControler
    {
        AllLange hand = null;
        Ship myShip => hand?.myShip;
        IEnumerator mainMotion { get; set; } = null;

        public HandControler(AllLange _hand)
        {
            hand = _hand;
        }

        public bool isStandby { get; set; } = false;

        /// <summary>
        /// 現在のモーションを示す番号
        /// </summary>
        protected ActionPattern nowActionState { get; set; } = ActionPattern.NON_COMBAT;
        /// <summary>
        /// 次のモーションの詳細識別番号
        /// </summary>
        protected HandMotionType nowMotionType { get; set; } = 0;

        public void DestroyMyself()
        {
            hand.myShip.isAlive = false;
        }

        public Coroutine BeginMotion(Sejiziuequje body)
        {
            if(hand == null) return null;
            hand.remote = true;
            if(nowActionState == ActionPattern.NON_COMBAT) nowActionState = ActionPattern.MOVE;
            return mainMotion == null ? body.StartCoroutine(Motion(body)) : null;
        }
        public void PauseMotion(Sejiziuequje body)
        {
            nowActionState = ActionPattern.NON_COMBAT;
        }
        public void EndMotion(Sejiziuequje body)
        {
            nowActionState = ActionPattern.NON_COMBAT;
            body.StopCoroutine(mainMotion);
            mainMotion = null;
        }
        public void SetMotionType(HandMotionType motionType)
        {
            nowActionState = ActionPattern.AIMING;
            nowMotionType = motionType;
        }
        IEnumerator Motion(Sejiziuequje body)
        {
            do
            {
                yield return Wait(() => myShip != null);
                switch(nowActionState)
                {
                    case ActionPattern.NON_COMBAT:
                        yield return mainMotion = MotionNonCombat(body);
                        break;
                    case ActionPattern.MOVE:
                        yield return mainMotion = MotionMove(body);
                        break;
                    case ActionPattern.AIMING:
                        yield return mainMotion = MotionAiming(body);
                        break;
                    case ActionPattern.ATTACK:
                        yield return mainMotion = MotionAttack(body);
                        break;
                    default:
                        break;
                }
            } while(mainMotion != null);
            yield break;
        }

        /// <summary>
        /// 非反応時行動
        /// </summary>
        /// <returns></returns>
        IEnumerator MotionNonCombat(Sejiziuequje body)
        {
            if(body == null || hand == null) yield break;
            hand.isFixedMode = true;
            yield break;
        }

        /// <summary>
        /// 移動時行動
        /// </summary>
        /// <returns>コルーチン</returns>
        IEnumerator MotionMove(Sejiziuequje body)
        {
            if(body == null || hand == null) yield break;
            nowActionState = ActionPattern.AIMING;
            nowMotionType = new[] {
                HandMotionType.GRENADE,
                HandMotionType.GRENADE_FIXED,
                HandMotionType.LASER,
                HandMotionType.LASER_FIXED,
                HandMotionType.LASER_SPIN,
                HandMotionType.LASER_CLOSEUP
            }.SelectRandom(body.seriousMode ? new[] { 32, 12, 3, 1, 1, 5 } : new[] { 16, 6, 6, 1, 3, 10 });

            var targetPosition = body.nearTarget.position + Random.Range(0f, 359f).ToVector(viewSize.x / 2);
            yield return AutoMove(targetPosition);
            yield break;
        }
        /// <summary>
        /// 照準操作行動
        /// </summary>
        /// <returns>コルーチン</returns>
        IEnumerator MotionAiming(Sejiziuequje body)
        {
            if(body == null || hand == null) yield break;
            nowActionState = ActionPattern.ATTACK;

            switch(nowMotionType)
            {
                case HandMotionType.GRENADE:
                case HandMotionType.LASER:
                case HandMotionType.LASER_SPIN:
                    {
                        var destination = body.nearTarget.position + Random.Range(0f, 359f).ToVector(viewSize.y / 2);
                        var targetPosition = body.nearTarget.position;
                        yield return AutoMove(destination, () => myShip.Aiming(targetPosition));
                    }
                    break;
                case HandMotionType.GRENADE_FIXED:
                case HandMotionType.LASER_FIXED:
                    {
                        hand.isFixedMode = true;
                        yield return Wait(() => hand.isFixed);
                    }
                    break;
                case HandMotionType.GRENADE_BURST:
                case HandMotionType.LASER_BURST:
                    {
                        isStandby = true;
                        yield return Wait(() => !isStandby);
                    }
                    break;
                case HandMotionType.LASER_CLOSEUP:
                    {
                        var destination = body.nearTarget.position;
                        yield return AutoMove(destination, () => myShip.Aiming(body.nearTarget.position));
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
        /// <returns>コルーチン</returns>
        IEnumerator MotionAttack(Sejiziuequje body)
        {
            if(body == null || hand == null) yield break;
            nowActionState = ActionPattern.MOVE;

            switch(nowMotionType)
            {
                case HandMotionType.GRENADE:
                case HandMotionType.GRENADE_FIXED:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NOMAL);
                        yield return Wait(() => hand.canAction);
                    }
                    break;
                case HandMotionType.GRENADE_BURST:
                    {
                        yield return Wait(() => hand.canAction);
                        var fireNum = Random.Range(1, body.shipLevel) + 5;
                        for(int fire = 0; fire < fireNum; fire++)
                        {
                            hand.Action(Weapon.ActionType.NOMAL);
                            for(int time = 0; !hand.canAction; time++)
                            {
                                myShip.Aiming(body.nearTarget.position, siteSpeedTweak: 0.2f);
                                yield return Wait(1);
                            }
                        }
                    }
                    break;
                case HandMotionType.LASER:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NPC);
                        for(int time = 0; !hand.canAction; time++)
                        {
                            myShip.Aiming(body.nearTarget.position, siteSpeedTweak: 0.1f);
                            yield return Wait(1);
                        }
                    }
                    break;
                case HandMotionType.LASER_FIXED:
                case HandMotionType.LASER_CLOSEUP:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NPC);
                        yield return Wait(() => hand.onAttack);
                        hand.isFixedMode = false;
                        yield return Wait(() => hand.canAction);
                    }
                    break;
                case HandMotionType.LASER_BURST:
                    {
                        yield return Wait(() => hand.canAction);
                        var fireNum = Random.Range(1, body.shipLevel) + 2;
                        for(int fire = 0; fire < fireNum; fire++)
                        {
                            hand.Action(Weapon.ActionType.NPC);
                            for(int time = 0; !hand.canAction; time++)
                            {
                                myShip.Aiming(body.nearTarget.position, siteSpeedTweak: 0.1f);
                                yield return Wait(1);
                            }
                        }
                    }
                    break;
                case HandMotionType.LASER_SPIN:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NPC);
                        yield return Wait(() => hand.onAttack);
                        var timeLimit = body.interval * (body.seriousMode ? 8 : 12);
                        var widthSign = hand.nWidthPositive;
                        var startAngle = myShip.siteAlignment.ToAngle();
                        var radius = viewSize.y / 2;
                        for(int time = 0; time < timeLimit; time++)
                        {
                            var rotationTweak = Easing.quadratic.In(widthSign * 360, time, timeLimit - 1) + startAngle;
                            var direction = (Vector2)(rotationTweak.ToRotation() * Vector2.right.ToVector(radius));
                            myShip.Aiming(hand.position + direction);
                            yield return Wait(1);
                        }
                        yield return Wait(() => hand.canAction);
                    }
                    break;
                default:
                    break;
            }

            yield return Wait(body.interval);
            yield break;
        }
        IEnumerator AutoMove(Vector2 targetPosition, UnityAction halfwayProcess = null)
        {
            if(hand == null) yield break;
            hand.isFixedMode = false;
            var speed = myShip.maximumSpeed;
            yield return myShip.HeadingDestination(targetPosition, speed, speed / baseMas.magnitude, halfwayProcess);
            yield return myShip.StoppingAction();
            yield break;
        }
    }
}
