using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 単発系モーション
    /// </summary>
    protected class OneShot : IMotion<Sword>
    {
        public IEnumerator MainMotion(Sword sword, bool forward = true)
        {
            sword.Slash();
            yield return Wait(1);
            yield break;
        }
        public IEnumerator EndMotion(Sword sword, bool forward = true)
        {
            yield break;
        }
    }
}
