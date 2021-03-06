﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 武装連結用パーツ
/// </summary>
public class WeaponBase : Accessory
{
    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [SerializeField]
    public List<Ship.WeaponSlot> weaponSlots = new List<Ship.WeaponSlot>();

    public Things things
    {
        get {
            AttachThings();
            return GetComponent<Things>();
        }
    }

    /// <summary>
    /// 初期動作時に武装のセットを行う
    /// </summary>
    public override void AccessoryStartMotion() => SetParamate();

    public List<Weapon> SetParamate(List<Weapon> setedWeapons = null)
    {
        SetAngle(baseAngle);
        setedWeapons = setedWeapons ?? new List<Weapon>();
        foreach(var parts in things.getPartsList)
        {
            var weapon = parts.GetComponent<Weapon>();
            if(weapon != null) weapon.DestroyMyself();
        }

        var residualWeapons = setedWeapons.Select(weapon => weapon).ToList();
        for(var index = 0; index < weaponSlots.Count; index++)
        {
            var weaponSlot = weaponSlots[index];
            if(!weaponSlot.unique) weaponSlot.entity = residualWeapons.FirstOrDefault();
            if(residualWeapons.Any()) residualWeapons = residualWeapons.Skip(1).ToList();

            weaponSlot.partsNum = -1;
            if(weaponSlot.entity == null) continue;

            var setedWeapon = Instantiate(weaponSlot.entity, globalPosition, transform.rotation);

            setedWeapon.nowSort = nowSort;
            setedWeapon.nowOrder = nowOrder;
            setedWeapon.nowParent = transform;
            setedWeapon.transform.localScale = new Vector3(1, 1, 1);

            var partsNum = things.SetParts(setedWeapon);
            if(partsNum >= 0)
            {
                setedWeapon.parentConnection = weaponSlot.rootPosition;
                setedWeapon.nowZ = weaponSlot.positionZ;
            }
            setedWeapon.CheckConnection();
            weaponSlot.partsNum = partsNum;

            var parts = things.GetParts(weaponSlot.partsNum);
            if(parts != null)
            {
                parts.selfConnection = weaponSlot.entity.handlePosition;
                parts.GetComponent<Weapon>().baseAngle = weaponSlot.baseAngle;
            }

            weaponSlots[index] = weaponSlot;
        }

        var childWeaponBases = things.nowChildren
            .Where(child => child.GetComponent<WeaponBase>() != null)
            .Select(child => child.GetComponent<WeaponBase>());
        foreach(var child in childWeaponBases) residualWeapons = child.SetParamate(residualWeapons);
        return residualWeapons;
    }

    /// <summary>
    /// Thingsコンポーネントをアタッチするだけの関数
    /// </summary>
    protected Things AttachThings()
    {
        var _things = GetComponent<Things>();
        if(_things == null)
        {
            _things = gameObject.AddComponent<Things>();
            foreach(Transform child in transform) _things.SetParts(child.GetComponent<Parts>());
        }
        return _things;
    }
}
