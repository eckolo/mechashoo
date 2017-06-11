using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// 機体クラス
/// </summary>
public partial class Ship : Things
{
    /// <summary>
    /// 最大速度計算
    /// </summary>
    public float maximumSpeed
    {
        get {
            if(wings.Count <= 0) return legs.Count <= 0 ? 0 : legs.Average(leg => leg.maxSpeed);
            if(legs.Count <= 0) return wings.Count <= 0 ? 0 : wings.Average(wing => wing.maxSpeed);
            return Mathf.Max(legs.Average(leg => leg.maxSpeed), wings.Average(leg => leg.maxSpeed));
        }
    }
    /// <summary>
    /// 低速時速度計算
    /// </summary>
    public float lowerSpeed
    {
        get {
            if(wings.Count <= 0) return legs.Count <= 0 ? 0 : legs.Average(leg => leg.minSpeed);
            if(legs.Count <= 0) return wings.Count <= 0 ? 0 : wings.Average(wing => wing.minSpeed);
            return Mathf.Min(legs.Average(leg => leg.minSpeed), wings.Average(leg => leg.minSpeed));
        }
    }
    /// <summary>
    /// 馬力計算
    /// </summary>
    public float reactPower
    {
        get {
            if(reactors.Count <= 0) return 0;
            return reactors.Average(reactor => reactor.horsepower);
        }
    }

    /// <summary>
    /// 基礎パラメータ
    /// </summary>
    [SerializeField]
    protected Palamates palamates = new Palamates();
    /// <summary>
    /// 最大装甲値
    /// </summary>
    protected virtual float maxArmor
    {
        get {
            return palamates.maxArmor;
        }
    }
    /// <summary>
    /// 最大障壁値
    /// </summary>
    protected virtual float maxBarrier
    {
        get {
            return palamates.maxBarrier;
        }
    }
    /// <summary>
    /// 最大燃料値
    /// </summary>
    protected float maxFuel
    {
        get {
            return palamates.maxFuel;
        }
    }
    /// <summary>
    /// 装甲ゲージオブジェクト
    /// </summary>
    protected Bar armorBar = null;
    /// <summary>
    /// 装甲ゲージオブジェクトのデフォルト位置パラメータ
    /// </summary>
    [SerializeField]
    protected float armorBarHeight = 0.5f;

    /// <summary>
    /// ダメージを受けてない期間のカウント
    /// </summary>
    private ulong noDamageCount = 0;
    /// <summary>
    /// 燃料を消費していない期間のカウント
    /// </summary>
    private ulong noReduceCount = 0;

    /// <summary>
    /// 基準照準位置
    /// </summary>
    public Vector2 siteAlignment { get; protected set; }
    /// <summary>
    /// 各腕の照準位置
    /// </summary>
    public List<Vector2> armAlignments => armStates.Select(arm => siteAlignment + arm.siteTweak).ToList();
    [SerializeField]
    private Vector2 defaultAlignment = new Vector2(1, -0.5f);
    public virtual Vector2 baseAimPosition => correctWidthVector(defaultAlignment.scaling(spriteSize));
    protected virtual float siteSpeed
    {
        get {
            return (Mathf.Log(siteAlignment.magnitude + 1) + 1) * palamates.baseSiteSpeed;
        }
    }
    /// <summary>
    /// 振り向き境界点補正
    /// </summary>
    [SerializeField]
    private float _turningBoundaryPoint = 0;
    /// <summary>
    /// 振り向き境界点補正
    /// 0以上
    /// </summary>
    protected float turningBoundaryPoint => Mathf.Max(_turningBoundaryPoint, 0) * spriteSize.x;
    /// <summary>
    /// 照準画像
    /// </summary>
    [SerializeField]
    protected Effect alignmentSprite = null;
    protected Effect alignmentEffect = null;
    protected List<Effect> alignmentEffects = new List<Effect>();
    /// <summary>
    /// 照準表示フラグ
    /// </summary>
    protected virtual bool displayAlignmentEffect => Debug.isDebugBuild;

