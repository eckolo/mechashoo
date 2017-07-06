using System.Collections;
using UnityEngine;

public partial class Sword : Weapon
{
    /// <summary>
    /// 叩きつけ系モーション
    /// </summary>
    protected class Rod : IMotion<Sword>
    {
        public IEnumerator BeginMotion(Sword sword, bool forward = true)
        {
            yield break;
        }
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            var fireNum = sword.fireNum;
            var turnoverRate = sword.turnoverRate;
            var monoTime = sword.timeRequired / 2;

            for(var index = 0; index < fireNum; index++)
            {
                var hand = sword.nowParent.GetComponent<Hand>();
                if(hand != null)
                {
                    sword.SoundSE(sword.swingUpSE, 1, (float)sword.timeRequiredPrior / 20);
                    yield return sword.SwingAction(endPosition: new Vector2(-1.5f, 0.5f),
                      timeLimit: sword.timeRequiredPrior,
                      timeEasing: Easing.quadratic.Out,
                      clockwise: false);
                }

                float startAngle = sword.nowLocalAngle.Compile();
                float endAngle = -360f * turnoverRate;

                sword.SoundSE(sword.swingDownSE, 1, (float)monoTime / 10);
                yield return sword.SwingAction(endPosition: Vector2.zero,
                  timeLimit: monoTime * Mathf.Max(turnoverRate, 1),
                  timeEasing: Easing.exponential.In,
                  clockwise: true,
                  midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

                sword.Slash();
                yield return Wait(1);
            }
            yield break;
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f + sword.defAngle;
            yield return sword.SwingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return Wait(sword.timeRequiredARest);
            yield break;
        }
    }
}
