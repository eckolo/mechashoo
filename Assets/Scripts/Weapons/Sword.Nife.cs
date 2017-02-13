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
            sword.soundSE(sword.swingUpSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: new Vector2(-1.5f, 0.5f),
              timeLimit: sword.timeRequired * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: false,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((sword.timeRequired - 1 - time) % interval < 1) sword.slash(Mathf.Pow(localTime / limit, 2));
              });

            yield return sword.swingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((sword.timeRequired - 1 - time) % interval < 1) sword.slash(Mathf.Pow(1 - localTime / limit, 2));
              });
        }
        public IEnumerator endMotion(Sword sword)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = 360f + sword.defAngle;
            yield return sword.swingAction(endPosition: Vector2.zero,
              timeLimit: sword.timeRequired * 2,
              timeEasing: Easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.In(endAngle - startAngle, time, limit))));

            yield return wait(sword.timeRequired);
        }
    }
}
