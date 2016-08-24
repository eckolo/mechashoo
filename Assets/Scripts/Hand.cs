using UnityEngine;
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
    public int setWeapon(Ship rootShip, Weapon weapon = null, int sequenceNum = -1)
    {
        if (weapon == null) weapon = takeWeapon;
        if (weapon == null) return -1;

        takeWeapon = ((GameObject)Instantiate(weapon.gameObject, (Vector2)transform.position, transform.rotation)).GetComponent<Weapon>();

        rootShip.setLayer(takeWeapon.gameObject);
        takeWeapon.transform.parent = transform;
        takeWeapon.transform.localScale = new Vector3(1, 1, 1);

        rootShip.setZ(takeWeapon.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        childParts = takeWeapon.GetComponent<Parts>();

        childParts.selfConnection = takeWeapon.GetComponent<Weapon>().handlePosition;
        childParts.parentConnection = takePosition;

        childParts.setParent(parentMaterial);

        return sequenceNum;
    }

    public bool actionWeapon(Weapon.ActionType action = Weapon.ActionType.Nomal)
    {
        if (takeWeapon == null) return false;
        return takeWeapon.Action(action);
    }

    public override Vector2 nowCorrection
    {
        get
        {
            if (takeWeapon == null) return correctionVector;
            if (takeWeapon.GetComponent<Parts>() == null) return correctionVector;
            return correctionVector + takeWeapon.GetComponent<Parts>().nowCorrection;
        }
    }
}
