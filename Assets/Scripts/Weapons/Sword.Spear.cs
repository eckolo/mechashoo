using UnityEngine;
using System.Collections;
using System.Linq;

public partial class Sword : Weapon
{
    /// <summary>
    /// 刺突系モーション
    /// </summary>
    protected class Spear : IMotion<Sword>
    {
        public IEnumerator BeginMotion(Sword sword, bool forward = true)
        {
            var fireNum = sword.fireNum;
            var turnoverRate = sword.turnoverRate;
            var stancePosition = new Vector2(-0.5f, -0.5f);

            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f * turnoverRate;
            yield return sword.SwingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequiredPrior,
              timeEasing: Easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredPrior * (fireNum - 1));
            yield break;
        }
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            var fireNum = sword.fireNum;
            var turnoverRate = sword.turnoverRate;
            var stancePosition = new Vector2(-0.5f, -0.5f);
            var monoTime = sword.timeRequired / 2;

            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f * turnoverRate;
            yield return sword.SwingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequiredPrior,
              timeEasing: Easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredPrior * (fireNum - 1));

            for(int fire = 0; fire < fireNum; fire++)
            {
                var targetPosition = new Vector2(1, 0);
                if(fireNum % 2 == 0)
                {
                    if(fire % 2 == 0) targetPosition = new Vector2(1, -0.5f * forward.ToSign());
                    else targetPosition = new Vector2(1, 0.5f * forward.ToSign());
                }
                else
                {
                    if(fire % 3 == 1) targetPosition = new Vector2(1, -1 * forward.ToSign());
                    else if(fire % 3 == 2) targetPosition = new Vector2(1, 1 * forward.ToSign());
                }

                sword.SoundSE(sword.swingDownSE, 1, (float)monoTime / 10);
                yield return sword.SwingAction(endPosition: targetPosition,
                  timeLimit: monoTime,
                  timeEasing: Easing.exponential.In,
                  clockwise: true);

                sword.Slash(1.2f);

                var localTimeRequired = fire + 1 < fireNum ? monoTime : sword.timeRequiredARest;

                yield return sword.SwingAction(endPosition: stancePosition,
                  timeLimit: localTimeRequired,
                  timeEasing: Easing.quadratic.Out,
                  clockwise: true);
            }
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f * sword.turnoverRate + sword.defAngle;
            yield return sword.SwingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredARest);
        }
    }
}
