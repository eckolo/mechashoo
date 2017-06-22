using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 大回転モーション
    /// </summary>
    protected class LargeSpin : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword, bool forward = true)
        {
            var monoTime = sword.timeRequired / 4;
            var coreTime = monoTime * 8;
            var startAngle = sword.nowLocalAngle.compile();
            var endAngle = -150f;
            var interval = Mathf.Max(coreTime / sword.density, 1);
            var sign = forward.toSign();

            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 0.5f * sign),
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: !forward,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            startAngle = sword.nowLocalAngle.compile();
            yield return sword.swingAction(endPosition: new Vector2(-0.5f, 1 * sign),
              timeLimit: monoTime,
              timeEasing: Easing.quadratic.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 7));

            sword.soundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
            yield return sword.swingAction(endPosition: new Vector2(0.5f, 0),
              timeLimit: monoTime,
              timeEasing: Easing.liner.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 6));

            yield return sword.swingAction(endPosition: new Vector2(-0.5f, -1 * sign),
              timeLimit: monoTime,
              timeEasing: Easing.liner.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 5));

            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 0),
              timeLimit: monoTime,
              timeEasing: Easing.liner.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 4));

            yield return sword.swingAction(endPosition: new Vector2(-0.5f, 1 * sign),
              timeLimit: monoTime * 2,
              timeEasing: Easing.liner.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 2));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: new Vector2(0, 1 * sign),
              timeLimit: monoTime * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 0));
            yield break;
        }
        public IEnumerator endMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = sword.defAngle;
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequiredARest);
        }
        static void attackAction(Sword sword, float startAngle, int sign, int interval, int time, float localTime, int limit, int coreTime, int reIndex)
        {
            var spins = 3;
            var _time = time + coreTime - limit * (reIndex + 1);
            var swordAngle = 360f * spins * Easing.quintic.InOut(_time, coreTime - 1) * sign;
            sword.setAngle(startAngle - swordAngle);

            var isTiming = coreTime * 1 / 3 < _time && _time < coreTime * 2 / 3
                && (coreTime - 1 - _time) % interval == 0;
            float center = coreTime / 2;
            var power = Easing.quintic.In(center - Mathf.Abs(center - _time), center) * spins;
            if(isTiming) sword.slash(power);
        }
    }
}
