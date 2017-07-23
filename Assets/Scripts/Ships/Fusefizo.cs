﻿using UnityEngine;
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
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.Action(Weapon.ActionType.SINK);
                break;
            }
        }
    }
    /// <summary>
    /// 挟み攻撃
    /// </summary>
    protected override void SubAttack()
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
}