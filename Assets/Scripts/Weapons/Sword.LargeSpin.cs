using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 大回転モーション
    /// </summary>
    protected class LargeSpin : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword, bool forward = true)
        {
            var coreTime = sword.timeRequired * 2;
            var startAngle = sword.nowLocalAngle.compile();
            var endAngle = -150f;
            var interval = Mathf.Max(coreTime / sword.density, 1);
            var sign = forward.toSign();

            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 1f * sign),
              timeLimit: sword.timeRequiredPrior * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            startAngle = sword.nowLocalAngle.compile();
            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var seordAngle = 360f * 3 * Easing.quintic.InOut(time, coreTime - 1) * sign;
                  sword.setAngle(startAngle - seordAngle);
                  var isTiming = coreTime / 3 < time && (coreTime - 1 - time) % interval == 0;
                  if(isTiming) sword.slash();
              });

            yield return sword.swingAction(endPosition: new Vector2(-0.5f, -1 * sign),
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  var _time = time + coreTime - limit;
                  var seordAngle = 360f * 3 * Easing.quintic.InOut(_time, coreTime - 1) * sign;
                  sword.setAngle(startAngle - seordAngle);
                  var isTiming = _time < coreTime * 2 / 3 && (coreTime - 1 - _time) % interval == 0;
                  if(isTiming) sword.slash();
              });
            yield break;
        }
        public IEnumerator endMotion(Sword sword, bool forward = true)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = sword.defAngle;
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequiredARest * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequiredARest);
        }
    }
}
