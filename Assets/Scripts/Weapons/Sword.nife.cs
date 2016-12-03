using UnityEngine;
using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 軽量刃物系モーション
    /// </summary>
    protected IEnumerator nife(bool main)
    {
        if(transform.parent.GetComponent<Hand>() == null) yield break;
        if(main)
        {
            var interval = Mathf.Max(timeRequired / density, 1);

            yield return swingAction(endPosition: new Vector2(-1, 0.5f),
              timeLimit: timeRequired * 2,
              timeEasing: easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => setAngle(60 + (easing.quartic.Out(300, time, limit))));

            wait(timeRequired);

            yield return swingAction(endPosition: Vector2.zero,
              timeLimit: timeRequired,
              timeEasing: easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((timeRequired - 1 - time) % interval < 1) slash(localTime / limit);
              });

            yield return swingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: timeRequired,
              timeEasing: easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((timeRequired - 1 - time) % interval < 1) slash(1 - localTime / limit);
              });
        }
        else
        {
            yield return swingAction(endPosition: Vector2.zero,
              timeLimit: timeRequired * 2,
              timeEasing: easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => setAngle((easing.quartic.In(420, time, limit))));
        }
    }
}