    /// <summary>
    /// 爆発のPrefab
    /// </summary>
    [SerializeField]
    private List<Explosion> explosionEffects = new List<Explosion>();
    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [SerializeField]
    private List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    /// <summary>
    /// 付属パーツの武装スロットリスト
    /// </summary>
    public List<WeaponSlot> partsWeaponSlots => weaponBases
        .SelectMany(weaponBase => weaponBase.weaponSlots)
        .ToList();
    /// <summary>
    /// 本体設置の武装スロットリスト
    /// </summary>
    public List<WeaponSlot> bodyWeaponSlots => weaponSlots
        .Where((slot, index) => index >= arms.Count)
        .ToList();
    /// <summary>
    /// 付属パーツの武装リスト
    /// </summary>
    public List<Weapon> partsWeapons => weaponBases
        .SelectMany(weaponBase => weaponBase.weaponSlots
            .Select(slot => weaponBase.things.getParts<Weapon>(slot.partsNum)))
        .ToList();
    /// <summary>
    /// 本体直接設置（付属パーツ抜き）の武装リスト
    /// </summary>
    public List<Weapon> directBodyWeapons => bodyWeaponSlots
        .Select(slot => getParts<Weapon>(slot.partsNum))
        .ToList();
    /// <summary>
    /// 本体設置の武装リスト
    /// </summary>
    public List<Weapon> bodyWeapons => directBodyWeapons
        .Concat(partsWeapons)
        .ToList();
    /// <summary>
    /// 手持ち武装一覧
    /// </summary>
    public List<Weapon> handWeapons => arms.Select(arm => arm.tipHand.takeWeapon).ToList();
    /// <summary>
    /// 全武装リスト
    /// </summary>
    public List<Weapon> allWeapons => handWeapons.Concat(bodyWeapons).ToList();
    [SerializeField]
    private List<ArmState> armStates = new List<ArmState>();
    /// <summary>
    /// 腕の付け根の座標
    /// </summary>
    public Vector2 armRoot
    {
        get {
            return correctWidthVector(armStates.FirstOrDefault()?.rootPosition ?? Vector2.zero);
        }
    }
    public List<Arm> arms
    {
        get {
            return getPartsList.toComponents<Arm>();
        }
    }
    [SerializeField]
    private List<AccessoryState> accessoryStates = new List<AccessoryState>();
    public List<Reactor> reactors
    {
        get {
            return getPartsList.toComponents<Reactor>();
        }
    }
    public List<Leg> legs
    {
        get {
            return getPartsList.toComponents<Leg>();
        }
    }
    public List<Wing> wings
    {
        get {
            return getPartsList.toComponents<Wing>();
        }
    }
    public List<WeaponBase> weaponBases
    {
        get {
            return getPartsList.toComponents<WeaponBase>();
        }
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        setParamate();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        recovery();
        updateAlignmentEffect();
        if(!isAlive) selfDestroy();

        for(int index = 0; index < armStates.Count; index++)
        {
            var arm = getParts<Arm>(armStates[index].partsNum);
            var hand = arm.tipHand;
            if(hand == null) continue;

            armStates[index].tipPosition = arm.setAlignment(siteAlignment + armStates[index].siteTweak, index, armStates[index].positive);
        }
        if(reactors.Any(reactor => reactor.rollable)) nowForward = siteAlignment;
        else setAngle(0);

        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);

        // 毎フレーム消滅判定
        autoClear();

