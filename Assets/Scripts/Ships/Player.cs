using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの操作機体クラス
/// </summary>
public class Player : Ship
{
    /// <summary>
    ///基となるShipオブジェクト
    /// </summary>
    public Ship baseShip = null;
    /// <summary>
    ///各種アクションのフラグ
    /// </summary>
    [SerializeField]
    private bool actionRight = false;
    [SerializeField]
    private bool actionLeft = false;
    [SerializeField]
    private bool actionBody = false;
    /// <summary>
    ///各種キーのAxes名
    /// </summary>
    const string rightActName = "ShotRight";
    const string leftActName = "ShotLeft";
    const string bodyActName = "ShotBody";

    public override void Start()
    {
        copyShipStatus();
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (armPosition.x < 0) widthPositive = !widthPositive;
        // 右・左
        float keyValueX = Input.GetAxisRaw("Horizontal");

        // 上・下
        float keyValueY = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float innerSpeed = Input.GetKey(KeyCode.LeftShift) ? 0 : speed;

        // 移動
        setVerosity(direction, innerSpeed, true);

        if (Input.GetButtonDown(rightActName)) actionRight = !actionRight;
        if (Input.GetButtonDown(leftActName)) actionLeft = !actionLeft;
        if (Input.GetButtonDown(bodyActName)) actionBody = !actionBody;

        if (actionRight)
        {
            if (getHand(getParts(0)) != null) getHand(getParts(0)).actionWeapon();
        }
        if (actionLeft)
        {
            if (getHand(getParts(1)) != null) getHand(getParts(1)).actionWeapon();
        }
        if (actionBody)
        {
            foreach (var weaponNum in weaponNumList)
            {
                if (getParts(weaponNum) != null) getParts(weaponNum).GetComponent<Weapon>().Action();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            armPosition.x += keyValueX / 200 * (widthPositive ? 1 : -1);
            armPosition.y += keyValueY / 200;
        }

        if (getHand(getParts(0)) != null) armPosition = getParts(armNumList[0]).setManipulatePosition(armPosition);
        if (getHand(getParts(1)) != null)
        {
            armPosition = getParts(armNumList[1]).setManipulatePosition(armPosition);
            if (getHand(getParts(0)) != null)
            {
                var differenceAngle = -45 * Vector2.Angle(Vector2.left, armPosition) / 180;
                getParts(armNumList[1]).transform.Rotate(0, 0, differenceAngle);
                getParts(armNumList[1]).childParts.transform.Rotate(0, 0, differenceAngle * -1);
            }
        }
    }

    public void copyShipStatus(Ship originShip = null)
    {
        ShipData originShipData = (originShip ?? baseShip).outputShiData();

        GetComponent<SpriteRenderer>().sprite = originShipData.image;
        MaxArmor = originShipData.MaxArmor;
        MaxBarrier = originShipData.MaxBarrier;
        recoveryBarrier = originShipData.recoveryBarrier;
        MaxFuel = originShipData.MaxFuel;
        recoveryFuel = originShipData.recoveryFuel;
        speed = originShipData.speed;
        acceleration = originShipData.acceleration;
        armRootPosition = originShipData.armRootPosition;
        accessoryRootPosition = originShipData.accessoryRootPosition;
        weaponRootPosition = originShipData.weaponRootPosition;
        defaultArms = originShipData.defaultArms;
        defaultAccessories = originShipData.defaultAccessories;
        defaultWeapons = originShipData.defaultWeapons;
        explosion = originShipData.explosion;
        accessoryBaseVector = originShipData.accessoryBaseVector;

        setParamate();
    }
}
