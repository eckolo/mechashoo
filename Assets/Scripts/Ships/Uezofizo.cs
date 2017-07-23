using UnityEngine;
using System.Collections;
using System.Linq;

public class Uezofizo : Fizo
{
    /// <summary>
    /// 殴り攻撃
    /// </summary>
    protected override void MainAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.Action(Weapon.ActionType.NOMAL);
                break;
            }
        }
    }
    /// <summary>
    /// 回転攻撃
    /// </summary>
    protected override void SubAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.Action(Weapon.ActionType.SINK);
                break;
            }
        }
    }
}
