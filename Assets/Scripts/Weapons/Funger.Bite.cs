using System.Collections;

public partial class Funger : Weapon
{
    /// <summary>
    /// 通常かみつき
    /// </summary>
    protected class Bite : IMotion<Funger>
    {
        public IEnumerator BeginMotion(Funger funger, bool forward = true)
        {
            yield break;
        }
        public IEnumerator MainMotion(Funger funger, bool forward = true)
        {
            yield return funger.Engage();
            yield break;
        }
        public IEnumerator EndMotion(Funger funger, bool forward = true)
        {
            yield return funger.Reengage();
            yield break;
        }
    }
}
