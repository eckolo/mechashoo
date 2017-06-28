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
            var fireNum = sword.fireNum;
            var turnoverRate = sword.turnoverRate;
            for(int fire = 0; fire < fireNum; fire++)
            {
                var startAngle = sword.nowLocalAngle.Compile();
                var angleWidth = 360f * turnoverRate;
                var limit = sword.timeRequired;
                var interval = Mathf.Max(limit / sword.density, 1);
                for(int time = 0; time < limit; time++)
                {
                    sword.SetAngle(startAngle - angleWidth * Easing.quintic.InOut(time, limit - 1) * forward.ToSign());
                    var isTiming = limit / 3 < time && time < limit * 2 / 3
                        && (limit - 1 - time) % interval == 0;
                    if(isTiming) sword.Slash(0.5f);
                    yield return Wait(1);
                }
            }
            yield break;
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            yield break;
        }
    }
}
