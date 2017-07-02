using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class Guhabaji : Npc
{
    /// <summary>
    /// 本体設置武装で狙う場合の目標地点
    /// </summary>
    protected Vector2 bodyAimPosition =>
          Vector2.right * (nearTarget.position.x + gunDistance * targetSign) +
          Vector2.up * (nearTarget.position.y - bodyWeaponRoot.y);
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
    /// <summary>
    /// 照準位置を標準座標へ連続的に移動させる
    /// </summary>
    /// <param name="siteTweak">照準移動速度補正値</param>
    /// <returns>照準位置</returns>
    protected Vector2 SetBaseAiming(float siteTweak = 1)
        => Aiming(standardAimPosition, 0, siteTweak);
}
