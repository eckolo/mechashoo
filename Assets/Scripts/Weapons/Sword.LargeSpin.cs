using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 大回転モーション
    /// </summary>
    protected class LargeSpin : IMotion<Sword>
    {
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            var monoTime = sword.timeRequired / 8;
            var coreTime = monoTime * 8;
            var interval = Mathf.Max(coreTime / sword.density, 1);
            var sign = forward.ToSign();

            var fireNum = sword.fireNum;
            for(int fire = 0; fire < fireNum; fire++)
            {
                var startAngle = sword.nowLocalAngle.Compile();
                var endAngle = -150f;
                yield return sword.SwingAction(endPosition: new Vector2(-1.5f, 0.5f * sign),
                    timeLimit: sword.timeRequiredPrior * 2,
                    timeEasing: Easing.quadratic.Out,
                    clockwise: !forward,
                    midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

                startAngle = sword.nowLocalAngle.Compile();
                yield return sword.SwingAction(endPosition: new Vector2(-0.5f, 1 * sign),
                    timeLimit: monoTime,
                    timeEasing: Easing.quadratic.In,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 7));

                sword.SoundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
                yield return sword.SwingAction(endPosition: new Vector2(0.5f, 0),
                    timeLimit: monoTime,
                    timeEasing: Easing.liner.In,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 6));

                yield return sword.SwingAction(endPosition: new Vector2(-0.5f, -1 * sign),
                    timeLimit: monoTime,
                    timeEasing: Easing.liner.In,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 5));

                yield return sword.SwingAction(endPosition: new Vector2(-1.5f, 0),
                    timeLimit: monoTime,
                    timeEasing: Easing.liner.In,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 4));

                yield return sword.SwingAction(endPosition: new Vector2(-0.5f, 1 * sign),
                    timeLimit: monoTime * 2,
                    timeEasing: Easing.liner.In,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 2));

                sword.SoundSE(sword.swingDownSE, 0.5f, monoTime / 2.5f);
                yield return sword.SwingAction(endPosition: new Vector2(0, 1 * sign),
                    timeLimit: monoTime * 2,
                    timeEasing: Easing.quadratic.Out,
                    clockwise: forward,
                    midstreamProcess: (time, localTime, limit) => AttackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 0));
            }
            yield break;
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = sword.defAngle;
            yield return sword.SwingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredARest);
        }
        static void AttackAction(Sword sword, float startAngle, int sign, int interval, int time, float localTime, int limit, int coreTime, int reIndex)
        {
            var spins = sword.turnoverRate;
            var _time = time + coreTime - limit * (reIndex + 1);
            var swordAngle = 360f * spins * Easing.quintic.InOut(_time, coreTime - 1) * sign;
            sword.SetAngle(startAngle - swordAngle);

            var isTiming = coreTime * 1 / 3 < _time && _time < coreTime * 2 / 3
                && (coreTime - 1 - _time) % interval == 0;
            float center = coreTime / 2;
            var power = Easing.quadratic.In(center - Mathf.Abs(center - _time), center) * spins;
            if(isTiming) sword.Slash(power);
        }
    }
}
