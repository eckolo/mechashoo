using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 機体クラス
/// </summary>
public class Ship : Material
{
    /// <summary>
    /// 装甲関係
    /// </summary>
    public float MaxArmor = 1;
    [SerializeField]
    protected float NowArmor;
    /// <summary>
    /// 装甲ゲージオブジェクト
    /// </summary>
    private Bar armorBar = null;
    /// <summary>
    /// 装甲ゲージオブジェクトのデフォルト位置パラメータ
    /// </summary>
    [SerializeField]
    protected float armorBarHeight;
    /// <summary>
    /// 障壁関係
    /// </summary>
    public float MaxBarrier = 1;
    public float recoveryBarrier = 0.1f;
    [SerializeField]
    protected float NowBarrier;
    /// <summary>
    /// 燃料関係
    /// </summary>
    public float MaxFuel = 1;
    public float recoveryFuel = 0.1f;
    [SerializeField]
    protected float NowFuel;
    /// <summary>
    /// 最大速度
    /// </summary>
    [SerializeField]
    protected float maxSpeed;
    /// <summary>
    /// 低速時の最大速度
    /// </summary>
    [SerializeField]
    protected float maxLowSpeed;
    /// <summary>
    /// 加速度
    /// </summary>
    [SerializeField]
    protected float acceleration;

    /// <summary>
    /// ダメージを受けてない期間のカウント
    /// </summary>
    [SerializeField]
    private ulong noDamageCount = 0;
    /// <summary>
    /// 燃料を消費していない期間のカウント
    /// </summary>
    [SerializeField]
    private ulong noReduceCount = 0;
    
    [SerializeField]
    protected Vector2 armRootPosition = Vector2.zero;
    [SerializeField]
    protected Vector2 accessoryRootPosition = Vector2.zero;
    [SerializeField]
    protected Vector2 weaponRootPosition = Vector2.zero;

    [SerializeField]
    protected Vector2 armPosition = Vector2.zero;

    [SerializeField]
    protected List<GameObject> defaultArms = new List<GameObject>();
    [SerializeField]
    protected List<Accessory> defaultAccessories = new List<Accessory>();
    [SerializeField]
    protected List<Weapon> defaultWeapons = new List<Weapon>();

    /// <summary>
    /// 爆発のPrefab
    /// </summary>
    [SerializeField]
    protected Explosion explosion;

    [SerializeField]
    protected List<int> armNumList = new List<int>();
    [SerializeField]
    protected List<int> accessoryNumList = new List<int>();
    [SerializeField]
    protected List<int> weaponNumList = new List<int>();

    // Use this for initialization
    protected override void baseStart()
    {
        setParamate();
    }

    // Update is called once per frame
    protected override void baseUpdate()
    {
        recovery();

        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * (widthPositive ? 1 : -1),
            transform.localScale.y,
            transform.localScale.z
            );

        if (!isAlive()) destroyMyself();

        foreach (var weaponNum in weaponNumList)
        {
            getParts(weaponNum).setManipulatePosition(Vector2.right);
        }

        if (getHand(getParts(0)) != null) armPosition = getParts(armNumList[0]).setManipulatePosition(armPosition);
        if (getHand(getParts(1)) != null)
        {
            armPosition = getParts(armNumList[1]).setManipulatePosition(armPosition);
            if (getHand(getParts(0)) != null)
            {
                var differenceAngle = -45 * Vector2.Angle(Vector2.left, armPosition) / 180;
                getParts(armNumList[1]).transform.Rotate(0, 0, differenceAngle);
                getParts(armNumList[1]).childParts.transform.Rotate(0, 0, differenceAngle * -1);
            }
        }

        //var color = GetComponent<SpriteRenderer>().color;
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
        NowArmor = MaxArmor;
        NowBarrier = MaxBarrier;
        NowFuel = MaxFuel;
        setArmorBar();

        //各種Listの初期化
        armNumList = new List<int>();
        accessoryNumList = new List<int>();
        weaponNumList = new List<int>();

