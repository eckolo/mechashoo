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
    ///操作可能フラグ
    /// </summary>
    public bool canRecieveKey = false;
    /// <summary>
    ///各種キーのAxes名
    /// </summary>
    const string rightActName = "ShotRight";
    const string leftActName = "ShotLeft";
    const string bodyActName = "ShotBody";
    /// <summary>
    ///各種バーオブジェクト
    /// </summary>
    private Bar HPbar = null;
    private Bar BRbar = null;
    private Bar ENbar = null;

    public override void Start()
    {
        base.Start();
        HPbar = GameObject.Find("HPbar").GetComponent<Bar>();
        BRbar = GameObject.Find("BRbar").GetComponent<Bar>();
        ENbar = GameObject.Find("ENbar").GetComponent<Bar>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        keyActioon();

        if (armPosition.x < 0) widthPositive = !widthPositive;

        var cameraWidth = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x - Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;

        var hpLanges = HPbar.setLanges(NowArmor, MaxArmor + MaxBarrier, cameraWidth);
        var hpright = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(hpLanges.x, 0);

        BRbar.setLanges(NowBarrier, MaxArmor + MaxBarrier, cameraWidth, hpright);

        ENbar.setLanges(NowFuel, MaxFuel, cameraWidth, (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(0, -hpLanges.y));
    }

    private void keyActioon()
    {
        if (!canRecieveKey) return;

        // 右・左
        float keyValueX = Input.GetAxisRaw("Horizontal");

        // 上・下
        float keyValueY = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float innerSpeed = Input.GetKey(KeyCode.LeftShift) ? maxspeed / 36 : maxspeed;

        // 移動
        setVerosity(direction, innerSpeed, acceleration, true);

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
    }

    public void copyShipStatus(Ship originShip = null)
    {
        ShipData originShipData = (originShip ?? baseShip).outputShiData();

        GetComponent<SpriteRenderer>().sprite = originShipData.image;
        MaxArmor = originShipData.MaxArmor;
        armorBarHeight = originShipData.armorBarHeight;
        MaxBarrier = originShipData.MaxBarrier;
        recoveryBarrier = originShipData.recoveryBarrier;
        MaxFuel = originShipData.MaxFuel;
        recoveryFuel = originShipData.recoveryFuel;
        maxspeed = originShipData.speed;
        acceleration = originShipData.acceleration;
        armRootPosition = originShipData.armRootPosition;
        accessoryRootPosition = originShipData.accessoryRootPosition;
        weaponRootPosition = originShipData.weaponRootPosition;
        defaultArms = originShipData.defaultArms;
        defaultAccessories = originShipData.defaultAccessories;
        defaultWeapons = originShipData.defaultWeapons;
        explosion = originShipData.explosion;
        accessoryBaseVector = originShipData.accessoryBaseVector;
        GetComponent<PolygonCollider2D>().points = originShipData.points;

        baseStart();
        baseUpdate();
    }
}
