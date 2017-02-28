using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 軽量刃物系モーション
    /// </summary>
    protected class Nife : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            var interval = Mathf.Max(sword.timeRequired / sword.density, 1);

            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = 360f;
            sword.soundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequiredPrior / 20);
            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 0.5f),
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow((localTime - halfLimit) / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime > halfLimit;
                  if(isTiming) sword.slash(power);
              });

            yield return sword.swingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var halfLimit = limit / 2;
                  var power = Mathf.Pow(1 - localTime / halfLimit, 2);
                  var isTiming = (limit - time) % interval == 0 && localTime < halfLimit;
                  if(isTiming) sword.slash(power);
              });
        }
        public IEnumerator endMotion(Sword sword)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
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
