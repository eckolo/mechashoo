﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 先端にWeaponを接続・制御するParts
/// </summary>
public class Hand : Parts
{
    public Weapon takeWeapon;
    public Vector2 takePosition = new Vector2();

    /// <summary>
    ///武装のセット
    /// </summary>
    public Hand setWeapon(Ship rootShip, Weapon weapon = null)
    {
        if(weapon == null) weapon = takeWeapon;
        if(weapon == null) return this;

        takeWeapon = Instantiate(weapon.gameObject, globalPosition, transform.rotation).GetComponent<Weapon>();

        takeWeapon.layer = rootShip.layer;
        takeWeapon.parent = transform;
        takeWeapon.transform.localScale = new Vector3(1, 1, 1);

        takeWeapon.nowZ = takeWeapon.handledZ;

        childParts = takeWeapon.GetComponent<Parts>();

        childParts.selfConnection = takeWeapon.handlePosition;
        childParts.parentConnection = takePosition;

        childParts.nowRoot = nowRoot;

        return this;
    }

    public bool actionWeapon(Weapon.ActionType action = Weapon.ActionType.NOMAL)
    {
        if(takeWeapon == null) return false;
        return takeWeapon.action(action);
    }

    public override Vector2 nowCorrection
    {
        get {
            if(takeWeapon == null) return base.nowCorrection;
            if(takeWeapon.GetComponent<Parts>() == null) return base.nowCorrection;
            return correctionVector + takeWeapon.nowCorrection;
        }
    }
}
