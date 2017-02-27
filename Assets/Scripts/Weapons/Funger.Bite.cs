using System.Collections;

public partial class Funger : Weapon
{
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
