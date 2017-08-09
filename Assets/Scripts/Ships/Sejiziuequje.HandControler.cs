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
        bool isFixedMode = false;

        public HandControler(AllLange _hand)
        {
            hand = _hand;
        }

        /// <summary>
        /// 現在のモーションを示す番号
        /// </summary>
        protected ActionPattern nowActionState { get; set; } = ActionPattern.NON_COMBAT;
        /// <summary>
        /// 強制固定されたモーションを示す番号
        /// </summary>
        protected ActionPattern? forceActionState { get; set; } = null;
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
            forceActionState = null;
            if(nowActionState == ActionPattern.NON_COMBAT) nowActionState = ActionPattern.MOVE;
            return mainMotion == null ? hand.StartCoroutine(Motion(body)) : null;
        }
        public void PauseMotion(Sejiziuequje body)
        {
            forceActionState = ActionPattern.NON_COMBAT;
        }
        public void EndMotion(Sejiziuequje body)
        {
            forceActionState = ActionPattern.NON_COMBAT;
            foreach(Transform child in body.transform)
            {
                var bullet = child.GetComponent<Bullet>();
                if(bullet == null) continue;
                bullet.DestroyMyself();
            }
            isFixedMode = true;
        }
        public void SetMotionType(HandMotionType motionType)
        {
            forceActionState = null;
            nowActionState = ActionPattern.AIMING;
            nowMotionType = motionType;
        }
        IEnumerator Motion(Sejiziuequje body)
        {
            yield return Wait(() => myShip != null);
            while(myShip != null)
            {
                nowActionState = forceActionState ?? nowActionState;
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
                Debug.Log($"{this}:{nowActionState}:{hand.isFixedMode}:{hand.isFixed}:{myShip}");
            }
            yield break;
        }

        /// <summary>
        /// 非反応時行動
        /// </summary>
        /// <returns></returns>
        IEnumerator MotionNonCombat(Sejiziuequje body)
        {
            if(body == null || hand == null) yield break;
            hand.Action(Weapon.ActionType.NOMOTION);
            Debug.Log($"{this}:{isFixedMode}:{hand.isFixedMode}:{hand.isFixed}");
            if(isFixedMode) hand.isFixedMode = true;
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
                HandMotionType.LASER_SPIN,
                HandMotionType.LASER_CLOSEUP
            }.SelectRandom(body.seriousMode ? new[] { 6, 3, 3, 1, 4 } : new[] { 32, 12, 3, 1, 5 });

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
                    {
                        hand.isFixedMode = true;
                        yield return Wait(() => hand.isFixed);
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
                        myShip.SetFixedAlignment(alimentTarget);
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NOMAL);
                        yield return Wait(() => hand.canAction);
                    }
                    break;
                case HandMotionType.LASER:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NPC);
                        myShip.SetFixedAlignment(fixedAlimentTarget, true);
                        yield return Wait(() => hand.onAttack);
                        for(int time = 0; !hand.canAction; time++)
                        {
                            myShip.Aiming(body.nearTarget.position, siteSpeedTweak: 0.1f);
                            yield return Wait(1);
                        }
                    }
                    break;
                case HandMotionType.LASER_CLOSEUP:
                    {
                        yield return Wait(() => hand.canAction);
                        hand.Action(Weapon.ActionType.NPC);
                        myShip.SetFixedAlignment(fixedAlimentTarget, true);
                        yield return Wait(() => hand.canAction);
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
                        var aliment = myShip.SetFixedAlignment(fixedAlimentTarget, true);
                        for(int time = 0; time < timeLimit; time++)
                        {
                            if(hand.onAttack) aliment = aliment
                                    ?? myShip.SetFixedAlignment(fixedAlimentTarget, true);
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
            if(isFixedMode) yield break;
            hand.isFixedMode = false;
            var speed = myShip.maximumSpeed;
            yield return myShip.HeadingDestination(targetPosition, speed, speed, halfwayProcess);
            yield return myShip.StoppingAction();
            yield break;
        }
        Vector2 alimentTarget => myShip.position + myShip.armRoot + myShip.siteAlignment;
        Vector2 fixedAlimentTarget => myShip.CorrectWidthVector((myShip.nowAngle * -1).ToRotation() * (myShip.armRoot + myShip.siteAlignment));
    }
}
