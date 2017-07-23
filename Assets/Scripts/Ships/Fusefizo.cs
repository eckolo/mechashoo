using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 小型蟹機クラス
/// </summary>
public class Fusefizo : Fizo
{
    /// <summary>
    /// 殴り攻撃
    /// </summary>
    protected override void MainAttack()
    {
        var handWeapon = handWeapons.FirstOrDefault(weapon => weapon.canAction);
        if(handWeapon != null) handWeapon.Action(Weapon.ActionType.SINK);
    }
    /// <summary>
    /// 挟み攻撃
    /// </summary>
    protected override void SubAttack()
    {
        var handWeapon = handWeapons.FirstOrDefault(weapon => weapon.canAction);
        if(handWeapon != null) handWeapon.Action(Weapon.ActionType.NOMAL);
    }
}