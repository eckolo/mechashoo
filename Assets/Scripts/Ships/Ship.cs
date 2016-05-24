using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
    // 装甲残量
    public int MaxHP = 1;
    public int NowHP;
    // 移動スピード
    public float speed;
    public bool positive = true;

    public Vector2 armRootPosition = new Vector2(0, 0);
    public Vector2 wingRootPosition = new Vector2(0, 0);

    public Vector2 armPosition = new Vector2(0, 0);
    public Vector2 wingPosition = new Vector2(0, 0);

    public List<GameObject> defaultArms = new List<GameObject>();
    public List<GameObject> defaultWings = new List<GameObject>();
    public List<GameObject> defaultWeapons = new List<GameObject>();

    // 爆発のPrefab
    public GameObject explosion;

    public Vector2 verosity = new Vector2(0, 0);

    public List<int> armNumList = new List<int>();
    public List<int> wingNumList = new List<int>();

    // Use this for initialization
    public void Start()
    {
        NowHP = MaxHP;
        foreach (var arm in defaultArms)
        {
            var seqNum = setArm(arm);
            getHand(GetComponent<Root>().childPartsList[armNumList[seqNum]].GetComponent<Parts>())
                .GetComponent<Hand>()
                .setWeapon(GetComponent<Ship>(), defaultWeapons[seqNum], seqNum);
        }
        foreach (var wing in defaultWings)
        {
            setWing(wing);
        }
        plusStart();
    }
    public virtual void plusStart() { }

    // Update is called once per frame
    public void Update()
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
        plusUpdate();
    }
    public virtual void plusUpdate() { }

    // 機体の移動
    public void Move(Vector2 direction, float inputSpeed)
    {
        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        verosity = direction.normalized * inputSpeed;
        pos += verosity * Time.deltaTime;

        // 制限をかけた値をプレイヤーの位置とする
        transform.position = pos;
    }

    public bool instructAction(int sequenceNum)
    {
        var hand = getHand(GetComponent<Root>().childPartsList[armNumList[sequenceNum]].GetComponent<Parts>());
        return hand.GetComponent<Hand>().actionWeapon();
    }

    //リアクターの基本動作
    private void wingMotion(Vector2 baseVector, float limitRange = 0.3f)
    {
        if (wingNumList.Count == 2)
        {
            var Root = GetComponent<Root>();
            var baseWingPosition = baseVector.normalized / 6;

            wingPosition.x = (verosity.y != 0)
                ? wingPosition.x - verosity.y / 100
                : wingPosition.x * 9 / 10;
            wingPosition.y = (verosity.x != 0)
                ? wingPosition.y + verosity.x * (positive ? 1 : -1) / 100
                : wingPosition.y * 9 / 10;

            if (wingPosition.magnitude > limitRange) wingPosition = wingPosition.normalized * limitRange;
            wingPosition = Root.setManipulatePosition(wingPosition + baseWingPosition, Root.childPartsList[wingNumList[0]], false) - baseWingPosition;

            Root.setManipulatePosition(Quaternion.Euler(0, 0, 12) * (wingPosition + baseWingPosition), Root.childPartsList[wingNumList[1]], false);
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
        GetComponent<Root>().childPartsList.Add(setedArm.GetComponent<Parts>());

        setedArm.GetComponent<Parts>().parentConnection = armRootPosition;

        setZ(setedArm.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        if (sequenceNum < armNumList.Count)
        {
            armNumList[sequenceNum] = GetComponent<Root>().childPartsList.Count - 1;
        }
        else
        {
            armNumList.Add(GetComponent<Root>().childPartsList.Count - 1);
        }

        return sequenceNum;
    }

    private GameObject getHand(Parts target)
    {
        return (target.GetComponent<Hand>() != null)
            ? target.gameObject
            : getHand(target.childParts);
    }

    public void setZ(Transform origin, int originZ, int once = 1)
    {
        origin.GetComponent<SpriteRenderer>().sortingOrder = originZ + once;
        foreach (Transform child in origin)
        {
            setZ(child, originZ + once, once);
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
            GetComponent<Root>().childPartsList.Add(setedWing.GetComponent<Parts>());
            setedWing.GetComponent<Parts>().parentConnection = wingRootPosition;

            if (sequenceNum < wingNumList.Count)
            {
                wingNumList[sequenceNum] = GetComponent<Root>().childPartsList.Count - 1;
            }
            else
            {
                wingNumList.Add(GetComponent<Root>().childPartsList.Count - 1);
            }
        }

        setZ(setedWing.transform, GetComponent<SpriteRenderer>().sortingOrder, sequenceNum % 2 == 0 ? 1 : -1);

        return sequenceNum;
    }
}
