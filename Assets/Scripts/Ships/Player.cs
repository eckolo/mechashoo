using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの操作機体クラス
/// </summary>
public class Player : Ship
{
    /// <summary>
    /// 照準画像
    /// </summary>
    [SerializeField]
    private Effect alignmentSprite = null;
    private Effect alignmentEffect = null;
    /// <summary>
    ///操作可能フラグ
    /// </summary>
    public bool canRecieveKey = false;
    /// <summary>
    ///デフォルト画像
    /// </summary>
    [SerializeField]
    private Sprite defaultImage = null;

    /// <summary>
    ///各種アクションのフラグ
    /// </summary>
    private bool actionRight = false;
    private bool actionLeft = false;
    private bool actionBody = false;

    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (canRecieveKey)
        {
            if (alignmentEffect == null)
            {
                alignmentEffect = outbreakEffect(alignmentSprite);
                alignmentEffect.transform.parent = transform;
            }
        }
        else
        {
            if (alignmentEffect != null)
            {
                alignmentEffect.selfDestroy();
                alignmentEffect = null;
            }
        }

        keyActioon();

        if (positions.baseAlignment.x < 0) invertWidth();

        var cameraWidth = Camera.main.ViewportToWorldPoint(Vector2.one).x - Camera.main.ViewportToWorldPoint(Vector2.zero).x;

        if (Sys.playerHPbar != null)
        {
            var hpLanges = Sys.playerHPbar.setLanges(palamates.nowArmor, palamates.maxArmor + palamates.maxBarrier, cameraWidth);
            var hpright = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(hpLanges.x, 0);

            if (Sys.playerBRbar != null) Sys.playerBRbar.setLanges(palamates.nowBarrier, palamates.maxArmor + palamates.maxBarrier, cameraWidth, hpright);

            if (Sys.playerENbar != null) Sys.playerENbar.setLanges(palamates.nowFuel, palamates.maxFuel, cameraWidth, (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 1)) + new Vector2(0, -hpLanges.y));
        }
    }

    public override void selfDestroy(bool system = false)
    {
        transparentPlayer();
    }

    /// <summary>
    /// プレイヤーが初期状態か否かの判定関数
    /// </summary>
    public bool isInitialState
    {
        get
        {
            return coreData.image.name == defaultImage.name;
        }
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

        if (armList.Count >= 1) actionRight = handAction(getHand(getParts(armList[0].num)), actionRight, ButtomZ, ButtomA);
        if (armList.Count >= 2) actionLeft = handAction(getHand(getParts(armList[1].num)), actionLeft, ButtomX, ButtomS);

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
            positions.baseAlignment += new Vector2(keyValueX * nWidthPositive, keyValueY) * (positions.baseAlignment.magnitude + 1) / 200;
            positions.baseAlignment = MathV.within((Vector2)transform.position + positions.baseAlignment, fieldLowerLeft, fieldUpperRight) - (Vector2)transform.position;

            if (armList.Count <= 0) setAngle(correctWidthVector(positions.baseAlignment));
        }
        var alignmentPosition = (Vector2)transform.position + correctWidthVector(positions.armRoot + positions.baseAlignment);
        alignmentEffect.transform.position = alignmentPosition;
        viewPosition = alignmentPosition;

        setAllAlignment(positions.baseAlignment);
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
