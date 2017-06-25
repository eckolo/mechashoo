using System.Collections;
using UnityEngine;

public partial class Funger : Weapon
{
    /// <summary>
    /// 噛み合わせた後振り下ろし斬撃
    /// </summary>
    protected class BiteAndSlash : IMotion<Funger>
    {
        public IEnumerator MainMotion(Funger funger, bool forward = true)
        {
            if(funger.nowParent.GetComponent<Hand>() == null) yield break;
            var start1 = funger.fung1.nowLocalAngle;
            var start2 = funger.fung2.nowLocalAngle;
            var interval = Mathf.Max(funger.timeRequired / funger.density, 1);

            yield return funger.SwingAction(endPosition: new Vector2(-1.5f, 0.5f),
              timeLimit: funger.timeRequired * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, _, limit) => funger.SetEngage(start1, start2, time, limit + 1)
              );

            funger.SoundSE(funger.biteSE);
            funger.fung1.Slash(0.5f);
            funger.fung2.Slash(0.5f);

            yield return funger.SwingAction(endPosition: Vector2.zero,
              timeLimit: funger.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow((localTime - halfLimit) / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime > halfLimit;
                  if(isTiming) funger.fung1.Slash(power);
              });

            yield return funger.SwingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: funger.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow(1 - localTime / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime < halfLimit;
                  if(isTiming) funger.fung1.Slash(power);
              });

            yield break;
        }
        public IEnumerator EndMotion(Funger funger, bool forward = true)
        {
            if(funger.nowParent.GetComponent<Hand>() == null) yield break;
            yield return funger.SwingAction(endPosition: Vector2.zero,
              timeLimit: funger.timeRequired * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, _, limit) => funger.SetReEngage(time, limit + 1));

            yield return Wait(funger.timeRequired);
            yield break;
        }
    }
}
