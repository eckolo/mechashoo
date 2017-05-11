using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの操作機体クラス
/// </summary>
public class Player : Ship
{
    /// <summary>
    /// 最大装甲値
    /// </summary>
    protected override float maxArmor
    {
        get {
            return base.maxArmor * 3;
        }
    }
    /// <summary>
    /// 最大障壁値
    /// </summary>
    protected override float maxBarrier
    {
        get {
            return base.maxBarrier * 3;
        }
    }

    /// <summary>
    /// 操作可能フラグ
    /// </summary>
    public bool canRecieveKey
    {
        get {
            if(!Key.recievable) return false;
            if(onPause) return false;
            return _canRecieveKey;
        }
        set {
            _canRecieveKey = value;
            canActionWeapon = value;
            if(value)
            {
                Key.recievable = true;
            }
            else
            {
                setVerosity(Vector2.zero);
                actionRight = false;
                actionLeft = false;
                actionBody = false;
            }
        }
    }
    /// <summary>
    /// 現在操作機体がキー入力を受け付けるか否かのフラグ
    /// </summary>
    bool _canRecieveKey = false;

    /// <summary>
    /// 武装の動作状態オンオフ
    /// </summary>
    protected bool canActionWeapon
    {
        set {
            foreach(var weapon in allWeapons) if(weapon != null) weapon.canAction = value;
        }
    }

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
        if(GetComponent<AudioListener>() == null) gameObject.AddComponent<AudioListener>();
        base.Start();
    }
    protected override void setParamate()
    {
        base.setParamate();
        nowLayer = Configs.Layers.PLAYER;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        keyAction();

        invertWidth(siteAlignment.x);

        if(armorBar != null) armorBar.nowAlpha = canRecieveKey ? 1 : 0;

        if(sys.playerHPbar != null)
        {
            var hpLanges = sys.playerHPbar.setLanges(palamates.nowArmor, maxArmor + maxBarrier, viewSize.x, pibotView: true);
            var hpright = Vector2.right * hpLanges.x;

            if(sys.playerBRbar != null) sys.playerBRbar.setLanges(palamates.nowBarrier, maxArmor + maxBarrier, viewSize.x, hpright, true);

            if(sys.playerENbar != null) sys.playerENbar.setLanges(palamates.nowFuel, maxFuel, viewSize.x, Vector2.down * hpLanges.y, true);
        }
    }

    /// <summary>
    /// 照準表示フラグ
    /// </summary>
    protected override bool displayAlignmentEffect => canRecieveKey;

    /// <summary>
    /// 自身の削除実行関数
    /// プレイヤーは削除せず透明化にとどめる
    /// </summary>
    protected override void executeDestroy() => transparentPlayer();
    /// <summary>
    /// プレイヤーが初期状態か否かの判定関数
    /// </summary>
    public bool isInitialState
    {
        get {
            if(coreData == null) return true;
            if(coreData.image == null) return true;
            if(coreData.image.name == null) return true;
            return coreData.image.name == defaultImage.name;
        }
    }
    /// <summary>
    ///ダメージ受けた時の統一動作
    /// </summary>
    public override float receiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
        var surplusDamage = base.receiveDamage(damage, penetration, continuation);
        if(surplusDamage > 0) sys.countToDirectHitCount();
        sys.countToHitCount();
        return surplusDamage;
    }

    private void keyAction()
    {
        if(!canRecieveKey) return;

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float targetSpeed = Configs.Buttom.Sub.judge(Key.Timing.ON) ? lowerSpeed : maximumSpeed;
        // 移動
        thrust(direction, reactPower, targetSpeed);

        if(arms.Count >= 1) actionRight = handAction(arms[0].tipHand, actionRight, Configs.Buttom.Key1);
        if(arms.Count >= 2) actionLeft = handAction(arms[1].tipHand, actionLeft, Configs.Buttom.Key2);

        if(Configs.Buttom.Key3.judge()) actionBody = !actionBody;
        if(actionBody)
        {
            foreach(var weapon in bodyWeapons)
            {
                if(weapon == null) continue;
                weapon.action(Weapon.ActionType.FIXED);
            }
        }

        manipulateAim();
    }
    float keyValueX
    {
        get {
            return Configs.Buttom.Right.toInt() - Configs.Buttom.Left.toInt();
        }
    }
    float keyValueY
    {
        get {
            return Configs.Buttom.Up.toInt() - Configs.Buttom.Down.toInt();
        }
    }

    private bool handAction(Hand actionHand, bool actionNow, KeyCode keyMain)
    {
        if(actionHand != null && actionHand.takeWeapon != null)
        {
            if(keyMain.judge())
            {
                if(Configs.Buttom.Sub.judge(Key.Timing.ON))
                {
                    actionHand.actionWeapon(Weapon.ActionType.SINK);
                }
                else
                {
                    actionNow = !actionNow;
                    if(!actionNow && actionHand.takeWeapon.nextAction == Weapon.ActionType.NOMAL) actionHand.actionWeapon(Weapon.ActionType.NOMOTION);
                }
            }
            if(actionNow && actionHand.takeWeapon.nextAction == Weapon.ActionType.NOMOTION) actionHand.actionWeapon();
        }
        return actionNow;
    }
    private Vector2 manipulateAim()
    {
        var difference = Vector2.zero;
        if(Configs.AimingWsad)
        {
            difference += Vector2.up * Configs.Buttom.SubUp.toInt();
            difference += Vector2.down * Configs.Buttom.SubDown.toInt();
            difference += Vector2.left * Configs.Buttom.SubLeft.toInt();
            difference += Vector2.right * Configs.Buttom.SubRight.toInt();
        }
        if(Configs.AimingShift && Configs.Buttom.Sub.judge(Key.Timing.ON))
        {
            difference += Vector2.up * Configs.Buttom.Up.toInt();
            difference += Vector2.down * Configs.Buttom.Down.toInt();
            difference += Vector2.left * Configs.Buttom.Left.toInt();
            difference += Vector2.right * Configs.Buttom.Right.toInt();
        }

        siteAlignment = (position + siteAlignment + difference * siteSpeed).within(fieldLowerLeft, fieldUpperRight) - position;
        var alignmentPosition = position + correctWidthVector(armRoot) + siteAlignment;
        viewPosition = alignmentPosition;
        return siteAlignment;
    }

    protected override bool forcedInScreen
    {
        get {
            return canRecieveKey;
        }
    }
}
