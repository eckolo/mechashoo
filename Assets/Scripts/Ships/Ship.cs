using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : Object
{
    // 装甲残量
    public int MaxHP = 1;
    [SerializeField]
    private int NowHP;
    // 移動スピード
    public float speed;
    public bool positive = true;

    public Parts childParts = null;

    [SerializeField]
    protected Vector2 armRootPosition = new Vector2(0, 0);
    [SerializeField]
    protected Vector2 wingRootPosition = new Vector2(0, 0);

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
        //武装設定
        foreach (var weapon in defaultWeapons)
        {
            var seqNum = defaultWeapons.IndexOf(weapon);
            if (seqNum < armNumList.Count)
            {
                getHand(getParts(armNumList[seqNum]).GetComponent<Parts>())
                    .setWeapon(GetComponent<Ship>(), defaultWeapons[seqNum].gameObject, seqNum);
            }
            else
            {
                setParts(weapon.GetComponent<Parts>());
            }
        }
        //羽パーツ設定
        foreach (var wing in defaultWings)
        {
            setWing(wing);
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

        //var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color + new Color(0.01f, 0.01f, 0.01f, 0);
    }

    public bool instructAction(int sequenceNum)
    {
        var hand = getHand(getParts(armNumList[sequenceNum]).GetComponent<Parts>());
        return hand.GetComponent<Hand>().actionWeapon();
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

        var setedArm = (GameObject)Instantiate(arm, (Vector2)transform.position, transform.rotation);

        setLayer(setedArm);
        setedArm.transform.parent = transform;
        setedArm.transform.localScale = new Vector3(1, 1, 1);
        var partsNum = setParts(setedArm.GetComponent<Parts>());

        setedArm.GetComponent<Parts>().parentConnection = armRootPosition;

        setZ(setedArm.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

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

    private Hand getHand(Parts target)
    {
        if (target.childParts == null) return target.GetComponent<Hand>();
        if (target.GetComponent<Hand>() != null) return target.GetComponent<Hand>();
        return getHand(target.childParts);
    }

    public void setZ(Transform origin, int originZ, int once = 1)
    {
        var addNum = once * (origin.GetComponent<Weapon>() != null ? 1 : -1);
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

    //羽のセット
    public int setWing(GameObject wing, int sequenceNum = -1)
    {
        sequenceNum = sequenceNum < 0 ? wingNumList.Count : sequenceNum;

        var setedWing = (GameObject)Instantiate(wing, (Vector2)transform.position, transform.rotation);

        setLayer(setedWing);
        setedWing.transform.parent = transform;
        setedWing.transform.localScale = new Vector3(1, 1, 1);

        if (setedWing.GetComponent<Parts>() != null)
        {
            var partsNum = setParts(setedWing.GetComponent<Parts>());
            setedWing.GetComponent<Parts>().parentConnection = wingRootPosition;

            if (sequenceNum < wingNumList.Count)
            {
                wingNumList[sequenceNum] = partsNum;
            }
            else
            {
                wingNumList.Add(partsNum);
            }
        }

        setZ(setedWing.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        return sequenceNum;
    }
}
