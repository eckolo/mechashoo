using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 先端にWeaponを接続・制御するParts
/// </summary>
public class Hand : Arm
{
    public Weapon takeWeapon;
    public Vector2 takePosition = new Vector2();

    /// <summary>
    /// 武装のセット
    /// </summary>
    public Hand SetWeapon(Ship rootShip, Weapon weapon = null)
    {
        if(weapon == null) weapon = takeWeapon;
        if(weapon == null) return this;

        takeWeapon = Instantiate(weapon.gameObject, globalPosition, transform.rotation).GetComponent<Weapon>();

        takeWeapon.nowSort = rootShip.nowSort;
        takeWeapon.nowOrder = rootShip.nowOrder;
        takeWeapon.nowParent = transform;
        takeWeapon.transform.localScale = new Vector3(1, 1, 1);

        takeWeapon.nowZ = takeWeapon.handledZ;

        childParts = takeWeapon.GetComponent<Parts>();

        childParts.selfConnection = takeWeapon.handlePosition;
        childParts.parentConnection = takePosition;

        childParts.nowRoot = nowRoot;

        return this;
    }

    public bool ActionWeapon(Weapon.ActionType action = Weapon.ActionType.NOMAL)
    {
        if(takeWeapon == null) return false;
        return takeWeapon.Action(action);
    }

    public override Vector2 nowCorrection
    {
        get {
            if(takeWeapon == null) return base.nowCorrection;
            if(takeWeapon.GetComponent<Parts>() == null) return base.nowCorrection;
            return correctionVector + takeWeapon.nowCorrection;
        }
    }

    public override Vector2 nowLengthVector
    {
        get {
            return takePosition - selfConnection;
        }
    }

    /// <summary>
    /// 所持物の長さ
    /// </summary>
    public override float takeWeaponReach
    {
        get {
            var weapon = takeWeapon;
            if(weapon == null) return 0;
            if(weapon.nowInjections == null) return 0;

            return weapon.nowInjections.Max(injection => injection.hole.x) - weapon.handlePosition.x;
        }
    }
}
