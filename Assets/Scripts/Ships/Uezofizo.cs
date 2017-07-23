using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 小型蟹機亜種クラス
/// </summary>
public class Uezofizo : Fizo
{
    /// <summary>
    /// 殴り攻撃
    /// </summary>
    protected override void MainAttack()
    {
        var handWeapon = handWeapons.FirstOrDefault(weapon => weapon.canAction);
        if(handWeapon != null) handWeapon.Action(Weapon.ActionType.NOMAL);
    }
    /// <summary>
    /// 回転攻撃
    /// </summary>
    protected override void SubAttack()
    {
        var handWeapon = handWeapons.FirstOrDefault(weapon => weapon.canAction);
        if(handWeapon != null) handWeapon.Action(Weapon.ActionType.SINK);
    }
}
