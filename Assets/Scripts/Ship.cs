using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
    // 移動スピード
    public float speed;
    public bool positive = true;

    public Vector2 armPosition = new Vector2(0, 0);
    public Vector2 wingPosition = new Vector2(0, 0);

    public List<Weapon> weapons = new List<Weapon>();

    // 爆発のPrefab
    public GameObject explosion;

    public Vector2 verosity = new Vector2(0, 0);

    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * (positive ? 1 : -1),
            transform.localScale.y,
            transform.localScale.z
            );
        wingMotion(new Vector2(-6, 1));
    }

    // 機体の移動
    public void Move(Vector2 direction, float inputSpeed)
    {
        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        verosity = direction * inputSpeed;
        pos += verosity * Time.deltaTime;

        // 制限をかけた値をプレイヤーの位置とする
        transform.position = pos;
    }

    //リアクターの基本動作
    private void wingMotion(Vector2 baseVector, float limitRange = 0.3f)
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
        wingPosition = Root.setManipulatePosition(wingPosition + baseWingPosition, Root.childPartsList[2], false) - baseWingPosition;

        Root.setManipulatePosition(Quaternion.Euler(0, 0, 12) * (wingPosition + baseWingPosition), Root.childPartsList[3], false);
    }

    // ぶつかった瞬間に呼び出される
    void OnTriggerEnter2D(Collider2D c)
    {
        // 弾の削除
        Destroy(c.gameObject);

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

    public GameObject setWeapon(GameObject weapon, int sequenceNum)
    {
        var parentAngle = GetComponent<Root>().childPartsList[sequenceNum].transform.rotation;
        var childAngle = GetComponent<Root>().childPartsList[sequenceNum].GetComponent<Parts>().childParts.transform.rotation;

        Destroy(GetComponent<Root>().childPartsList[sequenceNum].gameObject);

        var setedWeapon = (GameObject)Instantiate(weapon, (Vector2)transform.position, transform.rotation);
        setedWeapon.transform.rotation = parentAngle;
        setedWeapon.GetComponent<Parts>().childParts.transform.rotation = childAngle;

        setedWeapon.transform.parent = transform;
        GetComponent<Root>().childPartsList[sequenceNum] = setedWeapon.GetComponent<Parts>();

        weapons[sequenceNum] = setedWeapon.GetComponent<Parts>().childParts.GetComponent<Weapon>();

        return weapon;
    }
}
