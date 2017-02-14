using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 刺突系モーション
    /// </summary>
    protected class Spear : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword)
        {
            if(sword.nowParent.GetComponent<Hand>() == null) yield break;
            var interval = Mathf.Max(sword.timeRequired / sword.density, 1);
            var stancePosition = new Vector2(-0.5f, -0.5f);

            float startAngle = sword.nowLocalAngle.compile();
            float endAngle = 360f;
            yield return sword.swingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequired * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => sword.setAngle(startAngle + (Easing.quadratic.Out(endAngle - startAngle, time, limit))));

            sword.soundSE(sword.swingDownSE, 0.5f, (float)sword.timeRequired / 20);
            yield return sword.swingAction(endPosition: new Vector2(1, 0),
              timeLimit: sword.timeRequired,
              timeEasing: Easing.exponential.In,
              clockwise: true);

            sword.slash(1.2f);

            yield return sword.swingAction(endPosition: stancePosition,
              timeLimit: sword.timeRequired * 2,
              timeEasing: Easing.quadratic.Out,
              clockwise: true);
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
