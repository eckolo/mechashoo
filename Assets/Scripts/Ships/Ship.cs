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
    public float speed;
    /// <summary>
    /// 加速度
    /// </summary>
    public float acceleration;

    [SerializeField]
    protected Vector2 armRootPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 accessoryRootPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 weaponRootPosition = new Vector2(0, 0);

    [SerializeField]
    protected Vector2 armPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 accessoryPosition = new Vector2(0, 0);

    [SerializeField]
    protected List<GameObject> defaultArms = new List<GameObject>();
    [SerializeField]
    protected List<GameObject> defaultAccessories = new List<GameObject>();
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

    [SerializeField]
    protected Vector2 accessoryBaseVector = new Vector2(0, 0);

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
        accessoryMotion(accessoryBaseVector);

        if (NowArmor <= 0) destroyMyself();

        foreach (var weaponNum in weaponNumList)
        {
            getParts(weaponNum).setManipulatePosition(Vector2.right);
        }

        //var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);
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
            setAccessory(accessory);
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
    ///各種自然回復関数
    /// </summary>
    protected void recovery()
    {
        NowBarrier = Mathf.Min(NowBarrier + recoveryBarrier, MaxBarrier);
        NowFuel = Mathf.Min(NowFuel + recoveryFuel, MaxFuel);
    }

    /// <summary>
    ///リアクターの基本動作
    /// </summary>
    private void accessoryMotion(Vector2 baseVector, float limitRange = 0.3f)
    {
        if (accessoryNumList.Count == 2)
        {
            var baseAccessoryPosition = baseVector.normalized / 6;
            var verosity = GetComponent<Rigidbody2D>().velocity;

            accessoryPosition.x = (verosity.y != 0)
                ? accessoryPosition.x - verosity.y / 100
                : accessoryPosition.x * 9 / 10;
            accessoryPosition.y = (verosity.x != 0)
                ? accessoryPosition.y + verosity.x * (widthPositive ? 1 : -1) / 100
                : accessoryPosition.y * 9 / 10;

            if (accessoryPosition.magnitude > limitRange) accessoryPosition = accessoryPosition.normalized * limitRange;
            accessoryPosition = getParts(accessoryNumList[0]).setManipulatePosition(accessoryPosition + baseAccessoryPosition, false) - baseAccessoryPosition;

            getParts(accessoryNumList[1]).setManipulatePosition(Quaternion.Euler(0, 0, 12) * (accessoryPosition + baseAccessoryPosition), false);
        }
    }

    /// <summary>
    ///ダメージ受けた時の統一動作
    /// </summary>
    public float receiveDamage(float damage, bool penetration = false, bool continuation = false)
    {
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

        return surplusDamage;
    }

    /// <summary>
    ///機体の破壊
    /// </summary>
    public void destroyMyself()
    {
        // 破壊時の独自アクション実行
        onDestroyAction();

        // 爆発する
        Explosion();

        // プレイヤーを削除
        Destroy(gameObject);
    }
    /// <summary>
    ///破壊時の独自アクション
    /// </summary>
    protected virtual void onDestroyAction()
    {
    }

    /// <summary>
    /// 爆発の作成
    /// </summary>
    public void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
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
        var addNum = origin.GetComponent<Weapon>() == null ? once : -1;
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

    public ShipData outputShiData()
    {
        return new ShipData
        {
            image = GetComponent<SpriteRenderer>().sprite,
            MaxArmor = MaxArmor,
            MaxBarrier = MaxBarrier,
            recoveryBarrier = recoveryBarrier,
            MaxFuel = MaxFuel,
            recoveryFuel = recoveryFuel,
            speed = speed,
            acceleration = acceleration,
            armRootPosition = armRootPosition,
            accessoryRootPosition = accessoryRootPosition,
            weaponRootPosition = weaponRootPosition,
            defaultArms = defaultArms,
            defaultAccessories = defaultAccessories,
            defaultWeapons = defaultWeapons,
            explosion = explosion,
            accessoryBaseVector = accessoryBaseVector
        };
    }

    public class ShipData
    {
        public Sprite image = null;
        /// <summary>
        /// 装甲関係
        /// </summary>
        public float MaxArmor = 1;
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
        public float speed;
        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration;

        public Vector2 armRootPosition = new Vector2(0, 0);
        public Vector2 accessoryRootPosition = new Vector2(0, 0);
        public Vector2 weaponRootPosition = new Vector2(0, 0);

        public List<GameObject> defaultArms = new List<GameObject>();
        public List<GameObject> defaultAccessories = new List<GameObject>();
        public List<Weapon> defaultWeapons = new List<Weapon>();

        /// <summary>
        /// 爆発のPrefab
        /// </summary>
        public Explosion explosion;

        public Vector2 accessoryBaseVector = new Vector2(0, 0);
    }
}
