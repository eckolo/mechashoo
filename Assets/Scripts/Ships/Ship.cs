using UnityEngine;
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
    protected class Palamates
    {
        /// <summary>
        /// 装甲関係
        /// </summary>
        public float maxArmor = 1;
        [System.NonSerialized]
        public float nowArmor;
        /// <summary>
        /// 障壁関係
        /// </summary>
        public float maxBarrier = 1;
        public float recoveryBarrier = 0.1f;
        [System.NonSerialized]
        public float nowBarrier;
        /// <summary>
        /// 燃料関係
        /// </summary>
        public float maxFuel = 1;
        public float recoveryFuel = 0.1f;
        [System.NonSerialized]
        public float nowFuel;
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
    private Bar armorBar = null;
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
    /// 位置パラメータ
    /// </summary>
    [System.Serializable]
    protected class Positions
    {
        public Vector2 armRoot = Vector2.zero;
        public List<Vector2> accessoryRoots = new List<Vector2>();
        public Vector2 weaponRoot = Vector2.zero;
        [System.NonSerialized]
        public Vector2 baseAlignment = Vector2.zero;
    }
    /// <summary>
    /// 位置パラメータ
    /// </summary>
    [SerializeField]
    protected Positions positions = new Positions();
    /// <summary>
    /// 照準位置設定
    /// </summary>
    public void setAlignment(Vector2 setPosition)
    {
        positions.baseAlignment = correctWidthVector(setPosition);
    }

    /// <summary>
    /// パーツパラメータ
    /// </summary>
    [System.Serializable]
    protected class Defaults
    {
        public List<GameObject> arms = new List<GameObject>();
        public List<Accessory> accessories = new List<Accessory>();
        public List<Weapon> weapons = new List<Weapon>();
    }
    /// <summary>
    /// パーツパラメータ
    /// </summary>
    [SerializeField]
    protected Defaults defaults = new Defaults();

    /// <summary>
    /// 爆発のPrefab
    /// </summary>
    [SerializeField]
    protected Explosion explosion;

    /// <summary>
    /// パーツパラメータ
    /// </summary>
    protected class ArmState
    {
        public int num;
        public Vector2 alignment = Vector2.zero;
    }
    protected List<ArmState> armList = new List<ArmState>();


    protected List<int> accessoryNumList = new List<int>();
    protected List<int> weaponNumList = new List<int>();

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

        foreach (var weaponNum in weaponNumList)
        {
            getParts(weaponNum).setManipulator(Vector2.right);
        }

        var rightArm = armList.Count >= 1 ? getParts(armList[0].num) : null;
        var leftArm = armList.Count >= 2 ? getParts(armList[1].num) : null;

        if (getHand(rightArm) != null) armList[0].alignment = rightArm.setAlignment(armList[0].alignment);
        if (getHand(leftArm) != null)
        {
            leftArm.setAlignment(armList[1].alignment);
            if (getHand(rightArm) != null)
            {
                var differenceAngle = -45 * Vector2.Angle(Vector2.left, armList[1].alignment) / 180;
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
        armList = new List<ArmState>();
        accessoryNumList = new List<int>();
        weaponNumList = new List<int>();

        //腕パーツ設定
        foreach (var arm in defaults.arms)
        {
            setArm(arm);
        }
        //羽パーツ設定
        foreach (var accessory in defaults.accessories)
        {
            var accessoryNum = setAccessory(accessory.gameObject);
            getParts(accessoryNumList[accessoryNum]).GetComponent<Accessory>().accessoryMotion(Vector2.zero);
        }
        //武装設定
        for (var seqNum = 0; seqNum < defaults.weapons.Count; seqNum++)
        {
            if (seqNum < armList.Count)
            {
                getHand(getParts(armList[seqNum].num))
                    .setWeapon(GetComponent<Ship>(), defaults.weapons[seqNum], seqNum);
            }
            else
            {
                setWeapon(defaults.weapons[seqNum].gameObject);
            }
        }

        //照準を初期値に
        setAllAlignment(Vector2.right * baseSize.x);
    }
    /// <summary>
    ///全照準座標のリセット
    /// </summary>
    public void setAllAlignment(Vector2 setPosition)
    {
        positions.baseAlignment = setPosition;
        foreach (var arm in armList) arm.alignment = setPosition;
    }

    /// <summary>
    ///付属パーツの動作設定
    /// </summary>
    protected override void setVerosityAction(Vector2 acceleration)
    {
        for (var index = 0; index < accessoryNumList.Count; index++)
            getParts(accessoryNumList[index])
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
    public int setArm(GameObject arm, int sequenceNum = -1)
    {
        sequenceNum = sequenceNum < 0 ? armList.Count : sequenceNum;
        var partsNum = setParts(arm, sequenceNum);
        getParts(partsNum).parentConnection = positions.armRoot;

        if (sequenceNum < armList.Count)
        {
            armList[sequenceNum].num = partsNum;
        }
        else
        {
            armList.Add(new ArmState { num = partsNum });
            sequenceNum = armList.Count - 1;
        }

        return sequenceNum;
    }
    /// <summary>
    ///羽のセット
    /// </summary>
    public int setAccessory(GameObject accessory, int? seq = null)
    {
        int sequenceNum = seq ?? accessoryNumList.Count;
        var partsNum = setParts(accessory, sequenceNum);

        if (partsNum >= 0)
        {
            getParts(partsNum).parentConnection = positions.accessoryRoots[sequenceNum];

            if (seq < accessoryNumList.Count)
            {
                accessoryNumList[sequenceNum] = partsNum;
            }
            else
            {
                accessoryNumList.Add(partsNum);
                seq = accessoryNumList.Count - 1;
            }
        }

        return sequenceNum;
    }
    /// <summary>
    ///武装のセット
    /// </summary>
    public int setWeapon(GameObject weapon, int sequenceNum = -1)
    {
        sequenceNum = sequenceNum < 0 ? weaponNumList.Count : sequenceNum;
        var partsNum = setParts(weapon, sequenceNum);

        if (partsNum >= 0)
        {
            getParts(partsNum).traceRoot = true;
            getParts(partsNum).selfConnection = weapon.GetComponent<Weapon>().handlePosition;
            getParts(partsNum).parentConnection = positions.weaponRoot;

            if (sequenceNum < weaponNumList.Count)
            {
                weaponNumList[sequenceNum] = partsNum;
            }
            else
            {
                weaponNumList.Add(partsNum);
                sequenceNum = weaponNumList.Count - 1;
            }
        }

        return sequenceNum;
    }

    /// <summary>
    ///パーツのセット
    /// </summary>
    private int setParts(GameObject parts, int sequenceNum)
    {
        var setedParts = (GameObject)Instantiate(parts, (Vector2)transform.position, transform.rotation);

        setLayer(setedParts);
        setedParts.transform.parent = transform;
        setedParts.transform.localScale = new Vector3(1, 1, 1);

        var partsNum = setParts(setedParts.GetComponent<Parts>());
        if (partsNum >= 0)
        {
            setedParts.GetComponent<Parts>().parentConnection = positions.armRoot;

            setZ(setedParts.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);
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
        setCoreStatus(originShip.coreData);
    }
    public void setCoreStatus(CoreData originShipData)
    {
        GetComponent<SpriteRenderer>().sprite = originShipData.image;
        palamates.maxArmor = originShipData.MaxArmor;
        armorBarHeight = originShipData.armorBarHeight;
        palamates.maxBarrier = originShipData.MaxBarrier;
        palamates.recoveryBarrier = originShipData.recoveryBarrier;
        palamates.maxFuel = originShipData.MaxFuel;
        palamates.recoveryFuel = originShipData.recoveryFuel;
        palamates.maxSpeed = originShipData.maxSpeed;
        palamates.maxLowSpeed = originShipData.maxLowSpeed;
        palamates.acceleration = originShipData.acceleration;
        positions.armRoot = originShipData.armRootPosition;
        positions.accessoryRoots = originShipData.accessoryRootPosition;
        positions.weaponRoot = originShipData.weaponRootPosition;
        defaults.arms = originShipData.defaultArms;
        defaults.accessories = originShipData.defaultAccessories;
        defaults.weapons = originShipData.defaultWeapons;
        explosion = originShipData.explosion;

        Start();
        Update();
    }
    public CoreData coreData
    {
        get
        {
            return new CoreData
            {
                image = GetComponent<SpriteRenderer>().sprite,
                MaxArmor = palamates.maxArmor,
                armorBarHeight = armorBarHeight,
                MaxBarrier = palamates.maxBarrier,
                recoveryBarrier = palamates.recoveryBarrier,
                MaxFuel = palamates.maxFuel,
                recoveryFuel = palamates.recoveryFuel,
                maxSpeed = palamates.maxSpeed,
                maxLowSpeed = palamates.maxLowSpeed,
                acceleration = palamates.acceleration,
                armRootPosition = positions.armRoot,
                accessoryRootPosition = positions.accessoryRoots,
                weaponRootPosition = positions.weaponRoot,
                defaultArms = defaults.arms,
                defaultAccessories = defaults.accessories,
                defaultWeapons = defaults.weapons,
                explosion = explosion
            };
        }
    }
    public class CoreData
    {
        public Sprite image = null;
        /// <summary>
        /// 装甲関係
        /// </summary>
        public float MaxArmor = 1;
        public float armorBarHeight = 0.5f;
        /// <summary>
        /// 障壁関係
        /// </summary>
        public float MaxBarrier = 1;
        public float recoveryBarrier = 0.1f;
        /// <summary>
        /// 燃料関係
        /// </summary>
        public float MaxFuel = 1;
        public float recoveryFuel = 0.1f;
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

        public Vector2 armRootPosition = Vector2.zero;
        public List<Vector2> accessoryRootPosition = new List<Vector2>();
        public Vector2 weaponRootPosition = Vector2.zero;

        public List<GameObject> defaultArms = new List<GameObject>();
        public List<Accessory> defaultAccessories = new List<Accessory>();
        public List<Weapon> defaultWeapons = new List<Weapon>();

        /// <summary>
        /// 爆発のPrefab
        /// </summary>
        public Explosion explosion;

        public Vector2 accessoryBaseVector = Vector2.zero;

        public Vector2[] points;
    }
}
