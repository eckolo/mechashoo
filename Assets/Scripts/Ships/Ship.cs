using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : Material
{
    // 装甲残量
    public int MaxHP = 1;
    [SerializeField]
    private int NowHP;
    // 移動スピード
    public float speed;
    public bool positive = true;

    [SerializeField]
    protected Vector2 armRootPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 wingRootPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 weaponRootPosition = new Vector2(0, 0);

    [SerializeField]
    protected Vector2 armPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 wingPosition = new Vector2(0, 0);

    [SerializeField]
    protected List<GameObject> defaultArms = new List<GameObject>();
    [SerializeField]
    protected List<GameObject> defaultWings = new List<GameObject>();
    [SerializeField]
    protected List<Weapon> defaultWeapons = new List<Weapon>();

    // 爆発のPrefab
    [SerializeField]
    private Explosion explosion;

    [SerializeField]
    protected List<int> armNumList = new List<int>();
    [SerializeField]
    protected List<int> wingNumList = new List<int>();
    [SerializeField]
    protected List<int> weaponNumList = new List<int>();

    // Use this for initialization
    public virtual void Start()
    {
        //HP設定
        NowHP = MaxHP;
        //腕パーツ設定
        foreach (var arm in defaultArms)
        {
            setArm(arm);
        }
        //羽パーツ設定
        foreach (var wing in defaultWings)
        {
            setWing(wing);
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
    public virtual void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * (positive ? 1 : -1),
            transform.localScale.y,
            transform.localScale.z
            );
        wingMotion(new Vector2(-6, 1));

        if (NowHP <= 0) destroyMyself();

        foreach (var weaponNum in weaponNumList)
        {
            getParts(weaponNum).setManipulatePosition(Vector2.right);
        }

        //var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);
    }

    //リアクターの基本動作
    private void wingMotion(Vector2 baseVector, float limitRange = 0.3f)
    {
        if (wingNumList.Count == 2)
        {
            var baseWingPosition = baseVector.normalized / 6;
            var verosity = GetComponent<Rigidbody2D>().velocity;

            wingPosition.x = (verosity.y != 0)
                ? wingPosition.x - verosity.y / 100
                : wingPosition.x * 9 / 10;
            wingPosition.y = (verosity.x != 0)
                ? wingPosition.y + verosity.x * (positive ? 1 : -1) / 100
                : wingPosition.y * 9 / 10;

            if (wingPosition.magnitude > limitRange) wingPosition = wingPosition.normalized * limitRange;
            wingPosition = getParts(wingNumList[0]).setManipulatePosition(wingPosition + baseWingPosition, false) - baseWingPosition;

            getParts(wingNumList[1]).setManipulatePosition(Quaternion.Euler(0, 0, 12) * (wingPosition + baseWingPosition), false);
        }
    }

    //ダメージ受けた時の統一動作
    public int receiveDamage(int damage)
    {
        //HPの操作
        NowHP -= damage;

        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0.6f, 1);

        return damage;
    }

    //機体の破壊
    public void destroyMyself()
    {
        // 爆発する
        Explosion();

        // プレイヤーを削除
        Destroy(gameObject);
    }

    // 爆発の作成
    public void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
    }

    //腕パーツのセット
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
    //羽のセット
    public int setWing(GameObject wing, int sequenceNum = -1)
    {
        sequenceNum = sequenceNum < 0 ? wingNumList.Count : sequenceNum;
        var partsNum = setParts(wing, sequenceNum);

        if (partsNum >= 0)
        {
            getParts(partsNum).parentConnection = wingRootPosition;

            if (sequenceNum < wingNumList.Count)
            {
                wingNumList[sequenceNum] = partsNum;
            }
            else
            {
                wingNumList.Add(partsNum);
            }
        }

        return sequenceNum;
    }
    //武装のセット
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

    //パーツのセット
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
