using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// ひりゅう系フレーム共通処理
/// </summary>
public abstract class Guhabaji : Npc
{
    /// <summary>
    /// 射撃を想定した待機位置
    /// </summary>
    protected override Vector2 standardPosition => base.standardPosition - Vector2.up * bodyWeaponRoot.y;
    /// <summary>
    /// 格闘を想定した待機位置
    /// </summary>
    protected override Vector2 approachPosition => base.approachPosition - Vector2.up * bodyWeaponRoot.y;
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
