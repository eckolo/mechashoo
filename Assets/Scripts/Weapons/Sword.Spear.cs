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
        public IEnumerator mainMotion(Sword sword)
        {
            var fireMax = Mathf.Max(sword.onTypeInjections.Max(injection => injection.burst), 1);
            var stancePosition = new Vector2(-0.5f, -0.5f);

            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = 360f * fireMax;
            yield return sword.swingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequiredPrior * (fireMax - 1));

            for(int fire = 0; fire < fireMax; fire++)
            {
                var targetPosition = new Vector2(1, 0);
                if(fireMax % 2 == 0)
                {
                    if(fire % 2 == 0) targetPosition = new Vector2(1, -0.5f);
                    else targetPosition = new Vector2(1, 0.5f);
                }
                else
                {
                    if(fire % 3 == 1) targetPosition = new Vector2(1, -1);
                    else if(fire % 3 == 2) targetPosition = new Vector2(1, 1);
                }

                sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
                yield return sword.swingAction(endPosition: targetPosition,
                  timeLimit: sword.timeRequired,
                  timeEasing: Easing.exponential.In,
                  clockwise: true);

                sword.slash(1.2f);

                var localTimeRequired = fire + 1 < fireMax
                    ? sword.timeRequired
                    : sword.timeRequiredARest;

                yield return sword.swingAction(endPosition: stancePosition,
                  timeLimit: localTimeRequired * 2,
                  timeEasing: Easing.quadratic.Out,
                  clockwise: true);
            }
        }
        public IEnumerator endMotion(Sword sword)
        {
            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = 360f + sword.defAngle;
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequiredARest);
        }
    }
}
