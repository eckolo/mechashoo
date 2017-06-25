using System.Collections;

public partial class Funger : Weapon
{
    /// <summary>
    /// 大きくかみつき
    /// </summary>
    protected class BigBite : IMotion<Funger>
    {
        public IEnumerator MainMotion(Funger funger, bool forward = true)
        {
            var limit = funger.timeRequired * 2;
            var startAngle1 = funger.fung1.nowLocalAngle;
            var startAngle2 = funger.fung2.nowLocalAngle;
            for(int time = 0; time < limit; time++)
            {
                funger.fung1.SetAngle(startAngle1 + Easing.liner.In(30, time, limit - 1));
                funger.fung2.SetAngle(startAngle1 - Easing.liner.In(30, time, limit - 1));

                yield return Wait(1);
            }
            yield return funger.Engage(1.2f, 1.5f);
            yield break;
        }
        public IEnumerator EndMotion(Funger funger, bool forward = true)
        {
            yield return funger.Reengage(1.5f);
            yield break;
        }
    }
}
