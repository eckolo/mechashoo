using System.Collections;
using UnityEngine;

public partial class Funger : Weapon
{
    /// <summary>
    /// 噛み合わせた後振り下ろし斬撃
    /// </summary>
    protected class BiteAndSlash : IMotion<Funger>
    {
        public IEnumerator mainMotion(Funger funger)
        {
            if(funger.nowParent.GetComponent<Hand>() == null) yield break;
            var start1 = funger.fung1.nowLocalAngle;
            var start2 = funger.fung2.nowLocalAngle;
            var interval = Mathf.Max(funger.timeRequired / funger.density, 1);

            yield return funger.swingAction(endPosition: new Vector2(-1.5f, 0.5f),
              timeLimit: funger.timeRequired * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, _, limit) => funger.setEngage(start1, start2, time, limit + 1)
              );

            funger.soundSE(funger.biteSE);
            funger.fung1.slash(0.5f);
            funger.fung2.slash(0.5f);

            yield return funger.swingAction(endPosition: Vector2.zero,
              timeLimit: funger.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow((localTime - halfLimit) / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime > halfLimit;
                  if(isTiming) funger.fung1.slash(power);
              });

            yield return funger.swingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: funger.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow(1 - localTime / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime < halfLimit;
                  if(isTiming) funger.fung1.slash(power);
              });

            yield break;
        }
        public IEnumerator endMotion(Funger funger)
        {
            if(funger.nowParent.GetComponent<Hand>() == null) yield break;
            yield return funger.swingAction(endPosition: Vector2.zero,
              timeLimit: funger.timeRequired * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, _, limit) => funger.setReEngage(time, limit + 1));

            yield return wait(funger.timeRequired);
            yield break;
        }
    }
}
