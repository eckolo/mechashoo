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
            if (armorBar != null) armorBar.setAlpha(1);
            if (alignmentEffect == null)
            {
                alignmentEffect = outbreakEffect(alignmentSprite);
                alignmentEffect.transform.parent = transform;
            }
        }
        else
        {
            if (armorBar != null) armorBar.setAlpha(0);
            if (alignmentEffect != null)
            {
                alignmentEffect.selfDestroy();
                alignmentEffect = null;
            }
        }

        keyAction();

        if (baseAlignment.x < 0) invertWidth();

        if (Sys.playerHPbar != null)
        {
            var hpLanges = Sys.playerHPbar.setLanges(palamates.nowArmor, palamates.maxArmor + palamates.maxBarrier, viewSize.x, pibotView: true);
            var hpright = Vector2.right * hpLanges.x;

            if (Sys.playerBRbar != null) Sys.playerBRbar.setLanges(palamates.nowBarrier, palamates.maxArmor + palamates.maxBarrier, viewSize.x, hpright, true);

            if (Sys.playerENbar != null) Sys.playerENbar.setLanges(palamates.nowFuel, palamates.maxFuel, viewSize.x, Vector2.down * hpLanges.y, true);
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
            if (coreData == null) return true;
            if (coreData.image == null) return true;
            if (coreData.image.name == null) return true;
            return coreData.image.name == defaultImage.name;
        }
    }

    private void keyAction()
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

        if (armStates.Count >= 1) actionRight = handAction(getHand(getParts(armStates[0].partsNum)), actionRight, ButtomZ, ButtomA);
        if (armStates.Count >= 2) actionLeft = handAction(getHand(getParts(armStates[1].partsNum)), actionLeft, ButtomX, ButtomS);

        if (Input.GetKeyDown(ButtomC)) actionBody = !actionBody;
        if (actionBody)
        {
            foreach (var weaponSlot in weaponSlots)
            {
                if (weaponSlot.entity == null) continue;
                if (getParts(weaponSlot.partsNum) == null) continue;
                getParts(weaponSlot.partsNum).GetComponent<Weapon>().Action(Weapon.ActionType.Fixed);
            }
        }

        if (Input.GetKey(ButtomSub))
        {
            baseAlignment += new Vector2(keyValueX * nWidthPositive, keyValueY) * (baseAlignment.magnitude + 1) / 200;
            baseAlignment = MathV.within((Vector2)transform.position + baseAlignment, fieldLowerLeft, fieldUpperRight) - (Vector2)transform.position;

            if (armStates.Count <= 0) setAngle(correctWidthVector(baseAlignment));
        }
        Vector2 armRoot = armStates.Count > 0
            ? armStates[0].rootPosition
            : Vector2.zero;
        var alignmentPosition = (Vector2)transform.position + correctWidthVector(armRoot + baseAlignment);
        alignmentEffect.transform.position = alignmentPosition;
        viewPosition = alignmentPosition;

        setAllAlignment(baseAlignment);
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
