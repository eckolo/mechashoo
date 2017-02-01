using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 機体クラス
/// </summary>
public partial class Ship : Things
{
    /// <summary>
    /// 基礎パラメータ
    /// </summary>
    [System.Serializable]
    public class Palamates : ICopyAble<Palamates>
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
    [SerializeField]
    private Vector2 defaultAlignment = new Vector2(1, -0.5f);
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
    /// <summary>
    /// 照準表示フラグ
    /// </summary>
    protected bool displayAlignmentEffect = false;

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
    public class WeaponSlot : PartsState, ICopyAble<WeaponSlot>
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
                .Select(slot => getParts(slot.partsNum)?.GetComponent<Weapon>())
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
    public class ArmState : PartsState, ICopyAble<ArmState>
    {
        public Arm entity = null;

        public Vector2 alignment { get; set; }

        public new ArmState myself
        {
            get {
                return new ArmState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity,
                    alignment = alignment
                };
            }
        }
    }
    [SerializeField]
    private List<ArmState> armStates = new List<ArmState>();
    protected Vector2 armRoot
    {
        get {
            return armStates.Count > 0 ? armStates[0].rootPosition : Vector2.zero;
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
    public class AccessoryState : PartsState, ICopyAble<AccessoryState>
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

        setAllAlignment();
        for(int index = 0; index < armStates.Count; index++)
        {
            var arm = getParts(armStates[index].partsNum).GetComponent<Arm>();
            var hand = arm.tipHand;
            if(hand == null) continue;

            armStates[index].alignment = arm.setAlignment(armStates[index].alignment);
            var differenceAngle = -45 * Vector2.Angle(Vector2.left, armStates[index].alignment) / 180;
            arm.transform.Rotate(0, 0, differenceAngle * index);
            arm.childParts.transform.Rotate(0, 0, differenceAngle * -index);
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
    protected void setParamate()
    {
        if(Debug.isDebugBuild) displayAlignmentEffect = true;

        //紐づいたParts類の一掃
        deleteParts();

        //各種Nowパラメータの設定
        palamates.nowArmor = palamates.maxArmor;
        palamates.nowBarrier = palamates.maxBarrier;
        palamates.nowFuel = palamates.maxFuel;
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
                getParts(armStates[index].partsNum).GetComponent<Arm>().tipHand.setWeapon(this, weaponSlots[index].entity);
            }
            else
            {
                weaponSlots[index] = setWeapon(weaponSlots[index]);
            }
        }

        //照準を初期値に
        setAllAlignment(correctWidthVector(MathV.scaling(defaultAlignment, baseSize)));
    }
    /// <summary>
    ///全照準座標のリセット
    /// </summary>
    public void setAllAlignment(Vector2? setPosition = null)
    {
        siteAlignment = setPosition ?? siteAlignment;
        foreach(var arm in armStates) arm.alignment = siteAlignment;
    }
    /// <summary>
    ///照準画像の制御
    /// </summary>
    private Effect updateAlignmentEffect()
    {
        if(displayAlignmentEffect)
        {
            if(alignmentEffect == null)
            {
                alignmentEffect = outbreakEffect(alignmentSprite ?? sys.baseAlignmentSprite);
            }
        }
        else
        {
            if(alignmentEffect != null)
            {
                alignmentEffect.selfDestroy();
                alignmentEffect = null;
            }
        }
        if(alignmentEffect != null) alignmentEffect.position = position + armRoot + siteAlignment;
        return alignmentEffect;
    }

    /// <summary>
    ///付属パーツの動作設定
    /// </summary>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        for(var index = 0; index < accessoryStates.Count; index++)
            getParts(accessoryStates[index].partsNum)
                .GetComponent<Accessory>()
                .accessoryMotion(nowSpeed, index * 12);
    }

    /// <summary>
    ///各種自然回復関数
    /// </summary>
    protected void recovery()
    {
        palamates.nowBarrier = Mathf.Min(palamates.nowBarrier + palamates.recoveryBarrier * (1 + noDamageCount * 0.01f), palamates.maxBarrier);
        palamates.nowFuel = Mathf.Min(palamates.nowFuel + palamates.recoveryFuel * (1 + noReduceCount * 0.01f), palamates.maxFuel);
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
    public float receiveDamage(float damage, bool penetration = false, bool continuation = false)
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

        var returnVector = armorBar.setLanges(palamates.nowArmor, palamates.maxArmor, maxPixel, setedPosition);
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
        outbreakEffect(explosion);

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
            getParts(accessory.partsNum).GetComponent<Accessory>().accessoryMotion(Vector2.zero);
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

        setedParts.layer = layer;
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
        foreach(var armstate in armStates)
        {
            var hand = getParts(armstate.partsNum).GetComponent<Arm>().tipHand;
            if(hand != null) hand.actionWeapon(Weapon.ActionType.NOMOTION);
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
    ///自然停止ラッパー関数
    /// </summary>
    public Vector2 stopping() => stopping(reactPower);
}
