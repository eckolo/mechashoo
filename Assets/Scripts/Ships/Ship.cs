using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 機体クラス
/// </summary>
public class Ship : Things
{
    /// <summary>
    /// 基礎パラメータ
    /// </summary>
    [System.Serializable]
    public class Palamates
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
        /// 最大速度
        /// </summary>
        public float maxSpeed;
        /// <summary>
        /// 低速時の最大速度
        /// </summary>
        public float maxLowSpeed;
        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration;
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
    [System.NonSerialized]
    public Vector2 baseAlignment = Vector2.zero;
    /// <summary>
    /// 照準位置設定
    /// </summary>
    public void setAlignment(Vector2 setPosition)
    {
        baseAlignment = correctWidthVector(setPosition);
    }

    /// <summary>
    /// パーツパラメータベースクラス
    /// </summary>
    public class PartsState
    {
        public Vector2 rootPosition = Vector2.zero;
        public int positionZ = 1;

        public int partsNum { get; set; }
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
    public class WeaponSlot : PartsState
    {
        public Weapon entity = null;
        public float baseAngle = 0;
    }
    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [SerializeField]
    protected List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    public List<Weapon> weapons
    {
        get
        {
            return weaponSlots.Select(weaponSlot => weaponSlot.entity).ToList();
        }
    }
    public Ship setWeapon(int index, Weapon setWeapon = null)
    {
        coreData = coreData.setWeapon(weapons).setWeapon(index, setWeapon);
        return this;
    }

    /// <summary>
    /// 腕部パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class ArmState : PartsState
    {
        public Parts entity = null;

        public Vector2 alignment { get; set; }
    }
    [SerializeField]
    protected List<ArmState> armStates = new List<ArmState>();

    /// <summary>
    /// 付属パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class AccessoryState : PartsState
    {
        public Accessory entity = null;
    }
    [SerializeField]
    protected List<AccessoryState> accessoryStates = new List<AccessoryState>();

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

        if (!isAlive) destroyMyself();

        for (int index = 0; index < armStates.Count; index++)
        {
            var arm = getParts(armStates[index].partsNum);
            var hand = getHand(arm);
            if (hand == null) continue;

            armStates[index].alignment = arm.setAlignment(armStates[index].alignment);
            var differenceAngle = -45 * Vector2.Angle(Vector2.left, armStates[index].alignment) / 180;
            arm.transform.Rotate(0, 0, differenceAngle * index);
            arm.childParts.transform.Rotate(0, 0, differenceAngle * -index);
        }

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
        for (var index = 0; index < weaponSlots.Count; index++)
        {
            weaponSlots[index].partsNum = -1;
            if (weaponSlots[index].entity == null) continue;
            if (index < armStates.Count)
            {
                getHand(getParts(armStates[index].partsNum))
                    .setWeapon(this, weaponSlots[index].entity, weaponSlots[index]);
            }
            else
            {
                weaponSlots[index] = setWeapon(weaponSlots[index]);
            }
        }

        //照準を初期値に
        setAllAlignment(Vector2.right * baseSize.x - Vector2.up * baseSize.y / 2);
    }
    /// <summary>
    ///全照準座標のリセット
    /// </summary>
    public void setAllAlignment(Vector2 setPosition)
    {
        baseAlignment = setPosition;
        foreach (var arm in armStates) arm.alignment = setPosition;
    }

