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
    private bool actionRight = false;
    private bool actionLeft = false;
    private bool actionBody = false;
    /// <summary>
    ///操作可能フラグ
    /// </summary>
    public bool canRecieveKey = false;
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
        HPbar.GetComponent<SpriteRenderer>().color = Color.red;
        BRbar.GetComponent<SpriteRenderer>().color = Color.cyan;
        ENbar.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        keyActioon();

        if (positions.armTip.x < 0) widthPositive = !widthPositive;

        var cameraWidth = Camera.main.ViewportToWorldPoint(Vector2.one).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x;

        var hpLanges = HPbar.setLanges(palamates.nowArmor, palamates.maxArmor + palamates.maxBarrier, cameraWidth);
        var hpright = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(hpLanges.x, 0);

        BRbar.setLanges(palamates.nowBarrier, palamates.maxArmor + palamates.maxBarrier, cameraWidth, hpright);

        ENbar.setLanges(palamates.nowFuel, palamates.maxFuel, cameraWidth, (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(0, -hpLanges.y));
    }

    public override void selfDestroy(bool system = false)
    {
        deletePlayerCache();
        base.selfDestroy();
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
        float innerSpeed = Input.GetKey(ButtomSub) ? palamates.maxLowSpeed : palamates.maxSpeed;
        // 移動
        setVerosity(direction, innerSpeed, palamates.acceleration);

        actionRight = handAction(getHand(getParts(0)), actionRight, ButtomZ, ButtomA);
        actionLeft = handAction(getHand(getParts(1)), actionLeft, ButtomX, ButtomS);

        if (Input.GetKeyDown(ButtomC)) actionBody = !actionBody;
        if (actionBody)
        {
            foreach (var weaponNum in weaponNumList)
            {
                if (getParts(weaponNum) != null) getParts(weaponNum).GetComponent<Weapon>().Action(Weapon.ActionType.Fixed);
            }
        }

        if (Input.GetKey(ButtomSub))
        {
            positions.armTip.x += keyValueX / 200 * (widthPositive ? 1 : -1);
            positions.armTip.y += keyValueY / 200;
        }
    }
    private bool handAction(Hand actionHand, bool actionNow, KeyCode keyMain, KeyCode keySub)
    {
        if (actionHand != null)
        {
            if (Input.GetKeyDown(keyMain))
            {
                actionNow = !actionNow;
                if (!actionNow) actionHand.actionWeapon(Weapon.ActionType.NoMotion);
            }
            if (Input.GetKeyDown(keySub))
            {
                actionHand.actionWeapon(Weapon.ActionType.Sink);
                actionNow = false;
            }
            if (actionNow) actionHand.actionWeapon();
        }
        return actionNow;
    }
}
