using System.Collections;

public partial class Weapon : Parts
{
    /// <summary>
    /// モーションの雛形となるクラスインターフェース
    /// </summary>
    /// <typeparam name="WeaponType">モーションの適用される武装種別クラス</typeparam>
    protected interface IMotion<WeaponType>
    {
        IEnumerator BeginMotion(WeaponType weapon, bool forward = true);
        IEnumerator MainMotion(WeaponType weapon, bool forward = true);
        IEnumerator EndMotion(WeaponType weapon, bool forward = true);
    }
}