    /// <summary>
    ///付属パーツの動作設定
    /// </summary>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        for (var index = 0; index < accessoryStates.Count; index++)
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
        if (!canReduceFuel(reduceValue)) return false;
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
        get
        {
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
        if (!penetration) palamates.nowBarrier = Mathf.Max(palamates.nowBarrier - damage, 0);

        if (surplusDamage > 0)
        {
            //HPの操作
            palamates.nowArmor -= surplusDamage;

            if (!continuation) GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0.6f, 1);
        }
        else
        {
            if (!continuation) GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.6f, 1);
        }

        setArmorBar();

        return surplusDamage;
    }
    public Vector2 setArmorBar(float maxPixel = 1, Vector2? basePosition = null)
    {
        Vector2 setedPosition = basePosition ?? new Vector2(-maxPixel / 2, baseSize.y / 2 + armorBarHeight);
        if (armorBar == null)
        {
            armorBar = (Bar)Instantiate(Sys.basicBar, setedPosition, Quaternion.AngleAxis(0, Vector3.forward));
            armorBar.transform.parent = transform;
            armorBar.transform.localPosition = new Vector2(0, 0.5f);
        }

        var returnVector = armorBar.setLanges(palamates.nowArmor, palamates.maxArmor, maxPixel, setedPosition);
        return returnVector;
    }
    public void deleteArmorBar()
    {
        if (armorBar == null) return;
        Destroy(armorBar.gameObject);
        armorBar = null;
        return;
    }

    /// <summary>
    ///機体の破壊
    /// </summary>
    public void destroyMyself(bool fromPlayer = true)
    {
        // 破壊時の独自アクション実行
        onDestroyAction(fromPlayer);

        // 爆発する
        outbreakEffect(explosion);

        // 機体を削除
        Destroy(gameObject);
    }
    /// <summary>
    ///破壊時の独自アクション
    /// </summary>
    protected virtual void onDestroyAction(bool fromPlayer)
    {
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

        if (accessory.partsNum >= 0)
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
        if (parts != null)
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
        var setedParts = (Parts)Instantiate(parts, (Vector2)transform.position, transform.rotation);

        setLayer(setedParts.gameObject);
        setedParts.transform.parent = transform;
        setedParts.transform.localScale = new Vector3(1, 1, 1);

        var partsNum = setParts(setedParts);
        if (partsNum >= 0)
        {
            setedParts.parentConnection = partsState.rootPosition;
            setZ(setedParts.transform, GetComponent<SpriteRenderer>().sortingOrder, partsState.positionZ);
        }

        return partsNum;
    }

    protected Hand getHand(Parts target)
    {
        if (target == null) return null;
        if (target.childParts == null) return target.GetComponent<Hand>();
        if (target.GetComponent<Hand>() != null) return target.GetComponent<Hand>();
        return getHand(target.childParts);
    }

    public void setZ(Transform origin, int originZ, int once = 1)
    {
        var weaponData = origin.GetComponent<Weapon>();
        var addNum = weaponData != null ? weaponData.defaultZ : once;
        origin.GetComponent<SpriteRenderer>().sortingOrder = originZ + addNum;
        foreach (Transform child in origin)
        {
            setZ(child, origin.GetComponent<SpriteRenderer>().sortingOrder, once);
        }
    }

    public void setLayer(GameObject origin, int layer = -1)
    {
        origin.layer = layer < 0 ? gameObject.layer : layer;
        foreach (Transform child in origin.transform)
        {
            setLayer(child.gameObject, layer);
        }
    }

    void autoClear()
    {
        var upperRight = fieldUpperRight * 2;
        var lowerLeft = fieldLowerLeft * 2;
        if (transform.position.x > upperRight.x
            || transform.position.x < lowerLeft.x
            || transform.position.y > upperRight.y
            || transform.position.y < lowerLeft.y)
        {
            selfDestroy();
        }
    }

    public CoreData coreData
    {
        get
        {
            return new CoreData
            {
                name = gameObject.name,
                image = GetComponent<SpriteRenderer>().sprite,
                armorBarHeight = armorBarHeight,
                explosion = explosion,

                palamates = palamates,
                armStates = armStates,
                accessoryStates = accessoryStates,
                weaponSlots = weaponSlots
            };
        }
        set
        {
            value = value ?? new CoreData();
            var keepWeaponData = value.weapons;

            GetComponent<SpriteRenderer>().sprite = value.image;
            armorBarHeight = value.armorBarHeight;
            explosion = value.explosion;

            palamates = value.palamates;
            armStates = value.armStates;
            accessoryStates = value.accessoryStates;
            weaponSlots = value.weaponSlots;

            setParamate();
            value.setWeapon(keepWeaponData);
        }
    }
    public class CoreData
    {
        public string name = null;
        public Sprite image = null;

        public Palamates palamates = new Palamates();
        public List<ArmState> armStates = new List<ArmState>();
        public List<AccessoryState> accessoryStates = new List<AccessoryState>();
        public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();

        public List<Weapon> weapons
        {
            get
            {
                return weaponSlots.Select(slot => slot.entity).ToList();
            }
            set
            {
                for (int index = 0; index < weaponSlots.Count; index++) weaponSlots[index].entity = index < value.Count ? value[index] : null;
            }
        }

        public float armorBarHeight = 0.5f;
        public Explosion explosion;

        public CoreData setWeapon(List<Weapon> setWeapons = null)
        {
            weapons = setWeapons ?? new List<Weapon>();
            return this;
        }
        public CoreData setWeapon(int index, Weapon setWeapon = null)
        {
            if (index < 0) return this;
            if (index >= weapons.Count) return this;

            var setWeapons = weapons;
            setWeapons[index] = setWeapon;
            weapons = setWeapons;
            return this;
        }
    }
}
