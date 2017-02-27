using System.Collections;

public partial class Funger : Weapon
{
    /// <summary>
    /// 通常かみつき
    /// </summary>
    protected class Bite : IMotion<Funger>
    {
        public IEnumerator mainMotion(Funger funger)
        {
            yield return funger.engage();
            yield break;
        }
        public IEnumerator endMotion(Funger funger)
        {
            yield return funger.reengage();
            yield break;
        }
    }
}