        noDamageCount++;
        noReduceCount++;
    }

    /// <summary>
    /// 各種パラメータの初期設定
    /// </summary>
    protected virtual void setParamate()
    {
        //紐づいたParts類の一掃
        deleteParts();

        //各種Nowパラメータの設定
        palamates.nowArmor = maxArmor;
        palamates.nowBarrier = maxBarrier;
        palamates.nowFuel = maxFuel;
        setArmorBar();

        //腕パーツ設定
        armStates = armStates.Select(state => setArm(state)).ToList();
        //羽パーツ設定
        accessoryStates = accessoryStates.Select(state => setAccessory(state)).ToList();
        //武装設定
        for(var index = 0; index < weaponSlots.Count; index++)
        {
            weaponSlots[index].partsNum = -1;
            if(weaponSlots[index].entity == null) continue;
            if(index < armStates.Count)
            {
                getParts<Arm>(armStates[index].partsNum).tipHand.setWeapon(this, weaponSlots[index].entity);
            }
            else
            {
                weaponSlots[index] = setWeapon(weaponSlots[index]);
            }
        }

        //照準を初期値に
        resetAllAlignment(baseAimPosition);
    }
    /// <summary>
    /// 全照準座標のリセット
    /// </summary>
    /// <param name="setPosition">リセット後の照準座標</param>
    /// <returns>リセット後の照準座標</returns>
    public Vector2 resetAllAlignment(Vector2? setPosition = null)
    {
        setAlignment(setPosition);
        for(var index = 0; index < armStates.Count; index++) setAlignment(index);
        return siteAlignment;
    }
    /// <summary>
    /// 照準座標の個別設定
    /// </summary>
    /// <param name="arm">照準を設定するarm</param>
    /// <param name="setPosition">照準補正値</param>
    /// <returns>補正後の絶対照準位置</returns>
    public Vector2 setAlignment(int? armIndex, Vector2? setPosition = null)
    {
        if(armIndex != null)
        {
            if(armIndex < 0) return siteAlignment;
            if(armIndex >= armStates.Count) return siteAlignment;
        }

        var sitePosition = setPosition ?? siteAlignment;
        if(armIndex == null) return siteAlignment = sitePosition;
        return siteAlignment + (armStates[armIndex ?? 0].siteTweak = sitePosition - siteAlignment);
    }
    /// <summary>
    /// 照準座標の個別設定
    /// </summary>
    /// <param name="setPosition">照準座標</param>
    /// <returns>絶対照準位置</returns>
    public Vector2 setAlignment(Vector2? setPosition) => setAlignment(null, setPosition);
    /// <summary>
    /// 照準画像の制御
    /// </summary>
    /// <returns>照準画像</returns>
    private Effect updateAlignmentEffect()
    {
        if(displayAlignmentEffect)
        {
            if(alignmentEffect == null)
            {
                var effect = alignmentSprite ?? sys.baseObjects.baseAlignmentSprite;
                alignmentEffect = outbreakEffect(effect);
                alignmentEffect.setAngle(0);
                if(Debug.isDebugBuild)
                {
                    alignmentEffects = new List<Effect>();
                    for(int index = 0; index < armStates.Count; index++)
                    {
                        alignmentEffects.Add(outbreakEffect(effect));
                    }
                }
            }
        }
        else
        {
            if(alignmentEffect != null)
            {
                alignmentEffect.selfDestroy();
                alignmentEffect = null;
                if(Debug.isDebugBuild)
                {
                    for(int index = 0; index < alignmentEffects.Count; index++)
                    {
                        alignmentEffects[index].selfDestroy();
                    }
                    alignmentEffects = new List<Effect>();
                }
            }
        }
        if(alignmentEffect != null) alignmentEffect.position = position + armRoot + siteAlignment;
        if(Debug.isDebugBuild)
        {
            for(int index = 0; index < alignmentEffects.Count; index++)
            {
                alignmentEffects[index].position = position + armRoot + siteAlignment + armStates[index].siteTweak;
            }
        }
        return alignmentEffect;
    }

    /// <summary>
    /// 付属パーツの動作設定
    /// </summary>
    /// <param name="acceleration">動作加速量</param>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        for(var index = 0; index < accessoryStates.Count; index++)
        {
            getParts<Accessory>(accessoryStates[index].partsNum).accessoryMotion(nowSpeed, index * 12);
        }
    }

    /// <summary>
    /// 各種自然回復関数
    /// </summary>
    protected void recovery()
    {
        palamates.nowBarrier = Mathf.Min(palamates.nowBarrier + palamates.recoveryBarrier * (1 + noDamageCount * 0.01f), maxBarrier);
        palamates.nowFuel = Mathf.Min(palamates.nowFuel + palamates.recoveryFuel * (1 + noReduceCount * 0.01f), maxFuel);
    }
    /// <summary>
    /// 燃料消費関数
    /// </summary>
    /// <param name="reduceValue">消費量</param>
    /// <returns>正常に消費できたかフラグ</returns>
    public bool reduceFuel(float reduceValue)
    {
        if(!canReduceFuel(reduceValue)) return false;
        noReduceCount = 0;
        palamates.nowFuel = Mathf.Max(palamates.nowFuel - reduceValue, 0);
        return true;
    }
    /// <summary>
    /// 燃料消費可否関数
    /// </summary>
    /// <param name="reduceValue">消費量</param>
    /// <returns>正常に消費できるかフラグ</returns>
    public bool canReduceFuel(float reduceValue)
    {
        return palamates.nowFuel >= reduceValue;
    }
    /// <summary>
    /// 生存判定関数
    /// </summary>
    public bool isAlive
    {
        get {
            return palamates.nowArmor > 0;
        }
    }

    /// <summary>
    /// ダメージ受けた時の統一動作
    /// </summary>
    /// <param name="damage">受けたダメージ量</param>
    /// <param name="penetration">障壁貫通フラグ</param>
    /// <param name="continuation">色変化フラグ</param>
    /// <returns>最終的に受けたダメージ量</returns>
    public virtual float receiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
        noDamageCount = 0;

        float surplusDamage = !penetration ? Mathf.Max(damage - palamates.nowBarrier, 0) : damage;
        if(!penetration) palamates.nowBarrier = Mathf.Max(palamates.nowBarrier - damage, 0);

        if(surplusDamage > 0)
        {
            //HPの操作
            palamates.nowArmor -= surplusDamage;

            if(!continuation) GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0.6f, 1);
        }
        else
        {
            if(!continuation) GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.6f, 1);
        }

        setArmorBar();

        return surplusDamage;
    }
    /// <summary>
    /// 最後に攻撃を受けた相手
    /// </summary>
    public Ship lastToHitShip
    {
        get {
            if(_lastToHitShip == null) return null;
            return _lastToHitShip;
        }
        set {
            _lastToHitShip = value;
        }
    }
    Ship _lastToHitShip = null;

    /// <summary>
    /// 個々の装甲ゲージ表示関数
    /// </summary>
    /// <param name="maxPixel">装甲最大状態でのゲージ長</param>
    /// <param name="basePosition">表示位置</param>
    /// <returns>実表示位置</returns>
    public Vector2 setArmorBar(float maxPixel = 1, Vector2? basePosition = null)
    {
        Vector2 setedPosition = basePosition ?? new Vector2(-maxPixel / 2, spriteSize.y / 2 + armorBarHeight);
        if(armorBar == null)
        {
            armorBar = Instantiate(sys.baseObjects.basicBar, setedPosition, Quaternion.AngleAxis(0, Vector3.forward));
            armorBar.nowParent = transform;
            armorBar.position = new Vector2(0, 0.5f);
        }

        var returnVector = armorBar.setLanges(palamates.nowArmor, maxArmor, maxPixel, setedPosition);
        return returnVector;
    }
    /// <summary>
    /// 個々の装甲ゲージ削除関数
    /// </summary>
    public void deleteArmorBar()
    {
        if(armorBar == null) return;
        Destroy(armorBar.gameObject);
        armorBar = null;
        return;
    }

    /// <summary>
    /// 機体の破壊
    /// </summary>
    /// <param name="system">システムによる操作フラグ</param>
    public override void selfDestroy(bool system = false)
    {
        // 爆発する
        if(!system) explosion();

        if(alignmentEffect != null) alignmentEffect.selfDestroy();
        foreach(var alignment in alignmentEffects) alignment?.selfDestroy();

        base.selfDestroy(system);
    }

    /// <summary>
    /// 爆発！
    /// </summary>
    protected virtual void explosion()
    {
        var effect = explosionEffects.FirstOrDefault() ?? sys.baseObjects.explosionEffect;
        outbreakEffect(effect);
    }

    /// <summary>
    /// 腕パーツのセット
    /// </summary>
    /// <param name="arm">腕部情報</param>
    /// <returns>腕部情報</returns>
    public ArmState setArm(ArmState arm)
    {
        arm.partsNum = setOptionParts(arm.entity, arm);
        return arm;
    }
    /// <summary>
    /// アクセサリーのセット
    /// </summary>
    /// <param name="accessoryState">付属パーツ情報</param>
    /// <returns>付属パーツ情報</returns>
    public AccessoryState setAccessory(AccessoryState accessoryState)
    {
        accessoryState.partsNum = setOptionParts(accessoryState.entity, accessoryState);

        var accessory = getParts<Accessory>(accessoryState.partsNum);
        if(accessory != null)
        {
            accessory.baseAngle = accessoryState.baseAngle;
            accessory.accessoryStartMotion();
        }

        return accessoryState;
    }
    /// <summary>
    /// 武装のセット
    /// </summary>
    /// <param name="weaponSlot">武装スロット</param>
    /// <returns>セットされたスロット情報</returns>
    public WeaponSlot setWeapon(WeaponSlot weaponSlot)
    {
        weaponSlot.partsNum = setOptionParts(weaponSlot.entity, weaponSlot);

        var parts = getParts(weaponSlot.partsNum);
        if(parts != null)
        {
            parts.selfConnection = weaponSlot.entity.handlePosition;
            parts.GetComponent<Weapon>().baseAngle = weaponSlot.baseAngle;
        }

        return weaponSlot;
    }
    /// <summary>
    /// 武装のセット
    /// </summary>
    /// <param name="index">セット対象スロット番号</param>
    /// <param name="setWeapon">セットする武装</param>
    /// <returns>機体情報</returns>
    public Ship setWeapon(int index, Weapon setWeapon = null)
    {
        coreData = coreData.setWeapon().setWeapon(index, setWeapon);
        return this;
    }

    /// <summary>
    /// パーツのセット
    /// </summary>
    /// <param name="parts">設置パーツ情報</param>
    /// <param name="partsState">パーツ設置枠情報</param>
    /// <returns>パーツ番号</returns>
    private int setOptionParts(Parts parts, PartsState partsState)
    {
        var setedParts = Instantiate(parts, globalPosition, transform.rotation);

        setedParts.nowSort = nowSort;
        setedParts.nowOrder = nowOrder;
        setedParts.nowParent = transform;
        setedParts.transform.localScale = new Vector3(1, 1, 1);

        var partsNum = setParts(setedParts);
        if(partsNum >= 0)
        {
            setedParts.parentConnection = partsState.rootPosition;
            setedParts.nowZ = partsState.positionZ;
        }
        setedParts.checkConnection();

        return partsNum;
    }

    /// <summary>
    /// 全武装の動作停止
    /// </summary>
    public void stopAllWeapon()
    {
        foreach(var weapon in allWeapons)
        {
            if(weapon != null) weapon.action(Weapon.ActionType.NOMOTION);
        }
    }

    protected virtual void autoClear()
    {
        var upperRight = fieldUpperRight * 2;
        var lowerLeft = fieldLowerLeft * 2;
        if(globalPosition.x > upperRight.x
            || globalPosition.x < lowerLeft.x
            || globalPosition.y > upperRight.y
            || globalPosition.y < lowerLeft.y)
        {
            selfDestroy(true);
        }
    }
    /// <summary>
    /// Shipの能動移動ラッパー関数
    /// </summary>
    /// <param name="direction">力のかかる方向</param>
    /// <param name="power">力の大きさ</param>
    /// <param name="targetSpeed">最終目標速度</param>
    /// <returns>結果速度</returns>
    protected virtual Vector2 thrust(Vector2 direction, float power, float? targetSpeed = null)
    {
        return base.exertPower(direction, power, targetSpeed);
    }
    /// <summary>
    /// オブジェクトへ力を掛ける関数
    /// </summary>
    /// <param name="direction">力のかかる方向</param>
    /// <param name="power">力の大きさ</param>
    /// <param name="targetSpeed">最終目標速度</param>
    /// <returns>結果速度</returns>
    public override Vector2 exertPower(Vector2 direction, float power, float? targetSpeed = null)
    {
        return base.exertPower(direction, Mathf.Max(power - reactPower, 0), targetSpeed);
    }
    /// <summary>
    /// Shipの能動停止ラッパー関数
    /// </summary>
    /// <param name="power">停止加力量</param>
    /// <returns>結果速度</returns>
    protected Vector2 thrustStop(float power) => thrust(nowSpeed, power, 0);

    /// <summary>
    /// 自然停止ラッパー関数
    /// </summary>
    /// <returns>結果速度</returns>
    protected Vector2 thrustStop() => thrustStop(reactPower);
    /// <summary>
    /// 自然停止動作関数
    /// </summary>
    /// <param name="endSpeed">目標速度</param>
    /// <returns>イテレータ</returns>
    public IEnumerator stoppingAction(float endSpeed = 0)
    {
        while(nowSpeed.magnitude > endSpeed)
        {
            thrustStop();
            yield return wait(1);
        }
    }
    /// <summary>
    /// 照準位置に最も近い非味方機体の取得
    /// </summary>
    public Ship nowNearSiteTarget
    {
        get {
            Terms<Ship> term = target => target.nowLayer != nowLayer && target.inField;
            return alignmentEffect?.getNearObject(term)?.FirstOrDefault();
        }
    }
    /// <summary>
    /// 目標地点への移動
    /// </summary>
    /// <param name="destination">目標地点</param>
    /// <param name="headingSpeed">速度指定値</param>
    /// <param name="concurrentProcess">同時並行で行う処理</param>
    /// <returns>イテレータ</returns>
    public IEnumerator headingDestination(Vector2 destination, float headingSpeed, UnityAction concurrentProcess = null)
    {
        while((destination - (position + nowSpeed)).magnitude > nowSpeed.magnitude)
        {
            thrust(destination - position, reactPower, headingSpeed);
            concurrentProcess?.Invoke();
            yield return wait(1);
        }
        thrustStop();
    }
    /// <summary>
    /// 照準の連続移動
    /// </summary>
    /// <param name="destination">目的照準位置</param>
    /// <param name="armIndex">照準操作腕番号(null：ベース照準位置)</param>
    /// <param name="siteSpeedTweak">照準位置移動速度</param>
    /// <returns>結果照準位置</returns>
    public Vector2 aiming(Vector2 destination, int? armIndex = null, float siteSpeedTweak = 1)
    {
        var nowSite = armIndex == null ? siteAlignment : armAlignments[armIndex ?? 0];
        var degree = destination - (position + nowSite);
        var armCountTweak = armIndex == null ? 1 : Mathf.Max(armAlignments.Count, 1);
        var siteSpeedFinal = siteSpeed * siteSpeedTweak * armCountTweak;

        var setPosition = degree.magnitude < siteSpeedFinal
            ? destination - position
            : nowSite + degree.normalized * siteSpeedFinal;
        var result = setAlignment(armIndex, setPosition);

        invertWidth(siteAlignment.x + turningBoundaryPoint * nWidthPositive);
        return result;
    }
}
