using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 軽量刃物系モーション
    /// </summary>
    protected class Nife : IMotion<Sword>
    {
        public IEnumerator BeginMotion(Sword sword, bool forward = true)
        {
            yield break;
        }
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            var monoTime = sword.timeRequired / 2;
            var interval = Mathf.Max(monoTime / sword.density, 1);

            var fireNum = sword.fireNum;
            var turnoverRate = sword.turnoverRate;
            for(var fire = 0; fire < fireNum; fire++)
            {
                float startAngle = sword.nowLocalAngle.Compile();
                float endAngle = 360f * turnoverRate;
                sword.SoundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
                yield return sword.SwingAction(endPosition: new Vector2(-1.5f, 0.5f),
                  timeLimit: sword.timeRequiredPrior,
                  timeEasing: Easing.quadratic.Out,
                  clockwise: false,
                  midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

                sword.SoundSE(sword.swingDownSE, 0.5f, (float)monoTime / 10);
                yield return sword.SwingAction(endPosition: Vector2.zero,
                  timeLimit: monoTime,
                  timeEasing: Easing.exponential.In,
                  clockwise: true,
                  midstreamProcess: (time, localTime, limit) => {
                      var halfLimit = limit / 2;
                      var power = Mathf.Pow((localTime - halfLimit) / halfLimit, 2);
                      var isTiming = (limit - time) % interval == 0 && localTime > halfLimit;
                      if(isTiming) sword.Slash(power);
                  });

                yield return sword.SwingAction(endPosition: new Vector2(-0.5f, -1),
                  timeLimit: monoTime,
                  timeEasing: Easing.exponential.Out,
                  clockwise: true,
                  midstreamProcess: (time, localTime, limit) => {
                      var halfLimit = limit / 2;
                      var power = Mathf.Pow(1 - localTime / halfLimit, 2);
                      var isTiming = (limit - time) % interval == 0 && localTime < halfLimit;
                      if(isTiming) sword.Slash(power);
                  });
            }
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
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
