using System.Collections;
using UnityEngine;

public partial class Sword : Weapon
{
    /// <summary>
    /// 武装自体が旋回する系モーション
    /// </summary>
    protected class Spin : IMotion<Sword>
    {
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            var magnification = 4;
            var startAngle = sword.nowLocalAngle.Compile();
            var interval = Mathf.Max(sword.timeRequired * magnification / sword.density, 1);
            var limit = sword.timeRequired * magnification;
            for(int time = 0; time < limit; time++)
            {
                sword.SetAngle(startAngle - 360f * Easing.quintic.InOut(time, limit - 1) * forward.ToSign());
                var isTiming = limit / 3 < time && time < limit * 2 / 3
                    && (limit - 1 - time) % interval == 0;
                if(isTiming) sword.Slash(0.5f);
                yield return Wait(1);
            }
            yield break;
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            yield break;
        }
    }
}