        //腕パーツ設定
        foreach (var arm in defaultArms)
        {
            setArm(arm);
        }
        //羽パーツ設定
        foreach (var accessory in defaultAccessories)
        {
            var accessoryNum = setAccessory(accessory.gameObject);
            getParts(accessoryNumList[accessoryNum]).GetComponent<Accessory>().accessoryMotion(Vector2.zero);
        }
        //武装設定
        for (var seqNum = 0; seqNum < defaultWeapons.Count; seqNum++)
        {
            if (seqNum < armNumList.Count)
            {
                getHand(getParts(armNumList[seqNum]))
                    .setWeapon(GetComponent<Ship>(), defaultWeapons[seqNum], seqNum);
            }
            else
            {
                setWeapon(defaultWeapons[seqNum].gameObject);
            }
        }
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
        NowBarrier = Mathf.Min(NowBarrier + recoveryBarrier * (1 + noDamageCount * 0.01f), MaxBarrier);
        NowFuel = Mathf.Min(NowFuel + recoveryFuel * (1 + noReduceCount * 0.01f), MaxFuel);
    }
    /// <summary>
    ///燃料消費関数
    /// </summary>
    public bool reduceFuel(float reduceValue)
    {
        if (!canReduceFuel(reduceValue)) return false;
        noReduceCount = 0;
        NowFuel = Mathf.Max(NowFuel - reduceValue, 0);
        return true;
    }
    /// <summary>
    ///燃料消費可否関数
    /// </summary>
    public bool canReduceFuel(float reduceValue)
    {
        return NowFuel >= reduceValue;
    }
    /// <summary>
    ///生存判定関数
    /// </summary>
    public bool isAlive()
    {
        return NowArmor > 0;
    }

    /// <summary>
    ///ダメージ受けた時の統一動作
    /// </summary>
    public float receiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
        noDamageCount = 0;

        float surplusDamage = !penetration ? Mathf.Max(damage - NowBarrier, 0) : damage;
        if (!penetration) NowBarrier = Mathf.Max(NowBarrier - damage, 0);

        if (surplusDamage > 0)
        {
            //HPの操作
            NowArmor -= surplusDamage;

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
        Vector2 setedPosition = basePosition ?? new Vector2(-maxPixel / 2, armorBarHeight);
        if (armorBar == null)
        {
            armorBar = (Bar)Instantiate(getSystem().basicBar, setedPosition, Quaternion.AngleAxis(0, Vector3.forward));
            armorBar.transform.parent = transform;
            armorBar.transform.position = new Vector2(0, 0.5f);
        }

        var returnVector = armorBar.setLanges(NowArmor, MaxArmor, maxPixel, setedPosition);
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
        sequenceNum = sequenceNum < 0 ? armNumList.Count : sequenceNum;
        var partsNum = setParts(arm, sequenceNum);
        getParts(partsNum).parentConnection = armRootPosition;

        if (sequenceNum < armNumList.Count)
        {
            armNumList[sequenceNum] = partsNum;
        }
        else
        {
            armNumList.Add(partsNum);
            sequenceNum = armNumList.Count - 1;
        }

        return sequenceNum;
    }
    /// <summary>
    ///羽のセット
    /// </summary>
    public int setAccessory(GameObject accessory, int sequenceNum = -1)
    {
        sequenceNum = sequenceNum < 0 ? accessoryNumList.Count : sequenceNum;
        var partsNum = setParts(accessory, sequenceNum);

        if (partsNum >= 0)
        {
            getParts(partsNum).parentConnection = accessoryRootPosition;

            if (sequenceNum < accessoryNumList.Count)
            {
                accessoryNumList[sequenceNum] = partsNum;
            }
            else
            {
                accessoryNumList.Add(partsNum);
                sequenceNum = accessoryNumList.Count - 1;
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
            getParts(partsNum).parentConnection = weaponRootPosition;

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
            setedParts.GetComponent<Parts>().parentConnection = armRootPosition;

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
        var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(2, 2));
        var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(-1, -1));
        if (transform.position.x > upperRight.x
            || transform.position.x < lowerLeft.x
            || transform.position.y > upperRight.y
            || transform.position.y < lowerLeft.y)
        {
            selfDestroy();
        }
    }

    public void copyShipStatus(Ship originShip)
    {
        ShipData originShipData = originShip.outputShiData();

        GetComponent<SpriteRenderer>().sprite = originShipData.image;
        MaxArmor = originShipData.MaxArmor;
        armorBarHeight = originShipData.armorBarHeight;
        MaxBarrier = originShipData.MaxBarrier;
        recoveryBarrier = originShipData.recoveryBarrier;
        MaxFuel = originShipData.MaxFuel;
        recoveryFuel = originShipData.recoveryFuel;
        maxSpeed = originShipData.maxSpeed;
        maxLowSpeed = originShipData.maxLowSpeed;
        acceleration = originShipData.acceleration;
        armRootPosition = originShipData.armRootPosition;
        accessoryRootPosition = originShipData.accessoryRootPosition;
        weaponRootPosition = originShipData.weaponRootPosition;
        defaultArms = originShipData.defaultArms;
        defaultAccessories = originShipData.defaultAccessories;
        defaultWeapons = originShipData.defaultWeapons;
        explosion = originShipData.explosion;
        if (GetComponent<PolygonCollider2D>() != null) Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();

        baseStart();
        baseUpdate();
    }
    public ShipData outputShiData()
    {
        return new ShipData
        {
            image = GetComponent<SpriteRenderer>().sprite,
            MaxArmor = MaxArmor,
            armorBarHeight = armorBarHeight,
            MaxBarrier = MaxBarrier,
            recoveryBarrier = recoveryBarrier,
            MaxFuel = MaxFuel,
            recoveryFuel = recoveryFuel,
            maxSpeed = maxSpeed,
            maxLowSpeed = maxLowSpeed,
            acceleration = acceleration,
            armRootPosition = armRootPosition,
            accessoryRootPosition = accessoryRootPosition,
            weaponRootPosition = weaponRootPosition,
            defaultArms = defaultArms,
            defaultAccessories = defaultAccessories,
            defaultWeapons = defaultWeapons,
            explosion = explosion
        };
    }
    public class ShipData
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
        public Vector2 accessoryRootPosition = Vector2.zero;
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
