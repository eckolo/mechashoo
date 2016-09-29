﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    /// 爆発のPrefab
    /// </summary>
    [SerializeField]
    protected Explosion explosion;

    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [System.Serializable]
    public class WeaponSlot
    {
        public Weapon weapon = null;
        public Vector2 position = Vector2.zero;
        public float baseAngle = 0;
        public int partsNum = 0;
        public int positionZ = 1;
        public WeaponSlot slotEmpty
        {
            get
            {
                weapon = null;
                return this;
            }
        }
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
            var result = new List<Weapon>();
            for (int i = 0; i < weaponSlots.Count; i++) result.Add(weaponSlots[i].weapon);
            return result;
        }
    }
    public Ship setWeaponData(int index, Weapon setWeapon = null)
    {
        Debug.Log(displayName + " [" + index + "] " + setWeapon);
        setCoreStatus(coreData.setWeaponData(weapons).setWeaponData(index, setWeapon));
        return this;
    }

    /// <summary>
    /// 腕部パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class ArmState
    {
        public Parts entity = null;
        public int partsNum;
        public Vector2 alignment = Vector2.zero;
        public Vector2 rootPosition = Vector2.zero;
        public int positionZ = 1;
    }
    [SerializeField]
    protected List<ArmState> armStates = new List<ArmState>();

    /// <summary>
    /// 付属パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class AccessoryState
    {
        public Accessory entity = null;
        public int partsNum;
        public Vector2 rootPosition = Vector2.zero;
        public int positionZ = 1;
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

        var rightArm = armStates.Count >= 1 ? getParts(armStates[0].partsNum) : null;
        var leftArm = armStates.Count >= 2 ? getParts(armStates[1].partsNum) : null;

        if (getHand(rightArm) != null) armStates[0].alignment = rightArm.setAlignment(armStates[0].alignment);
        if (getHand(leftArm) != null)
        {
            leftArm.setAlignment(armStates[1].alignment);
            if (getHand(rightArm) != null)
            {
                var differenceAngle = -45 * Vector2.Angle(Vector2.left, armStates[1].alignment) / 180;
                leftArm.transform.Rotate(0, 0, differenceAngle);
                leftArm.childParts.transform.Rotate(0, 0, differenceAngle * -1);
            }
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

        //各種Listの初期化
        armStates = new List<ArmState>();

        //腕パーツ設定
        for (int index = 0; index < armStates.Count; index++)
        {
            armStates[index] = setArm(armStates[index]);
        }
        //羽パーツ設定
        for (int index = 0; index < accessoryStates.Count; index++)
        {
            accessoryStates[index] = setAccessory(accessoryStates[index]);
        }
        //武装設定
        for (var index = 0; index < weaponSlots.Count; index++)
        {
            if (weaponSlots[index].weapon == null) continue;
            if (index < armStates.Count)
            {
                getHand(getParts(armStates[index].partsNum))
                    .setWeapon(GetComponent<Ship>(), weaponSlots[index].weapon, index);
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
        arm.partsNum = setOptionParts(arm.entity, arm.positionZ);
        if (arm.partsNum >= 0) getParts(arm.partsNum).parentConnection = arm.rootPosition;

        return arm;
    }
    /// <summary>
    ///羽のセット
    /// </summary>
    public AccessoryState setAccessory(AccessoryState accessory)
    {
        accessory.partsNum = setOptionParts(accessory.entity, accessory.positionZ);

        if (accessory.partsNum >= 0)
        {
            getParts(accessory.partsNum).parentConnection = accessory.rootPosition;

            getParts(accessory.partsNum).GetComponent<Accessory>().accessoryMotion(Vector2.zero);
        }

        return accessory;
    }
    /// <summary>
    ///武装のセット
    /// </summary>
    public WeaponSlot setWeapon(WeaponSlot weaponSlot)
    {
        weaponSlot.partsNum = setOptionParts(weaponSlot.weapon, weaponSlot.positionZ);

        if (weaponSlot.partsNum >= 0)
        {
            getParts(weaponSlot.partsNum).traceRoot = true;
            getParts(weaponSlot.partsNum).selfConnection = weaponSlot.weapon.handlePosition;
            getParts(weaponSlot.partsNum).parentConnection = weaponSlot.position;
        }

        return weaponSlot;
    }

    /// <summary>
    ///パーツのセット
    /// </summary>
    private int setOptionParts(Parts parts, int positionZ)
    {
        var setedParts = (Parts)Instantiate(parts, (Vector2)transform.position, transform.rotation);

        setLayer(setedParts.gameObject);
        setedParts.transform.parent = transform;
        setedParts.transform.localScale = new Vector3(1, 1, 1);

        var partsNum = setParts(setedParts);
        if (partsNum >= 0)
        {
            setZ(setedParts.transform, GetComponent<SpriteRenderer>().sortingOrder, positionZ);
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

    public void setCoreStatus(Ship originShip)
    {
        setCoreStatus(originShip != null ? originShip.coreData : null);
    }
    public void setCoreStatus(CoreData originShipData)
    {
        originShipData = originShipData ?? new CoreData();
        GetComponent<SpriteRenderer>().sprite = originShipData.image;
        armorBarHeight = originShipData.armorBarHeight;
        explosion = originShipData.explosion;

        palamates = originShipData.palamates;
        armStates = originShipData.armStates;
        accessoryStates = originShipData.accessoryStates;
        weaponSlots = originShipData.weaponSlots;

        Start();
        Update();
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
    }
    public class CoreData
    {
        public string name = null;
        public Sprite image = null;

        public Palamates palamates = new Palamates();
        public List<ArmState> armStates = new List<ArmState>();
        public List<AccessoryState> accessoryStates = new List<AccessoryState>();
        public List<WeaponSlot> weaponSlots
        {
            get
            {
                return _weaponSlots;
            }
            set
            {
                _weaponSlots = new List<WeaponSlot>();
                for (int i = 0; i < value.Count; i++) _weaponSlots.Add(value[i].slotEmpty);
            }
        }
        private List<WeaponSlot> _weaponSlots = new List<WeaponSlot>();
        public List<Weapon> weapons
        {
            get
            {
                var weaponList = new List<Weapon>();
                foreach (var weaponSlot in weaponSlots) weaponList.Add(weaponSlot.weapon);
                return weaponList;
            }
            set
            {
                for (int index = 0; index < value.Count && index < weaponSlots.Count; index++) weaponSlots[index].weapon = value[index];
            }
        }

        public float armorBarHeight = 0.5f;
        public Explosion explosion;

        public CoreData setWeaponData(List<Weapon> setWeapons = null)
        {
            setWeapons = setWeapons ?? new List<Weapon>();
            for (int index = 0; index < setWeapons.Count && index < weaponSlots.Count; index++) weapons[index] = setWeapons[index];
            return this;
        }
        public CoreData setWeaponData(int index, Weapon setWeapon = null)
        {
            if (index < 0) return this;
            if (index >= weaponSlots.Count) return this;
            weapons[index] = setWeapon;
            return this;
        }
    }
}
