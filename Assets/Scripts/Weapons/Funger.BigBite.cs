using System.Collections;

public partial class Funger : Weapon
{
    /// <summary>
    /// 大きくかみつき
    /// </summary>
    protected class BigBite : IMotion<Funger>
    {
        public IEnumerator mainMotion(Funger funger)
        {
            var limit = funger.timeRequired * 2;
            var startAngle1 = funger.fung1.nowLocalAngle;
            var startAngle2 = funger.fung2.nowLocalAngle;
            for(int time = 0; time < limit; time++)
            {
                funger.fung1.setAngle(startAngle1 + Easing.liner.In(30, time, limit - 1));
                funger.fung2.setAngle(startAngle1 - Easing.liner.In(30, time, limit - 1));

                yield return wait(1);
            }
            yield return funger.engage(1.2f, 1.5f);
            yield break;
        }
        public IEnumerator endMotion(Funger funger)
        {
            yield return funger.reengage(1.5f);
            yield break;
        }
    }
}
