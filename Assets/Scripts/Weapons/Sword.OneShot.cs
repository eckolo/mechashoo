using System.Collections;

public partial class Sword : Weapon
{
    /// <summary>
    /// 単発系モーション
    /// </summary>
    protected class OneShot : IMotion<Sword>
    {
        public IEnumerator mainMotion(Sword sword)
        {
            sword.slash();
            yield return wait(1);
            yield break;
        }
        public IEnumerator endMotion(Sword sword)
        {
            yield break;
        }
    }
}
