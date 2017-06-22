using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 長柄武器系モーション
    /// </summary>
    protected class LongSleeved : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;

            var monoTime = sword.timeRequired / 2;
            var coreTime = monoTime * 4;
            var interval = Mathf.Max(coreTime / sword.density, 1);
            var startAngle = sword.nowLocalAngle.compile();
            var endAngle = 180f;
            var sign = forward.toSign();

            sword.soundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 0.5f * sign),
              timeLimit: sword.timeRequiredPrior,
              timeEasing: Easing.quadratic.Out,
              clockwise: !forward,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            startAngle = sword.nowLocalAngle.compile();
            yield return sword.swingAction(endPosition: new Vector2(-1, 1 * sign),
              timeLimit: monoTime,
              timeEasing: Easing.quadratic.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 3));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: new Vector2(0.5f, 0),
              timeLimit: monoTime,
              timeEasing: Easing.liner.In,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 2));

            yield return sword.swingAction(endPosition: new Vector2(-1, -1 * sign),
              timeLimit: monoTime,
              timeEasing: Easing.liner.Out,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 1));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: new Vector2(-1.5f, -0.5f * sign),
              timeLimit: monoTime,
              timeEasing: Easing.quadratic.Out,
              clockwise: forward,
              midstreamProcess: (time, localTime, limit) => attackAction(sword, startAngle, sign, interval, time, localTime, limit, coreTime, 0));
        }
        public IEnumerator endMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = sword.defAngle;
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: !forward,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequiredARest);
        }
        static void attackAction(Sword sword, float startAngle, int sign, int interval, int time, float localTime, int limit, int coreTime, int reIndex)
        {
            var _time = time + coreTime - limit * (reIndex + 1);
            var swordAngle = 360f * Easing.quintic.InOut(_time, coreTime - 1) * sign;
            sword.setAngle(startAngle - swordAngle);

            var isTiming = coreTime * 1 / 3 < _time && _time < coreTime * 2 / 3
                && (coreTime - 1 - _time) % interval == 0;
            float center = coreTime / 2;
            var power = Mathf.Pow(1 - Mathf.Abs(center - _time) / center, 2);
            if(isTiming) sword.slash(power);
        }
    }
}
