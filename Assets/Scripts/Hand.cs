using UnityEngine;
using System.Collections;

public class Hand : Parts
{
    public GameObject takeWeapon;
    public Vector2 takePosition = new Vector2();

    //武装のセット
    public int setWeapon(Ship rootShip, GameObject weapon = null, int sequenceNum = -1)
    {
        if (weapon == null) weapon = takeWeapon;

        takeWeapon = (GameObject)Instantiate(weapon, (Vector2)transform.position, transform.rotation);

        rootShip.setLayer(takeWeapon);
        takeWeapon.transform.parent = transform;
        takeWeapon.transform.localScale = new Vector3(1, 1, 1);

        rootShip.setZ(takeWeapon.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        takeWeapon.GetComponent<Parts>().selfConnection = takeWeapon.GetComponent<Weapon>().handlePosition;
        takeWeapon.GetComponent<Parts>().parentConnection = takePosition;

        return sequenceNum;
    }

    public bool actionWeapon()
    {
        return takeWeapon.GetComponent<Weapon>().Action(takeWeapon.gameObject.transform); ;
    }
}
