using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class Kemi : Npc
{
    /// <summary>
    /// 本体設置武装で狙う場合の目標地点
    /// </summary>
    protected Vector2 bodyAimPosition =>
          Vector2.right * (nearTarget.position.x + viewSize.x * (position.x - nearTarget.position.x).toSign() / 2) +
          Vector2.up * (nearTarget.position.y + bodyWeaponRoot.y);
    /// <summary>
    /// 武装の接続基点
    /// </summary>
    protected Vector2 bodyWeaponRoot
    {
        get {
            var weaponBase = weaponBases.FirstOrDefault();
            if(weaponBase == null) return Vector2.zero;

            var slot = weaponBase.weaponSlots.FirstOrDefault();
            return weaponBase.position + (slot?.rootPosition ?? Vector2.zero);
        }
    }
}
