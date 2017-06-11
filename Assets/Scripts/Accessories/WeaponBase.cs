using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            attachThings();
            return GetComponent<Things>();
        }
    }

    /// <summary>
    /// 初期動作時に武装のセットを行う
    /// </summary>
    public override void accessoryStartMotion() => setParamate();

    protected void setParamate()
    {
        setAngle(baseAngle);

        for(var index = 0; index < weaponSlots.Count; index++)
        {
            var weaponSlot = weaponSlots[index];

            weaponSlot.partsNum = -1;
            if(weaponSlot.entity == null) continue;

            var setedWeapon = Instantiate(weaponSlot.entity, globalPosition, transform.rotation);

            setedWeapon.nowSort = nowSort;
            setedWeapon.nowOrder = nowOrder;
            setedWeapon.nowParent = transform;
            setedWeapon.transform.localScale = new Vector3(1, 1, 1);

            var partsNum = things.setParts(setedWeapon);
            if(partsNum >= 0)
            {
                setedWeapon.parentConnection = weaponSlot.rootPosition;
                setedWeapon.nowZ = weaponSlot.positionZ;
            }
            setedWeapon.checkConnection();
            weaponSlot.partsNum = partsNum;

            var parts = things.getParts(weaponSlot.partsNum);
            if(parts != null)
            {
                parts.selfConnection = weaponSlot.entity.handlePosition;
                parts.GetComponent<Weapon>().baseAngle = weaponSlot.baseAngle;
            }

            weaponSlots[index] = weaponSlot;
        }
        return;
    }

    /// <summary>
    /// Thingsコンポーネントをアタッチするだけの関数
    /// </summary>
    protected Things attachThings()
    {
        var things = GetComponent<Things>();
        if(things == null) things = gameObject.AddComponent<Things>();

        return things;
    }
}
