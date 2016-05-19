using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour
{
    public GameObject takeWeapon;
    public Vector2 takePosition = new Vector2();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //武装のセット
    public int setWeapon(Ship rootShip, GameObject weapon = null, int sequenceNum = -1)
    {
        if (weapon == null) weapon = takeWeapon;

        var setedWeapon = (GameObject)Instantiate(weapon, (Vector2)transform.position, transform.rotation);

        rootShip.setLayer(setedWeapon);
        setedWeapon.transform.parent = transform;
        setedWeapon.transform.localScale = new Vector3(1, 1, 1);

        rootShip.setZ(setedWeapon.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        takeWeapon = setedWeapon;

        return sequenceNum;
    }

    public bool actionWeapon()
    {
        return takeWeapon.GetComponent<Weapon>().Action(takeWeapon.gameObject.transform); ;
    }
}
