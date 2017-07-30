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
                SetVerosity(Vector2.zero);
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
    /// デフォルト画像
    /// </summary>
    [SerializeField]
    private Sprite defaultImage = null;

    /// <summary>
    /// 各種アクションのフラグ
    /// </summary>
    private bool actionRight = false;
    private bool actionLeft = false;
    private bool actionBody = false;

    public override void Start()
    {
        if(GetComponent<AudioListener>() == null) gameObject.AddComponent<AudioListener>();
        base.Start();
    }
    protected override void SetParamate()
    {
        base.SetParamate();
        nowLayer = Configs.Layers.PLAYER;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        RecieveKeyAction();

        InvertWidth(siteAlignment.x + turningBoundaryPoint * nWidthPositive);

        if(armorBar != null) armorBar.nowAlpha = canRecieveKey ? 1 : 0;

        if(sys.playerHPbar != null)
        {
            var hpLanges = sys.playerHPbar.SetLanges(palamates.nowArmor, maxArmor + maxBarrier, viewSize.x, pibotView: true);
            var hpright = Vector2.right * hpLanges.x;

            if(sys.playerBRbar != null) sys.playerBRbar.SetLanges(palamates.nowBarrier, maxArmor + maxBarrier, viewSize.x, hpright, true);

            if(sys.playerENbar != null) sys.playerENbar.SetLanges(palamates.nowFuel, maxFuel, viewSize.x, Vector2.down * hpLanges.y, true);
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
    protected override void ExecuteDestroy() => TransparentPlayer();
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
    /// ダメージ受けた時の統一動作
    /// </summary>
    public override float ReceiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
        var surplusDamage = base.ReceiveDamage(damage, penetration, continuation);
        if(surplusDamage > 0) sys.CountToDirectHitCount();
        sys.CountToHitCount();
        return surplusDamage;
    }

    private void RecieveKeyAction()
    {
        if(!canRecieveKey) return;

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float targetSpeed = Configs.Buttom.Sub.Judge(Key.Timing.ON) ? lowerSpeed : maximumSpeed;
        // 移動
        Thrust(direction, reactPower, targetSpeed);

        AccumulateAllWeapon(Configs.Buttom.Sink.Judge(Key.Timing.ON));

        if(arms.Count >= 1) actionRight = HandAction(arms[0].tipHand, actionRight, Configs.Buttom.Key1);
        if(arms.Count >= 2) actionLeft = HandAction(arms[1].tipHand, actionLeft, Configs.Buttom.Key2);

        if(Configs.Buttom.Key3.Judge()) actionBody = !actionBody;
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.Action(actionBody ? Weapon.ActionType.FIXED : Weapon.ActionType.NOMOTION);
        }

        ManipulateAim();
    }
    float keyValueX => Configs.Buttom.Right.ToInt() - Configs.Buttom.Left.ToInt();
    float keyValueY => Configs.Buttom.Up.ToInt() - Configs.Buttom.Down.ToInt();

    private bool HandAction(Hand actionHand, bool actionNow, KeyCode keyMain)
    {
        if(actionHand != null && actionHand.takeWeapon != null)
        {
            var takeWeapon = actionHand.takeWeapon;
            if(actionNow && Configs.Buttom.Sink.Judge(Key.Timing.DOWN))
            {
                if(takeWeapon.nextAction != Weapon.ActionType.SINK)
                {
                    actionHand.ActionWeapon(Weapon.ActionType.SINK);
                }
            }
            else if(keyMain.Judge())
            {
                actionNow = !actionNow;
                if(!actionNow && takeWeapon.nextAction == Weapon.ActionType.NOMAL)
                {
                    actionHand.ActionWeapon(Weapon.ActionType.NOMOTION);
                }
            }
            if(actionNow && takeWeapon.nextAction == Weapon.ActionType.NOMOTION) actionHand.ActionWeapon();
        }
        return actionNow;
    }
    private Vector2 ManipulateAim()
    {
        var difference = Vector2.zero;
        if(Configs.aimingWsad)
        {
            difference += Vector2.up * Configs.Buttom.SubUp.ToInt();
            difference += Vector2.down * Configs.Buttom.SubDown.ToInt();
            difference += Vector2.left * Configs.Buttom.SubLeft.ToInt();
            difference += Vector2.right * Configs.Buttom.SubRight.ToInt();
        }
        if(Configs.aimingShift && Configs.Buttom.Sub.Judge(Key.Timing.ON))
        {
            difference += Vector2.up * Configs.Buttom.Up.ToInt();
            difference += Vector2.down * Configs.Buttom.Down.ToInt();
            difference += Vector2.left * Configs.Buttom.Left.ToInt();
            difference += Vector2.right * Configs.Buttom.Right.ToInt();
        }

        siteAlignment = (position + siteAlignment + difference * siteSpeed).Within(fieldLowerLeft, fieldUpperRight) - position;
        var alignmentPosition = position + CorrectWidthVector(armRoot) + siteAlignment;
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
