using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 軽量刃物系モーション
    /// </summary>
    protected class Nife : IMotion<Sword>
    {
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            var interval = Mathf.Max(sword.timeRequired / sword.density, 1);

            float startAngle = sword.nowLocalAngle.Compile();
            float endAngle = 360f;
            sword.SoundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
            yield return sword.SwingAction(endPosition: new Vector2(-1.5f, 0.5f),
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, localTime, limit) => sword.SetAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            sword.SoundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.SwingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow((localTime - halfLimit) / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime > halfLimit;
                  if(isTiming) sword.Slash(power);
              });

            yield return sword.SwingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow(1 - localTime / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime < halfLimit;
                  if(isTiming) sword.Slash(power);
              });
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
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
