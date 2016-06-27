using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 機体クラス
/// </summary>
public class Ship : Material
{
    /// <summary>
    /// 装甲残量
    /// </summary>
    public int MaxHP = 1;
    [SerializeField]
    private int NowHP;
    /// <summary>
    /// 移動スピード
    /// </summary>
    public float speed;

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
    private Explosion explosion;

    [SerializeField]
    protected List<int> armNumList = new List<int>();
    [SerializeField]
    protected List<int> accessoryNumList = new List<int>();
    [SerializeField]
    protected List<int> weaponNumList = new List<int>();

    [SerializeField]
    private Vector2 accessoryBaseVector = new Vector2(0, 0);

    // Use this for initialization
    protected override void baseStart()
    {
        //HP設定
        NowHP = MaxHP;
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

    // Update is called once per frame
    protected override void baseUpdate()
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * (widthPositive ? 1 : -1),
            transform.localScale.y,
            transform.localScale.z
            );
        accessoryMotion(accessoryBaseVector);

        if (NowHP <= 0) destroyMyself();

        foreach (var weaponNum in weaponNumList)
        {
            getParts(weaponNum).setManipulatePosition(Vector2.right);
        }

        //var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);
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
    public int receiveDamage(int damage)
    {
        //HPの操作
        NowHP -= damage;

        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0.6f, 1);

        return damage;
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
}
