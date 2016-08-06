using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの操作機体クラス
/// </summary>
public class Player : Ship
{
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
        HPbar = getBar(barType.HPbar);
        BRbar = getBar(barType.BRbar);
        ENbar = getBar(barType.ENbar);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        keyActioon();

        if (armPosition.x < 0) widthPositive = !widthPositive;

        var cameraWidth = Camera.main.ViewportToWorldPoint(Vector2.one).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x;

        var hpLanges = HPbar.setLanges(NowArmor, MaxArmor + MaxBarrier, cameraWidth);
        var hpright = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(hpLanges.x, 0);

        BRbar.setLanges(NowBarrier, MaxArmor + MaxBarrier, cameraWidth, hpright);

        ENbar.setLanges(NowFuel, MaxFuel, cameraWidth, (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(0, -hpLanges.y));
    }

    private void keyActioon()
    {
        if (!canRecieveKey) return;

        // 右・左
        float keyValueX = Input.GetAxisRaw(ButtomNameWidth);

        // 上・下
        float keyValueY = Input.GetAxisRaw(ButtomNameHeight);

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float innerSpeed = Input.GetKey(ButtomSub) ? maxLowSpeed : maxSpeed;
        // 移動
        setVerosity(direction, innerSpeed, acceleration, true);

        if (Input.GetKeyDown(ButtomZ)) actionRight = !actionRight;
        if (Input.GetKeyDown(ButtomX)) actionLeft = !actionLeft;
        if (Input.GetKeyDown(ButtomC)) actionBody = !actionBody;

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

        if (Input.GetKey(ButtomSub))
        {
            armPosition.x += keyValueX / 200 * (widthPositive ? 1 : -1);
            armPosition.y += keyValueY / 200;
        }
    }
}
