﻿using UnityEngine;
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
    /// 基礎パラメータ
    /// </summary>
    [System.Serializable]
    public class Palamates : ICopyAble<Palamates>, System.IEquatable<Palamates>
    {
        /// <summary>
        /// 装甲関係
        /// </summary>
        public float maxArmor = 1;
        public float nowArmor { get; set; }
        /// <summary>
        /// 障壁関係
        /// </summary>
        public float maxBarrier = 1;
        public float recoveryBarrier = 0.1f;
        public float nowBarrier { get; set; }
        /// <summary>
        /// 燃料関係
        /// </summary>
        public float maxFuel = 1;
        public float recoveryFuel = 0.1f;
        public float nowFuel { get; set; }
        /// <summary>
        /// 照準移動速度
        /// </summary>
        public float baseSiteSpeed = 0.005f;

        public bool Equals(Palamates other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(maxArmor != other.maxArmor) return false;
            if(maxBarrier != other.maxBarrier) return false;
            if(recoveryBarrier != other.recoveryBarrier) return false;
            if(maxFuel != other.maxFuel) return false;
            if(recoveryFuel != other.recoveryFuel) return false;
            if(baseSiteSpeed != other.baseSiteSpeed) return false;

            return true;
        }

        public Palamates myself
        {
            get {
                return new Palamates
                {
                    maxArmor = maxArmor,
                    maxBarrier = maxBarrier,
                    recoveryBarrier = recoveryBarrier,
                    maxFuel = maxFuel,
                    recoveryFuel = recoveryFuel,
                    baseSiteSpeed = baseSiteSpeed
                };
            }
        }
    }
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
    protected virtual Vector2 baseAimPosition => correctWidthVector(defaultAlignment.scaling(baseSize));
    protected virtual float siteSpeed
    {
        get {
            return (Mathf.Log(siteAlignment.magnitude + 1) + 1) * palamates.baseSiteSpeed;
        }
    }
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
    protected virtual bool displayAlignmentEffect { get; private set; } = false;

    /// <summary>
    /// パーツパラメータベースクラス
    /// </summary>
    public class PartsState : ICopyAble<PartsState>
    {
        public Vector2 rootPosition = Vector2.zero;
        public float positionZ = 1;

        public int partsNum { get; set; }
        public PartsState myself
        {
            get {
                return new PartsState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum
                };
            }
        }
    }

    /// <summary>
    /// 爆発のPrefab
    /// </summary>
    [SerializeField]
    protected Explosion explosion;

    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [System.Serializable]
    public class WeaponSlot : PartsState, ICopyAble<WeaponSlot>, System.IEquatable<WeaponSlot>
    {
        public Weapon entity = null;
        public float baseAngle = 0;

        public new WeaponSlot myself
        {
            get {
                return new WeaponSlot
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity,
                    baseAngle = baseAngle
                };
            }
        }

        public bool Equals(WeaponSlot other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;
            if(entity != other.entity) return false;
            if(baseAngle != other.baseAngle) return false;

            return true;
        }
    }
    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [SerializeField]
    private List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    public List<Weapon> bodyWeapons
    {
        get {
            return weaponSlots
                .Where((slot, index) => index >= arms.Count)
                .Select(slot => getParts<Weapon>(slot.partsNum))
                .ToList();
        }
    }
    /// <summary>
    /// 手持ち武装一覧
    /// </summary>
    public List<Weapon> handWeapons
    {
        get {
            return arms.Select(arm => arm.tipHand.takeWeapon).ToList();
        }
    }
    /// <summary>
    /// 全武装リスト
    /// </summary>
    public List<Weapon> allWeapons
    {
        get {
            return handWeapons.Concat(bodyWeapons).ToList();
        }
    }
    public Ship setWeapon(int index, Weapon setWeapon = null)
    {
        coreData = coreData.setWeapon().setWeapon(index, setWeapon);
        return this;
    }

    /// <summary>
    /// 腕部パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class ArmState : PartsState, ICopyAble<ArmState>, System.IEquatable<ArmState>
    {
        public Arm entity = null;

        public Vector2 tipPosition { get; set; }

        public Vector2 siteTweak { get; set; }

        public new ArmState myself
        {
            get {
                return new ArmState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity,
                    tipPosition = tipPosition,
                    siteTweak = siteTweak
                };
            }
        }

        public bool Equals(ArmState other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;
            if(entity != other.entity) return false;
            if(tipPosition != other.tipPosition) return false;
            if(siteTweak != other.siteTweak) return false;

            return true;
        }
    }
    [SerializeField]
    private List<ArmState> armStates = new List<ArmState>();
    public Vector2 armRoot
    {
        get {
            return armStates.FirstOrDefault()?.rootPosition ?? Vector2.zero;
        }
    }
    public List<Arm> arms
    {
        get {
            return toComponents<Arm>(getPartsList);
        }
    }

    /// <summary>
    /// 付属パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class AccessoryState : PartsState,
        ICopyAble<AccessoryState>,
        System.IEquatable<AccessoryState>
    {
        public Accessory entity = null;

        public new AccessoryState myself
        {
            get {
                return new AccessoryState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity
                };
            }
        }

        public bool Equals(AccessoryState other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;
            if(entity != other.entity) return false;

            return true;
        }
    }
    [SerializeField]
    private List<AccessoryState> accessoryStates = new List<AccessoryState>();
    public List<Reactor> reactors
    {
        get {
            return toComponents<Reactor>(getPartsList);
        }
    }
    public List<Leg> legs
    {
        get {
            return toComponents<Leg>(getPartsList);
        }
    }
    public List<Wing> wings
    {
        get {
            return toComponents<Wing>(getPartsList);
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

            armStates[index].tipPosition = arm.setAlignment(siteAlignment + armStates[index].siteTweak, index);
        }
        if(wings.Any(wing => wing.rollable)) nowForward = siteAlignment;
        else setAngle(0);

        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);

        // 毎フレーム消滅判定
        autoClear();

        noDamageCount++;
        noReduceCount++;
    }

    /// <summary>
    ///各種パラメータの初期設定
    /// </summary>
    protected virtual void setParamate()
    {
        if(Debug.isDebugBuild) displayAlignmentEffect = true;

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
        if(armIndex < 0) return siteAlignment;
        if(armIndex >= armStates.Count) return siteAlignment;

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
    ///照準画像の制御
    /// </summary>
    private Effect updateAlignmentEffect()
    {
        if(displayAlignmentEffect)
        {
            if(alignmentEffect == null)
            {
                var effect = alignmentSprite ?? sys.baseAlignmentSprite;
                alignmentEffect = outbreakEffect(effect);
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
    ///付属パーツの動作設定
    /// </summary>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        for(var index = 0; index < accessoryStates.Count; index++)
        {
            getParts<Accessory>(accessoryStates[index].partsNum).accessoryMotion(nowSpeed, index * 12);
        }
    }

    /// <summary>
    ///各種自然回復関数
    /// </summary>
    protected void recovery()
    {
        palamates.nowBarrier = Mathf.Min(palamates.nowBarrier + palamates.recoveryBarrier * (1 + noDamageCount * 0.01f), maxBarrier);
        palamates.nowFuel = Mathf.Min(palamates.nowFuel + palamates.recoveryFuel * (1 + noReduceCount * 0.01f), maxFuel);
    }
    /// <summary>
    ///燃料消費関数
    /// </summary>
    public bool reduceFuel(float reduceValue)
    {
        if(!canReduceFuel(reduceValue)) return false;
        noReduceCount = 0;
        palamates.nowFuel = Mathf.Max(palamates.nowFuel - reduceValue, 0);
        return true;
    }
    /// <summary>
    ///燃料消費可否関数
    /// </summary>
    public bool canReduceFuel(float reduceValue)
    {
        return palamates.nowFuel >= reduceValue;
    }
    /// <summary>
    ///生存判定関数
    /// </summary>
    public bool isAlive
    {
        get {
            return palamates.nowArmor > 0;
        }
    }

    /// <summary>
    ///ダメージ受けた時の統一動作
    /// </summary>
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
    public Vector2 setArmorBar(float maxPixel = 1, Vector2? basePosition = null)
    {
        Vector2 setedPosition = basePosition ?? new Vector2(-maxPixel / 2, baseSize.y / 2 + armorBarHeight);
        if(armorBar == null)
        {
            armorBar = Instantiate(sys.basicBar, setedPosition, Quaternion.AngleAxis(0, Vector3.forward));
            armorBar.nowParent = transform;
            armorBar.position = new Vector2(0, 0.5f);
        }

        var returnVector = armorBar.setLanges(palamates.nowArmor, maxArmor, maxPixel, setedPosition);
        return returnVector;
    }
    public void deleteArmorBar()
    {
        if(armorBar == null) return;
        Destroy(armorBar.gameObject);
        armorBar = null;
        return;
    }

    /// <summary>
    ///機体の破壊
    /// </summary>
    public override void selfDestroy(bool system = false)
    {
        // 爆発する
        if(!system) outbreakEffect(explosion);

        if(alignmentEffect != null) alignmentEffect.selfDestroy();

        base.selfDestroy(system);
    }

    /// <summary>
    ///腕パーツのセット
    /// </summary>
    public ArmState setArm(ArmState arm)
    {
        arm.partsNum = setOptionParts(arm.entity, arm);
        return arm;
    }
    /// <summary>
    ///アクセサリーのセット
    /// </summary>
    public AccessoryState setAccessory(AccessoryState accessory)
    {
        accessory.partsNum = setOptionParts(accessory.entity, accessory);

        if(accessory.partsNum >= 0)
        {
            getParts<Accessory>(accessory.partsNum).accessoryMotion(Vector2.zero);
        }

        return accessory;
    }
    /// <summary>
    ///武装のセット
    /// </summary>
    public WeaponSlot setWeapon(WeaponSlot weaponSlot)
    {
        weaponSlot.partsNum = setOptionParts(weaponSlot.entity, weaponSlot);

        var parts = getParts(weaponSlot.partsNum);
        if(parts != null)
        {
            parts.selfConnection = weaponSlot.entity.handlePosition;
            parts.GetComponent<Weapon>().setBaseAngle(weaponSlot.baseAngle);
        }

        return weaponSlot;
    }

    /// <summary>
    ///パーツのセット
    /// </summary>
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
    ///全武装の動作停止
    /// </summary>
    public void stopAllWeapon()
    {
        foreach(var weapon in allWeapons)
        {
            if(weapon != null) weapon.action(Weapon.ActionType.NOMOTION);
        }
    }

    void autoClear()
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
    ///自然停止ラッパー関数
    /// </summary>
    protected Vector2 thrustStop() => thrustStop(reactPower);
    /// <summary>
    ///自然停止動作関数
    /// </summary>
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
    /// <returns>コルーチン</returns>
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
}
