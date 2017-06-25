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
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            var fireMax = Mathf.Max(sword.onTypeInjections.Max(injection => injection.burst), 1);
            var stancePosition = new Vector2(-0.5f, -0.5f);

            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f * fireMax;
            yield return sword.SwingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredPrior * (fireMax - 1));

            for(int fire = 0; fire < fireMax; fire++)
            {
                var targetPosition = new Vector2(1, 0);
                if(fireMax % 2 == 0)
                {
                    if(fire % 2 == 0) targetPosition = new Vector2(1, -0.5f * forward.ToSign());
                    else targetPosition = new Vector2(1, 0.5f * forward.ToSign());
                }
                else
                {
                    if(fire % 3 == 1) targetPosition = new Vector2(1, -1 * forward.ToSign());
                    else if(fire % 3 == 2) targetPosition = new Vector2(1, 1 * forward.ToSign());
                }

                sword.SoundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
                yield return sword.SwingAction(endPosition: targetPosition,
                  timeLimit: sword.timeRequired,
                  timeEasing: Easing.exponential.In,
                  clockwise: true);

                sword.Slash(1.2f);

                var localTimeRequired = fire + 1 < fireMax
                    ? sword.timeRequired
                    : sword.timeRequiredARest;

                yield return sword.SwingAction(endPosition: stancePosition,
                  timeLimit: localTimeRequired * 2,
                  timeEasing: Easing.quadratic.Out,
                  clockwise: true);
            }
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f + sword.defAngle;
            yield return sword.SwingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredARest);
        }
    }
}
